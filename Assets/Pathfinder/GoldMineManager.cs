using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

public class GoldMineManager : MonoBehaviour
{
    public GraphView graphView;
    public List<GoldMineNode<Vector2Int>> goldMines = new List<GoldMineNode<Vector2Int>>();

    public void SetGoldMines(int count, float distanceBetweenNodes)
    {
        for (int i = 0; i < count; i++)
        {
            GoldMineNode<Vector2Int> goldMine = new GoldMineNode<Vector2Int>();
            Node<Vector2Int> randomNode = graphView.Graph.nodes[Random.Range(0, graphView.Graph.nodes.Count)];
            
            goldMine.SetCoordinate(randomNode.GetCoordinate());
            //goldMine.SetGoldAmount(Random.Range(10, 15));
            //goldMine.SetFoodAmount(Random.Range(2, 5));
            goldMines.Add(goldMine);
            
            graphView.Graph.nodes.Remove(randomNode);
            graphView.Graph.nodes.Add(goldMine);
        }

        graphView.goldMines = goldMines.Cast<Node<Vector2Int>>().ToList(); 
    }
    
    public GoldMineNode<Vector2Int> FindClosestGoldMine(Node<Vector2Int> startNode)
    {
        GoldMineNode<Vector2Int> closestMine = null;
        float closestDistance = float.MaxValue;

        foreach (GoldMineNode<Vector2Int> mine in goldMines)
        {
            float distance = Vector2Int.Distance(mine.GetCoordinate(), startNode.GetCoordinate());
                if (distance < closestDistance && mine.HasGold())
                {
                    closestDistance = distance;
                    closestMine = mine;
                }
            
        }

        return closestMine;
    }
}