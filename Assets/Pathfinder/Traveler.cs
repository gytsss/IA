using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Traveler : MonoBehaviour
{
    public GraphView graphView;

    private AStarPathfinder<Node<Vector2Int>> Pathfinder;
    //private DijkstraPathfinder<Node<Vector2Int>> Pathfinder;
    //private DepthFirstPathfinder<Node<Vector2Int>> Pathfinder;
    //private BreadthPathfinder<Node<Vector2Int>> Pathfinder;

    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;

    public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;

    private List<Node<Vector2Int>> goldMines = new List<Node<Vector2Int>>();


    public void GetMapInputValues()
    {
        string height = heightInputField.text;
        string width = widthInputField.text;
        string goldMines = goldMinesInputField.text;
        string distanceBetweenNodes = distanceBetweenNodesInputField.text.Replace(',', '.');

        Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines + " Distance: " +
                  distanceBetweenNodes);

        graphView.CreateGraph(int.Parse(height), int.Parse(width), float.Parse(distanceBetweenNodes));
        SetGoldMines(int.Parse(goldMines), float.Parse(distanceBetweenNodes));
        InitTraveler(float.Parse(distanceBetweenNodes));
    }

    private void InitTraveler(float distanceBetweenNodes)
    {
        Pathfinder = new AStarPathfinder<Node<Vector2Int>>(graphView.Graph, distanceBetweenNodes);
        //Pathfinder = new DijkstraPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new BreadthPathfinder<Node<Vector2Int>>(graphView.Graph);

        startNode = new Node<Vector2Int>();
        startNode.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));

        //destinationNode = new Node<Vector2Int>();
        //destinationNode.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));

        destinationNode = FindClosestGoldMine(startNode);

        graphView.startNode = startNode;
        graphView.destinationNode = destinationNode;
        
        if (startNode == null || destinationNode == null)
        {
            Debug.LogError("Start node or destination node is null.");
            return;
        }

        List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes);
        
        if (path == null)
        {
            Debug.LogError("Path is null. No valid path found.");
            return;
        }

        graphView.pathNodes = path;

        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path)
    {
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(0.70f);
        }

        Debug.Log("Destination reached! x: " + destinationNode.GetCoordinate().x + " y: " +
                  destinationNode.GetCoordinate().y);
    }

    private void SetGoldMines(int count, float distance)
    {
        goldMines.Clear();

        for (int i = 0; i < count; i++)
        {
            Node<Vector2Int> goldMine = new Node<Vector2Int>();
            Node<Vector2Int> randomNode = graphView.Graph.nodes[Random.Range(0, graphView.Graph.nodes.Count)];
            goldMine.SetCoordinate(randomNode.GetCoordinate());
            goldMine.SetIsGoldMine(true);
            goldMines.Add(goldMine);

            graphView.Graph.nodes.Add(goldMine);
        }

        graphView.goldMines = goldMines;
    }

    private Node<Vector2Int> FindClosestGoldMine(Node<Vector2Int> startNode)
    {
        Node<Vector2Int> closestMine = null;
        float closestDistance = float.MaxValue;

        foreach (Node<Vector2Int> mine in goldMines)
        {
            float distance = Vector2Int.Distance(mine.GetCoordinate(), startNode.GetCoordinate());
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMine = mine;
            }
        }

        return closestMine;
    }
}