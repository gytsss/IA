using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

public class GoldMineManager : MonoBehaviour
{
    public GraphView graphView;
    public List<GoldMineNode<Vec2Int>> goldMines = new List<GoldMineNode<Vec2Int>>();
    
    public int goldAmount = 3;
    public int foodAmount = 5;

    public void SetGoldMines(int count, float distanceBetweenNodes)
    {
        for (int i = 0; i < count; i++)
        {
            GoldMineNode<Vec2Int> goldMine = new GoldMineNode<Vec2Int>();
            Node<Vec2Int> randomNode = graphView.Graph.nodes[Random.Range(0, graphView.Graph.nodes.Count)];
            
            goldMine.SetCoordinate(randomNode.GetCoordinate());
            //goldMine.SetGoldAmount(Random.Range(10, 15));
            //goldMine.SetFoodAmount(Random.Range(2, 5));
            goldMine.SetMaxFoodAmount(100);
            goldMine.SetMaxGoldAmount(100);
            goldMine.SetFoodAmount(foodAmount);
            goldMine.SetGoldAmount(goldAmount);
            goldMines.Add(goldMine);
            
            graphView.Graph.nodes.Remove(randomNode);
            graphView.Graph.nodes.Add(goldMine);
        }

        graphView.goldMines = goldMines.Cast<Node<Vec2Int>>().ToList(); 
    }
    
    public GoldMineNode<Vec2Int> FindClosestGoldMine(Node<Vec2Int> startNode)
    {
        GoldMineNode<Vec2Int> closestMine = null;
        float closestDistance = float.MaxValue;

        foreach (GoldMineNode<Vec2Int> mine in goldMines)
        {
            float distance = Vec2Int.Distance(mine.GetCoordinate(), startNode.GetCoordinate());
                if (distance < closestDistance && mine.HasGold())
                {
                    closestDistance = distance;
                    closestMine = mine;
                }
            
        }

        return closestMine;
    }
}