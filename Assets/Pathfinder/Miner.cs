using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Pathfinder;
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
    OnFoodAvailable,
    OnFullInventory,
    OnMineEmpty,
    OnMineEmptyOfFood,
    OnGoldDeposit,
    OnNoMoreMines,
    OnAlarmTrigger
}


public class Miner : MonoBehaviour
{
    public GraphView graphView;

    private AStarPathfinder<Node<Vec2Int>> Pathfinder;
    //private DijkstraPathfinder<Node<Vec2Int>> Pathfinder;
    //private DepthFirstPathfinder<Node<Vec2Int>> Pathfinder;
    //private BreadthPathfinder<Node<Vec2Int>> Pathfinder;

    private GoldMineNode<Vec2Int> currentMine;

    private FSM<MinerStates, MinerFlags> fsm;
    private Node<Vec2Int> startNode;
    private Node<Vec2Int> destinationNode;

    private Node<Vec2Int> currentNode;
    private List<Node<Vec2Int>> path;

    public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;
    public TMP_Text urbanCenterText, currentGoldText, currentEnergyText;
    
    public GameManager gameManager;
    
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
        return new object[] { this as Miner, this.transform, travelTime, gameManager.GetDistanceBetweenNodes() };
    }

    private object[] MineGoldTickParameters()
    {
        return new object[] { this, goldExtractionSpeed, maxGold };
    }

    private object[] EatFoodTickParameters()
    {
        return new object[] { this };
    }

    private object[] WaitForFoodTickParameters()
    {
        return new object[] { this };
    }

    private object[] DepositGoldTickParameters()
    {
        return new object[] { this, currentNode, gameManager.GetUrbanCenterNode(), travelTime, gameManager.GetDistanceBetweenNodes() };
    }

    private object[] ReturnToUrbanCenterTickParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode() };
    }

    private object[] RespondToAlarmTickParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode() };
    }

    private void Update()
    {
        fsm.Tick();
    }

    public void GetMapInputValues()
    {
        start = true;
        
    }

    public void InitTraveler()
    {
        Pathfinder = new AStarPathfinder<Node<Vec2Int>>(graphView.Graph, gameManager.GetDistanceBetweenNodes());
        //Pathfinder = new DijkstraPathfinder<Node<Vec2Int>>(graphView.Graph);
        //Pathfinder = new DepthFirstPathfinder<Node<Vec2Int>>(graphView.Graph);
        //Pathfinder = new BreadthPathfinder<Node<Vec2Int>>(graphView.Graph);

        currentNode = gameManager.GetUrbanCenterNode();
        startNode = gameManager.GetUrbanCenterNode();
        destinationNode = gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);
        
        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);
        
        path = Pathfinder.FindPath(startNode, destinationNode, gameManager.GetDistanceBetweenNodes());
        
        
        fsm.AddBehaviour<MoveToMineState>(MinerStates.MoveToMine, onTickParameters: MoveToMineTickParameters);
        fsm.AddBehaviour<MineGoldState>(MinerStates.MineGold, onTickParameters: MineGoldTickParameters);
        fsm.AddBehaviour<EatFoodState>(MinerStates.EatFood, onTickParameters: EatFoodTickParameters);
        fsm.AddBehaviour<DepositGoldState>(MinerStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        fsm.AddBehaviour<WaitFoodState>(MinerStates.WaitFood, onTickParameters: WaitForFoodTickParameters);
        // fsm.AddBehaviour<ReturnHomeState>(MinerStates.ReturnHome, onTickParameters: ReturnToUrbanCenterTickParameters);
        // fsm.AddBehaviour<AlarmState>(MinerStates.Alarm, onTickParameters: RespondToAlarmTickParameters);
        
         

        fsm.SetTransition(MinerStates.MoveToMine, MinerFlags.OnMineFind, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFoodNeed, MinerStates.EatFood);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFullInventory, MinerStates.DepositGold);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnMineEmpty, MinerStates.MoveToMine);
        fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.DepositGold, MinerFlags.OnNoMoreMines, MinerStates.Idle);
        fsm.SetTransition(MinerStates.DepositGold, MinerFlags.OnGoldDeposit, MinerStates.MoveToMine);
        fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnMineEmptyOfFood, MinerStates.WaitFood);
        fsm.SetTransition(MinerStates.WaitFood, MinerFlags.OnFoodAvailable, MinerStates.EatFood);
        // fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnAlarmTrigger, MinerStates.ReturnHome);
        // fsm.SetTransition(MinerStates.ReturnHome, MinerFlags.OnAlarmTrigger, MinerStates.MoveToMine);

    }

    public Node<Vec2Int> GetCurrentNode()
    {
        return currentNode;
    }

    public UrbanCenterNode<Vec2Int> GetUrbanCenterNode()
    {
        return gameManager.GetUrbanCenterNode();
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
        return gameManager.GetDistanceBetweenNodes();
    }

    public GoldMineNode<Vec2Int> GetClosestGoldMineNode(Node<Vec2Int> startNode)
    {
        return gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);
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
        return transform.position.x == gameManager.GetUrbanCenterNode().GetCoordinate().x && transform.position.y == gameManager.GetUrbanCenterNode().GetCoordinate().y;
    }

    public void SetCurrentMine(GoldMineNode<Vec2Int> mine)
    {
        currentMine = mine;
    }

    public GoldMineNode<Vec2Int> GetCurrentMine()
    {
        return currentMine;
    }

    public int GetEnergy()
    {
        return energy;
    }
    
    public int GetGoldCollected()
    {
        return goldCollected;
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

    public Vector2 GetMapSize()
    {
        return new Vector2(float.Parse(widthInputField.text), float.Parse(heightInputField.text));
    }
    
    public int GetMineCount()
    {
        return int.Parse(goldMinesInputField.text);
    }

    public void SetStart(bool start)
    {
        this.start = start;
    }
}

public static class MinerEvents
{
    public static Action<Miner> OnNeedFood;
}