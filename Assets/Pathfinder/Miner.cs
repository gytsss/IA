using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MinerStates
{
    MoveToMine,
    MineGold,
    EatFood,
    DepositGold,
    WaitFood,
    ReturnHome,
    Alarm
}

public enum MinerFlags
{
    OnMineFind,
    OnGoldExtract,
    OnFoodNeed,
    OnFoodEaten,
    OnFullInventory,
    OnMineEmpty,
    OnNoFood,
    OnAlarmTrigger
}


public class Miner : MonoBehaviour
{
    public GraphView graphView;
    public GoldMineManager goldMineManager;
    
    private AStarPathfinder<Node<Vector2Int>> Pathfinder;
    //private DijkstraPathfinder<Node<Vector2Int>> Pathfinder;
    //private DepthFirstPathfinder<Node<Vector2Int>> Pathfinder;
    //private BreadthPathfinder<Node<Vector2Int>> Pathfinder;
    
    private Node<Vector2Int> currentMine;
    private Node<Vector2Int> urbanCenter;
    
    private FSM<MinerStates, MinerFlags> fsm;
    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;

    public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;
    
    public float travelTime = 0.70f;
    public int maxGold = 15;
    public int goldCollected = 0;
    public int goldBetweenFood = 3;
    
    
    private void Start()
    {
        // fsm = new FSM<MinerStates, MinerFlags>();
        //
        // fsm.AddBehaviour<MoveToMineState>(MinerStates.MoveToMine, onTickParameters: MoveToMineTickParameters);
        // fsm.AddBehaviour<MineGoldState>(MinerStates.MineGold, onTickParameters: MineGoldTickParameters);
        // fsm.AddBehaviour<EatFoodState>(MinerStates.EatFood, onTickParameters: EatFoodTickParameters);
        // fsm.AddBehaviour<DepositGoldState>(MinerStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        // fsm.AddBehaviour<WaitFoodState>(MinerStates.WaitFood, onTickParameters: WaitForFoodTickParameters);
        // fsm.AddBehaviour<ReturnHomeState>(MinerStates.ReturnHome, onTickParameters: ReturnToUrbanCenterTickParameters);
        // fsm.AddBehaviour<AlarmState>(MinerStates.Alarm, onTickParameters: RespondToAlarmTickParameters);
        //
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFullInventory, MinerStates.DepositGold);
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFoodNeed, MinerStates.EatFood);
        // fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        // fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnNoFood, MinerStates.WaitFood);
        // fsm.SetTransition(MinerStates.WaitFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnMineEmpty, MinerStates.MoveToMine);
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnAlarmTrigger, MinerStates.ReturnHome);
        // fsm.SetTransition(MinerStates.ReturnHome, MinerFlags.OnAlarmTrigger, MinerStates.MoveToMine);
        //
        // fsm.ForceState(MinerStates.MoveToMine);
    }
    
    private object[] MoveToMineTickParameters()
    {
        return new object[] { this, currentMine, travelTime};
    }

    private object[] MineGoldTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] EatFoodTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] WaitForFoodTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] DepositGoldTickParameters()
    {
        return new object[] { this, urbanCenter };
    }

    private object[] ReturnToUrbanCenterTickParameters()
    {
        return new object[] { this, urbanCenter };
    }

    private object[] RespondToAlarmTickParameters()
    {
        return new object[] { this, urbanCenter };
    }

    private void Update()
    {
        //fsm.Tick();
    }
    
    public void GetMapInputValues()
    {
        string height = heightInputField.text;
        string width = widthInputField.text;
        string goldMines = goldMinesInputField.text;
        string distanceBetweenNodes = distanceBetweenNodesInputField.text.Replace(',', '.');

        Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines + " Distance: " +
                  distanceBetweenNodes);

        graphView.CreateGraph(int.Parse(height), int.Parse(width), float.Parse(distanceBetweenNodes));
        goldMineManager.SetGoldMines(int.Parse(goldMines), float.Parse(distanceBetweenNodes));
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
        
        destinationNode = goldMineManager.FindClosestGoldMine(startNode);

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
            yield return new WaitForSeconds(travelTime);
        }

        Debug.Log("Destination reached! x: " + destinationNode.GetCoordinate().x + " y: " +
                  destinationNode.GetCoordinate().y);
    }
}