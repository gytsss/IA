using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinder;
using Pathfinder.Voronoi;
using UnityEditor.Experimental.GraphView;
using Direction = Pathfinder.Voronoi.Direction;

namespace Voronoi
{
    public class Voronoi<TCoordinate, TCoordinateType>
        where TCoordinate : IEquatable<TCoordinate>, ICoordinate<TCoordinateType>, new()
        where TCoordinateType : IEquatable<TCoordinateType>
    {
        private readonly List<Limit<TCoordinate, TCoordinateType>> limits = new();
        private readonly List<Sector<TCoordinate,TCoordinateType>> sectors = new();

        public void Init(TCoordinateType size, float distanceBetweenNodes, TCoordinateType origin)
        {
            InitLimits(size, distanceBetweenNodes, origin);
        }

        private void InitLimits(TCoordinateType size, float distanceBetweenNodes, TCoordinateType origin)
        {
            // Calculo los limites del mapa con sus dimensiones, distancia entre nodos y punto de origen
            TCoordinate mapSize = new TCoordinate();
            mapSize.SetCoordinate(size);
            mapSize.Multiply(distanceBetweenNodes);
            TCoordinate offset = new TCoordinate();
            offset.SetCoordinate(origin);

            
            TCoordinate coordinateUp = new TCoordinate();
            coordinateUp.SetCoordinate(0, mapSize.GetY());
            coordinateUp.Add(offset.GetCoordinate());
            limits.Add(new Limit<TCoordinate, TCoordinateType>(coordinateUp, Direction.Up));
            
            TCoordinate coordinateDown = new TCoordinate();
            coordinateDown.SetCoordinate(mapSize.GetX(), 0);
            coordinateDown.Add(offset.GetCoordinate());
            limits.Add(new Limit<TCoordinate, TCoordinateType>(coordinateDown, Direction.Down));
            
            TCoordinate coordinateRight = new TCoordinate();
            coordinateRight.SetCoordinate(mapSize.GetX(), mapSize.GetY());
            coordinateRight.Add(offset.GetCoordinate());
            limits.Add(new Limit<TCoordinate, TCoordinateType>(coordinateRight, Direction.Right));
            
            TCoordinate coordinateLeft = new TCoordinate();
            coordinateLeft.SetCoordinate(0, 0);
            coordinateLeft.Add(offset.GetCoordinate());
            limits.Add(new Limit<TCoordinate, TCoordinateType>(coordinateLeft, Direction.Left));
        }

        public void SetVoronoi(List<TCoordinate> goldMines, TCoordinate mapSize)
        {
            sectors.Clear();
            if (goldMines.Count <= 0) return;

            foreach (var mine in goldMines)
            {
                // Agrego las minas de oro como sectores
                Node<TCoordinateType> node = new Node<TCoordinateType>();
                node.SetCoordinate(mine.GetCoordinate());
                sectors.Add(new Sector<TCoordinate, TCoordinateType>(node));
            }

            foreach (var sector in sectors)
            {
                // Agrego los limites a cada sector
                sector.AddSegmentLimits(limits);
            }

            for (int i = 0; i < goldMines.Count; i++)
            {
                for (int j = 0; j < goldMines.Count; j++)
                {
                    // Agrego los segmentos entre cada sector (menos entre si mismo)
                    if (i == j) continue;
                    sectors[i].AddSegment(goldMines[i], goldMines[j]);
                }
            }

            foreach (var sector in sectors)
            {
                // Calculo las intersecciones
                sector.SetIntersections(mapSize);
            }
        }

        public Node<TCoordinateType> GetMineCloser(TCoordinate agentPosition)
        {
            // Calculo que mina esta mas cerca a x position
            return sectors != null ? (from sector in sectors 
                where sector.CheckPointInSector(agentPosition) select sector.Mine).FirstOrDefault() : null;
        }

        public List<Sector<TCoordinate,TCoordinateType>> SectorsToDraw()
        {
            return sectors;
        }
    }
}