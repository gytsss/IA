using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MinerStates
{
    Idle,
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
    OnStart,
    OnMineFind,
    OnFoodNeed,
    OnFoodEaten,
    OnFullInventory,
    OnMineEmpty,
    OnMineEmptyOfFood,
    OnGoldDeposit,
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
    private UrbanCenterNode<Vector2Int> urbanCenter;

    private FSM<MinerStates, MinerFlags> fsm;
    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;

    private Node<Vector2Int> currentNode;
    private List<Node<Vector2Int>> path;
    private float distanceBetweenNodes = 0;

    public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;

    public float travelTime = 0.70f;
    public int maxGold = 15;
    public int goldCollected = 0;
    public float goldExtractionSpeed = 1f;
    public int energy = 3;
    public int maxEnergy = 3;
    private bool start = false;


    private void Start()
    {
        fsm = new FSM<MinerStates, MinerFlags>();

        fsm.AddBehaviour<IdleState>(MinerStates.Idle, onTickParameters: () => { return new object[] { start }; });

        fsm.SetTransition(MinerStates.Idle, MinerFlags.OnStart, MinerStates.MoveToMine);
        
        fsm.ForceState(MinerStates.Idle);
    }

    private object[] MoveToMineTickParameters()
    {
        return new object[] { this as Miner, this.transform, destinationNode, startNode, travelTime, distanceBetweenNodes, path };
    }

    private object[] MineGoldTickParameters()
    {
        return new object[] { this, currentMine, goldExtractionSpeed, maxGold };
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
        return new object[] { this, currentNode, urbanCenter, travelTime, distanceBetweenNodes };
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
        fsm.Tick();
    }

    public void GetMapInputValues()
    {
        string height = heightInputField.text;
        string width = widthInputField.text;
        string goldMines = goldMinesInputField.text;
        distanceBetweenNodes = float.Parse(distanceBetweenNodesInputField.text.Replace(',', '.'));

        Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines + " Distance: " +
                  distanceBetweenNodes);

        graphView.CreateGraph(int.Parse(height), int.Parse(width), distanceBetweenNodes);
        goldMineManager.SetGoldMines(int.Parse(goldMines), distanceBetweenNodes);
        start = true;
        InitTraveler(distanceBetweenNodes);
    }

    private void InitTraveler(float distanceBetweenNodes)
    {
        Pathfinder = new AStarPathfinder<Node<Vector2Int>>(graphView.Graph, distanceBetweenNodes);
        //Pathfinder = new DijkstraPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new DepthFirstPathfinder<Node<Vector2Int>>(graphView.Graph);
        //Pathfinder = new BreadthPathfinder<Node<Vector2Int>>(graphView.Graph);

        urbanCenter = new UrbanCenterNode<Vector2Int>();
        urbanCenter.SetCoordinate(new Vector2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));
        currentNode = urbanCenter;
        startNode = urbanCenter;
        destinationNode = goldMineManager.FindClosestGoldMine(startNode);

        graphView.startNode = startNode;
        graphView.destinationNode = destinationNode;
        graphView.urbanCenterNode = urbanCenter;
        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);
        
        path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes);
        
        
        fsm.AddBehaviour<MoveToMineState>(MinerStates.MoveToMine, onTickParameters: MoveToMineTickParameters);
        fsm.AddBehaviour<MineGoldState>(MinerStates.MineGold, onTickParameters: MineGoldTickParameters);
        fsm.AddBehaviour<EatFoodState>(MinerStates.EatFood, onTickParameters: EatFoodTickParameters);
        fsm.AddBehaviour<DepositGoldState>(MinerStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        // fsm.AddBehaviour<WaitFoodState>(MinerStates.WaitFood, onTickParameters: WaitForFoodTickParameters);
        // fsm.AddBehaviour<ReturnHomeState>(MinerStates.ReturnHome, onTickParameters: ReturnToUrbanCenterTickParameters);
        // fsm.AddBehaviour<AlarmState>(MinerStates.Alarm, onTickParameters: RespondToAlarmTickParameters);
        //
         

        fsm.SetTransition(MinerStates.MoveToMine, MinerFlags.OnMineFind, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFoodNeed, MinerStates.EatFood);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFullInventory, MinerStates.DepositGold);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnMineEmpty, MinerStates.MoveToMine);
        fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.DepositGold, MinerFlags.OnGoldDeposit, MinerStates.MoveToMine);
        // fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnNoFood, MinerStates.WaitFood);
        // fsm.SetTransition(MinerStates.WaitFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnAlarmTrigger, MinerStates.ReturnHome);
        // fsm.SetTransition(MinerStates.ReturnHome, MinerFlags.OnAlarmTrigger, MinerStates.MoveToMine);
        

        graphView.pathNodes = path;
        
    }

    public Node<Vector2Int> GetCurrentNode()
    {
        return currentNode;
    }

    public void SetCurrentNode(Node<Vector2Int> node)
    {
        currentNode = node;
    }
    
    public void SetStartNode(Node<Vector2Int> node)
    {
        startNode = node;
    }
    public Node<Vector2Int> GetStartNode()
    {
        return startNode;
    }
    
    public void SetDestinationNode(Node<Vector2Int> node)
    {
        destinationNode = node;
    }
    public Node<Vector2Int> GetDestinationNode()
    {
        return destinationNode;
    }

    public void SetPathfinder(AStarPathfinder<Node<Vector2Int>> pathfinder)
    {
        Pathfinder = pathfinder;
    }

    public float GetDistanceBetweenNodes()
    {
        return distanceBetweenNodes;
    }

    public GoldMineNode<Vector2Int> GetClosestGoldMineNode(Node<Vector2Int> startNode)
    {
        return goldMineManager.FindClosestGoldMine(startNode);
    }

    public void SetPath(List<Node<Vector2Int>> path)
    {
        this.path = path;
       // graphView.pathNodes = path;
    }

    public AStarPathfinder<Node<Vector2Int>> GetAStarPathfinder()
    {
        return Pathfinder;
    }

    public bool IsAtMine(Node<Vector2Int> mine)
    {
        return transform.position.x == mine.GetCoordinate().x && transform.position.y == mine.GetCoordinate().y;
    }

    public bool IsAtUrbanCenter()
    {
        return transform.position.x == urbanCenter.GetCoordinate().x && transform.position.y == urbanCenter.GetCoordinate().y;
    }

    public void SetCurrentMine(Node<Vector2Int> mine)
    {
        currentMine = mine;
    }
    
    public int GetEnergy()
    {
        return energy;
    }
    
    public void SetEnergy(int energy)
    {
        this.energy = energy;
    }
    
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    
    public void SetMaxEnergy(int maxEnergy)
    {
        this.maxEnergy = maxEnergy;
    }
    
    public void ResetEnergy()
    {
        energy = maxEnergy;
    }
}