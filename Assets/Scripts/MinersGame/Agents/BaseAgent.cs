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

public enum BaseAgentsFlags
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

    protected AStarPathfinder<Node<Vector2>, Vector2, NodeVoronoi> Pathfinder;
    protected GoldMineNode<Vector2> currentMine;

   // protected FSM<States, BaseAgentsFlags> fsm;
    protected Node<Vector2> startNode;
    protected Node<Vector2> destinationNode;

    protected Node<Vector2> currentNode;
    protected List<Node<Vector2>> path;
    protected float distanceBetweenNodes = 0;

    public float travelTime = 0.70f;
    protected bool start = false;
    protected bool isMiner = false;

    protected virtual void Start()
    {
        //fsm = new FSM<States, BaseAgentsFlags>();
    }

    public void InitAgent()
    {
        distanceBetweenNodes = gameManager.GetDistanceBetweenNodes();

        Pathfinder = new AStarPathfinder<Node<Vector2>, Vector2, NodeVoronoi>(graphView.Graph, distanceBetweenNodes);

        currentNode = gameManager.GetUrbanCenterNode();
        startNode = gameManager.GetUrbanCenterNode();
        NodeVoronoi startNodeVoronoi = new NodeVoronoi();
        startNodeVoronoi.SetCoordinate(startNode.GetCoordinate());
        destinationNode = gameManager.voronoi.GetMineCloser(startNodeVoronoi);

        transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);

        path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes);

        AddStates();
        AddTransitions();
    }

    protected abstract void AddStates();

    public abstract void AddTransitions();

    public int GetTransitionCost(Node<Vector2> fromNode, Node<Vector2> toNode)
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


    public Node<Vector2> GetCurrentNode()
    {
        return currentNode;
    }

    public void SetCurrentNode(Node<Vector2> node)
    {
        currentNode = node;
    }

    public Node<Vector2> GetStartNode()
    {
        return startNode;
    }

    public void SetStartNode(Node<Vector2> node)
    {
        startNode = node;
    }

    public Node<Vector2> GetDestinationNode()
    {
        return destinationNode;
    }

    public void SetDestinationNode(Node<Vector2> node)
    {
        destinationNode = node;
    }

    public void SetPathfinder(AStarPathfinder<Node<Vector2>, Vector2, NodeVoronoi> pathfinder)
    {
        Pathfinder = pathfinder;
    }

    public float GetDistanceBetweenNodes()
    {
        return distanceBetweenNodes;
    }

    public GoldMineNode<Vector2> GetClosestGoldMineNode(Node<Vector2> startNode)
    {
        return gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);
    }

    public GoldMineNode<Vector2> GetClosestGoldMineNodeBeingMined(Node<Vector2> startNode)
    {
        return gameManager.GetGoldMineManager().FindClosestGoldMineBeingMined(startNode);
    }

    public void SetPath(List<Node<Vector2>> path)
    {
        this.path = path;
    }

    public AStarPathfinder<Node<Vector2>, Vector2, NodeVoronoi> GetAStarPathfinder()
    {
        return Pathfinder;
    }

    public bool IsAtMine(Node<Vector2> mine)
    {
        return transform.position.x == mine.GetCoordinate().x && transform.position.y == mine.GetCoordinate().y;
    }

    public bool IsAtUrbanCenter()
    {
        return transform.position.x == gameManager.GetUrbanCenterNode().GetCoordinate().x &&
               transform.position.y == gameManager.GetUrbanCenterNode().GetCoordinate().y;
    }

    public void SetCurrentMine(GoldMineNode<Vector2> mine)
    {
        currentMine = mine;
    }

    public GoldMineNode<Vector2> GetCurrentMine()
    {
        return currentNode as GoldMineNode<Vector2>;
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
        //fsm.Tick();
    }
}