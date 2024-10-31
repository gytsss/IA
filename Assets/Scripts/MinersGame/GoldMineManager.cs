using System.Collections.Generic;
using System.Linq;
using Pathfinder;
using TreeEditor;
using UnityEngine;

public class GoldMineManager : MonoBehaviour
{
    public GraphView graphView;
    public List<GoldMineNode<Vector2>> goldMines = new List<GoldMineNode<Vector2>>();
    public List<NodeVoronoi> goldMinesVoronois = new List<NodeVoronoi>();

    public int goldAmount = 0;
    public int foodAmount = 0;

    public void SetGoldMines(int count, float distanceBetweenNodes)
    {
        for (int i = 0; i < count; i++)
        {
            GoldMineNode<Vector2> goldMine = new GoldMineNode<Vector2>();
            int randomNode = Random.Range(0, graphView.Graph.nodes.Count);
           //int randomNode = 1;
           

            goldMine.SetCoordinate(graphView.Graph.nodes[randomNode].GetCoordinate());
            goldMine.SetMaxFoodAmount(100);
            goldMine.SetMaxGoldAmount(100);
            goldMine.SetFoodAmount(foodAmount);
            goldMine.SetGoldAmount(goldAmount);
            goldMine.SetNeighbors(graphView.Graph.nodes[randomNode].PassNeighbors());
            goldMines.Add(goldMine);

            graphView.Graph.nodes.Remove(graphView.Graph.nodes[randomNode]);
            graphView.Graph.nodes.Add(goldMine);
            goldMinesVoronois.Add(new NodeVoronoi(goldMine.GetCoordinate()));
        }

        graphView.goldMines = goldMines.Cast<Node<Vector2>>().ToList();
    }

    public GoldMineNode<Vector2> FindClosestGoldMine(Node<Vector2> startNode)
    {
        GoldMineNode<Vector2> closestMine = null;
        float closestDistance = float.MaxValue;

        foreach (var mine in goldMines)
        {
            float distance = Vector2.Distance(mine.GetCoordinate(), startNode.GetCoordinate());
            if (distance < closestDistance && mine.HasGold())
            {
                closestDistance = distance;
                closestMine = mine;
            }
        }

        return closestMine;
    }

    public GoldMineNode<Vector2> FindClosestGoldMineBeingMined(Node<Vector2> startNode)
    {
        GoldMineNode<Vector2> closestMine = null;
        float closestDistance = float.MaxValue;

        foreach (GoldMineNode<Vector2> mine in goldMines)
        {
            float distance = Vec2Int.Distance(mine.GetCoordinate(), startNode.GetCoordinate());
            if (distance < closestDistance && mine.HasGold() && mine.IsBeingMined())
            {
                closestDistance = distance;
                closestMine = mine;
            }
        }

        return closestMine;
    }
}