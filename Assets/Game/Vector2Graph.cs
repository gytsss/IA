using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

namespace Pathfinder
{
    public class Vector2Graph : Graph<Node<Vector2>, Vector2>
    {
        public Vector2Graph(int x, int y, float distance) : base(x, y, distance)
        {
        }

        public override void GenerateGraph(int x, int y, float distance)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Node<Vector2> node = new Node<Vector2>();
                    node.SetCoordinate(new Vector2((i * distance), (j * distance)));
                    nodes.Add(node);
                }
            }
        }

        public override void GetNeighbors(float distance)
        {
            foreach (var node in nodes)
            {
                var nodeCoordinate = node.GetCoordinate();

                nodes.ForEach(neighbor =>
                {
                    var neighborCoordinate = neighbor.GetCoordinate();
                    if ((neighborCoordinate.x == nodeCoordinate.x &&
                         Math.Abs(neighborCoordinate.y - nodeCoordinate.y) <= distance) ||
                        (neighborCoordinate.y == nodeCoordinate.y &&
                         Math.Abs(neighborCoordinate.x - nodeCoordinate.x) <= distance) ||
                        (Math.Abs(neighborCoordinate.x - nodeCoordinate.x) <= distance &&
                         Math.Abs(neighborCoordinate.y - nodeCoordinate.y) <= distance))
                    {
                        node.AddNeighbor(neighbor);
                    }
                });
                
            }
            
        }

        public List<INode<Vector2>> GetNeighborsNode(Vector2 nodeCoordinate)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if(nodes[i].GetCoordinate() == nodeCoordinate)
                    return nodes[i].GetNeighbors();
            }
            
            return null;
        }
    }
}