    
    using System;
    using System.Collections.Generic;
    using DefaultNamespace;
    using Pathfinder;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public class BaseAgent<TState, TFlag> : MonoBehaviour 
    where TState : Enum
    where TFlag : Enum
    {
    
        public GameManager gameManager;
        public GraphView graphView;
        public GoldMineManager goldMineManager;
    
        protected AStarPathfinder<Node<Vec2Int>> Pathfinder;
        protected GoldMineNode<Vec2Int> currentMine;
        protected UrbanCenterNode<Vec2Int> urbanCenter;
    
        protected FSM<TState, TFlag> fsm;
        protected Node<Vec2Int> startNode;
        protected Node<Vec2Int> destinationNode;
    
        protected Node<Vec2Int> currentNode;
        protected List<Node<Vec2Int>> path;
        protected float distanceBetweenNodes = 0;
    
        public float travelTime = 0.70f;
    
        protected bool start = false;
    
        protected virtual void Start()
        {
            fsm = new FSM<TState, TFlag>();
            InitAgent();
        }
    
        // Método común para inicializar el agente
        protected void InitAgent()
        {
            distanceBetweenNodes = GetDistanceBetweenNodes();
    
            Pathfinder = new AStarPathfinder<Node<Vec2Int>>(graphView.Graph, distanceBetweenNodes);
    
            urbanCenter = new UrbanCenterNode<Vec2Int>();
            urbanCenter.SetCoordinate(new Vec2Int(Random.Range(0, graphView.size.x),
                Random.Range(0, graphView.size.y)));
            currentNode = urbanCenter;
            startNode = urbanCenter;
            destinationNode = GetClosestGoldMineNode(startNode);
    
            transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);
    
            path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes);
            
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
    
    
        private void Update()
        {
            fsm.Tick();
        }
    }
    
       
    
    
