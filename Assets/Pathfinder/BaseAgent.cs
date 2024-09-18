    
    using System;
    using System.Collections.Generic;
    using DefaultNamespace;
    using Pathfinder;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public abstract class BaseAgent<TState, TFlag> : MonoBehaviour 
    where TState : Enum
    where TFlag : Enum
    {
    
        public GameManager gameManager;
        public GraphView graphView;

        protected AStarPathfinder<Node<Vec2Int>> Pathfinder;
        protected GoldMineNode<Vec2Int> currentMine;

        protected FSM<TState, TFlag> fsm;
        protected Node<Vec2Int> startNode;
        protected Node<Vec2Int> destinationNode;
    
        protected Node<Vec2Int> currentNode;
        protected List<Node<Vec2Int>> path;
        protected float distanceBetweenNodes = 0;
    
        public float travelTime = 0.70f;
        
    
        protected virtual void Start()
        {
            fsm = new FSM<TState, TFlag>();
        }
        
        public void InitAgent()
        {
            distanceBetweenNodes = gameManager.GetDistanceBetweenNodes();
    
            Pathfinder = new AStarPathfinder<Node<Vec2Int>>(graphView.Graph, distanceBetweenNodes);
            
            currentNode = gameManager.GetUrbanCenterNode();
            startNode = gameManager.GetUrbanCenterNode();
            destinationNode = gameManager.GetGoldMineManager().FindClosestGoldMine(startNode);
    
            transform.position = new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y);
    
            path = Pathfinder.FindPath(startNode, destinationNode, distanceBetweenNodes);
            
            AddStates();
            AddTransitions();
        }

        protected abstract void AddStates();
        
        public abstract void AddTransitions();

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
            return currentMine;
        }
    
    
        private void Update()
        {
            fsm.Tick();
        }
    }
    
       
    
    
