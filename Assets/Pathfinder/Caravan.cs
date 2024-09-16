using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CaravanStates
{
    Idle,
    CaravanMoveToMine,
    DepositFood,
    ReturnHome,
    Alarm
}

public enum CaravanFlags
{
    OnFoodNeed,
    OnMineFind,
    OnFoodDeposit,
    OnHome,
    OnAlarmTrigger
}


public class Caravan : MonoBehaviour
{
    public GraphView graphView;
    public GoldMineManager goldMineManager;

    private AStarPathfinder<Node<Vec2Int>> Pathfinder;
    //private DijkstraPathfinder<Node<Vec2Int>> Pathfinder;
    //private DepthFirstPathfinder<Node<Vec2Int>> Pathfinder;
    //private BreadthPathfinder<Node<Vec2Int>> Pathfinder;

    private GoldMineNode<Vec2Int> currentMine;
    private UrbanCenterNode<Vec2Int> urbanCenter;

    private FSM<CaravanStates, CaravanFlags> fsm;
    private Node<Vec2Int> startNode;
    private Node<Vec2Int> destinationNode;

    private Node<Vec2Int> currentNode;
    private List<Node<Vec2Int>> path;
    private float distanceBetweenNodes = 0;


    public float travelTime = 0.70f;
    private bool start = false;


    public Miner miner;

    private void OnEnable()
    {
        MinerEvents.OnNeedFood += OnFoodNeed;
    }
    
    private void OnDisable()
    {
        MinerEvents.OnNeedFood -= OnFoodNeed;
    }

    private void OnFoodNeed(Miner obj)
    {
        Debug.Log("Caravan moving to mine");
        fsm.AddBehaviour<CaravanMoveToMineState>(CaravanStates.CaravanMoveToMine, onTickParameters: CaravanMoveToMineTickParameters);

        fsm.ForceState(CaravanStates.CaravanMoveToMine);
    }

    private void Start()
    {
        fsm = new FSM<CaravanStates, CaravanFlags>();

        fsm.AddBehaviour<IdleState>(CaravanStates.Idle, onTickParameters: () => { return new object[] { start }; });

        fsm.SetTransition(CaravanStates.Idle, CaravanFlags.OnFoodNeed, CaravanStates.CaravanMoveToMine);

        fsm.ForceState(CaravanStates.Idle);
    }

    private object[] CaravanMoveToMineTickParameters()
    {
        return new object[]
            { this as Caravan, this.transform, destinationNode, startNode, travelTime, distanceBetweenNodes, path };
    }


    private object[] DepositFoodTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] ReturnHomeTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] AlarmTickParameters()
    {
        return new object[] { this };
    }

    private void Update()
    {
        fsm.Tick();
    }

    public void GetMapInputValues()
    {
       
        start = true;
        InitCaravan();
    }

    private void InitCaravan()
    {
        distanceBetweenNodes = miner.GetDistanceBetweenNodes();
        
        Pathfinder = new AStarPathfinder<Node<Vec2Int>>(graphView.Graph, distanceBetweenNodes);
        //Pathfinder = new DijkstraPathfinder<Node<Vec2Int>>(graphView.Graph);
        //Pathfinder = new DepthFirstPathfinder<Node<Vec2Int>>(graphView.Graph);
        //Pathfinder = new BreadthPathfinder<Node<Vec2Int>>(graphView.Graph);

        urbanCenter = miner.GetUrbanCenterNode();
        currentNode = urbanCenter;
        startNode = urbanCenter;
        destinationNode = miner.GetCurrentMine();

        
        transform.position = new Vector3(urbanCenter.GetCoordinate().x, urbanCenter.GetCoordinate().y);
        

       //fsm.AddBehaviour<CaravanMoveToMineState>(CaravanStates.CaravanMoveToMine, onTickParameters: CaravanMoveToMineTickParameters);
        //fsm.AddBehaviour<MineGoldState>(CaravanStates.DepositGold, onTickParameters: MineGoldTickParameters);
        //fsm.AddBehaviour<EatFoodState>(CaravanStates.EatFood, onTickParameters: EatFoodTickParameters);
        //fsm.AddBehaviour<DepositGoldState>(CaravanStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        


        //fsm.SetTransition(CaravanStates.CaravanMoveToMine, CaravanFlags.OnMineFind, CaravanStates.DepositFood);
        //fsm.SetTransition(CaravanStates.DepositFood, CaravanFlags.OnFoodDeposit, CaravanStates.ReturnHome);
        //fsm.SetTransition(CaravanStates.ReturnHome, CaravanFlags.OnHome, CaravanStates.Idle);
        
    }

    public Node<Vec2Int> GetCurrentNode()
    {
        return currentNode;
    }

    public void SetCurrentNode(Node<Vec2Int> node)
    {
        currentNode = node;
    }

    public void SetStartNode(Node<Vec2Int> node)
    {
        startNode = node;
    }

    public Node<Vec2Int> GetStartNode()
    {
        return startNode;
    }

    public void SetDestinationNode(Node<Vec2Int> node)
    {
        destinationNode = node;
    }

    public Node<Vec2Int> GetDestinationNode()
    {
        return destinationNode;
    }

    public void SetPathfinder(AStarPathfinder<Node<Vec2Int>> pathfinder)
    {
        Pathfinder = pathfinder;
    }

    public float GetDistanceBetweenNodes()
    {
        return distanceBetweenNodes;
    }

    public GoldMineNode<Vec2Int> GetClosestGoldMineNode(Node<Vec2Int> startNode)
    {
        return goldMineManager.FindClosestGoldMine(startNode);
    }

    public void SetPath(List<Node<Vec2Int>> path)
    {
        this.path = path;
    }

    public AStarPathfinder<Node<Vec2Int>> GetAStarPathfinder()
    {
        return Pathfinder;
    }

    public bool IsAtMine(Node<Vec2Int> mine)
    {
        return transform.position.x == mine.GetCoordinate().x && transform.position.y == mine.GetCoordinate().y;
    }

    public bool IsAtUrbanCenter()
    {
        return transform.position.x == urbanCenter.GetCoordinate().x &&
               transform.position.y == urbanCenter.GetCoordinate().y;
    }

    public void SetCurrentMine(GoldMineNode<Vec2Int> mine)
    {
        currentMine = mine;
    }

    public GoldMineNode<Vec2Int> GetCurrentMine()
    {
        return currentMine;
    }
}

    