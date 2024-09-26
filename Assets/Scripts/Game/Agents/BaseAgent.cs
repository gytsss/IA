using System;
using System.Collections.Generic;
using DefaultNamespace;
using Pathfinder;
using UnityEngine;
using Random = UnityEngine.Random;

public enum States
{
    Idle,
    MoveToMine,
    MineGold,
    EatFood,
    DepositGold,
    WaitFood,
    Alarm,
    DepositFood,
    WaitMine,
    ReturnHome
}

public enum Flags
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
    OnAlarmTrigger,
    OnHome,
    OnBackToWork,
    OnFoodDeposit
}

public abstract class BaseAgent : MonoBehaviour
{
    public GameManager gameManager;
    public GraphView graphView;

    protected AStarPathfinder<Node<Vec2Int>> Pathfinder;
    protected GoldMineNode<Vec2Int> currentMine;

    protected FSM<States, Flags> fsm;
    protected Node<Vec2Int> startNode;
    protected Node<Vec2Int> destinationNode;

    protected Node<Vec2Int> currentNode;
    protected List<Node<Vec2Int>> path;
    protected float distanceBetweenNodes = 0;

    public float travelTime = 0.70f;
    protected bool start = false;
    protected bool isMiner = false;

    protected virtual void Start()
    {
        fsm = new FSM<States, Flags>();
    }

    public void InitAgent()
    {
        distanceBetweenNodes = gameManager.GetDistanceBetweenNodes();

        Pathfinder = new AStarPathfinder<Node<Vec2Int>>(graphView.Graph, distanceBetweenNodes);

        currentNode = gameManager.GetUrbanCenterNode();
        startNode = gameManager.GetUrbanCenterNode();
        destinationNode = gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);

        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);

        path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes,GetTransitionCost);

        AddStates();
        AddTransitions();
    }

    protected abstract void AddStates();

    public abstract void AddTransitions();

    public int GetTransitionCost(Node<Vec2Int> fromNode, Node<Vec2Int> toNode)
    {
        // Ejemplo de cómo establecer costos en función del tipo de nodo y del agente
        int cost = 0; // Valor por defecto

        // Asignar costos personalizados según el tipo de nodo y agente
        switch (toNode.GetNodeType())
        {
            case NodeTypes.Default:
                cost = isMiner ? 1 : 2; // Mineros atraviesan nodos default más fácilmente que las caravanas
                break;
            case NodeTypes.UrbanCenter:
                cost = isMiner ? 3 : 1; // Caravanas prefieren moverse a centros urbanos, costo menor
                break;
            case NodeTypes.GoldMine:
                cost = isMiner ? 2 : 4; // Mineros prefieren moverse a minas de oro, pero las caravanas evitan estas áreas
                break;
        }

        return cost;
    }


    public Node<Vec2Int> GetCurrentNode()
    {
        return currentNode;
    }

    public void SetCurrentNode(Node<Vec2Int> node)
    {
        currentNode = node;
    }

    public Node<Vec2Int> GetStartNode()
    {
        return startNode;
    }

    public void SetStartNode(Node<Vec2Int> node)
    {
        startNode = node;
    }

    public Node<Vec2Int> GetDestinationNode()
    {
        return destinationNode;
    }

    public void SetDestinationNode(Node<Vec2Int> node)
    {
        destinationNode = node;
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
        return gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);
    }

    public GoldMineNode<Vec2Int> GetClosestGoldMineNodeBeingMined(Node<Vec2Int> startNode)
    {
        return gameManager.GetGoldMineManager().FindClosestGoldMineBeingMined(startNode);
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
        return transform.position.x == gameManager.GetUrbanCenterNode().GetCoordinate().x &&
               transform.position.y == gameManager.GetUrbanCenterNode().GetCoordinate().y;
    }

    public void SetCurrentMine(GoldMineNode<Vec2Int> mine)
    {
        currentMine = mine;
    }

    public GoldMineNode<Vec2Int> GetCurrentMine()
    {
        return currentNode as GoldMineNode<Vec2Int>;
    }

    public float GetTravelTime()
    {
        return travelTime;
    }

    public void SetStart(bool start)
    {
        this.start = start;
    }

    public bool GetStart()
    {
        return start;
    }

    public void SetIsMiner(bool isMiner)
    {
        this.isMiner = isMiner;
    }

    public bool GetIsMiner()
    {
        return isMiner;
    }


    private void Update()
    {
        fsm.Tick();
    }
}