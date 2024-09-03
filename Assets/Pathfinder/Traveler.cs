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

    public TMP_InputField heightInputField, widthInputField, goldMinesInputField;
    
    private List<Node<Vector2Int>> goldMines = new List<Node<Vector2Int>>();

    void Start()
    {
        
    }

    public void GetMapInputValues()
    {
        string height = heightInputField.text;
        string width = widthInputField.text;
        string goldMines = goldMinesInputField.text;
        
        Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines);

        graphView.CreateGraph(int.Parse(height), int.Parse(width));
        SetGoldMines(int.Parse(goldMines));
        InitTraveler();
    }

    private void InitTraveler()
    {
        Pathfinder = new AStarPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new DijkstraPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new BreadthPathfinder<Node<Vector2Int>>(graphView.Graph);
        
        startNode = new Node<Vector2Int>();
        startNode.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));

        destinationNode = new Node<Vector2Int>();
        destinationNode.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));

        graphView.startNode = startNode;
        graphView.destinationNode = destinationNode;

        List<Node<Vector2Int>> path = Pathfinder.FindPath(startNode, destinationNode);
        
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
        
        Debug.Log("Destination reached!");
    }

    private void SetGoldMines(int count)
    {
        goldMines.Clear();

        for (int i = 0; i < count; i++)
        {
            Node<Vector2Int> goldMine = new Node<Vector2Int>();
            goldMine.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));
            goldMine.SetIsGoldMine(true);
            goldMines.Add(goldMine);

            graphView.Graph.nodes.Add(goldMine);
        }

        graphView.goldMines = goldMines;
    }
}
