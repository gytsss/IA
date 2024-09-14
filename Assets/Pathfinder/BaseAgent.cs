
    using System.Collections.Generic;
    using DefaultNamespace;
    using UnityEngine;

    public class BaseAgent : MonoBehaviour
    {
        private AStarPathfinder<Node<Vector2Int>> Pathfinder;
        //private DijkstraPathfinder<Node<Vector2Int>> Pathfinder;
        //private DepthFirstPathfinder<Node<Vector2Int>> Pathfinder;
        //private BreadthPathfinder<Node<Vector2Int>> Pathfinder;

        private GoldMineNode<Vector2Int> currentMine;
        private UrbanCenterNode<Vector2Int> urbanCenter;

        private FSM<MinerStates, MinerFlags> fsm;
        private Node<Vector2Int> startNode;
        private Node<Vector2Int> destinationNode;

        private Node<Vector2Int> currentNode;
        private List<Node<Vector2Int>> path;
        private float distanceBetweenNodes = 0;


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

        // public GoldMineNode<Vector2Int> GetClosestGoldMineNode(Node<Vector2Int> startNode)
        // {
        //     return goldMineManager.FindClosestGoldMine(startNode);
        // }

        public void SetPath(List<Node<Vector2Int>> path)
        {
            this.path = path;
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
            return transform.position.x == urbanCenter.GetCoordinate().x &&
                   transform.position.y == urbanCenter.GetCoordinate().y;
        }

        public void SetCurrentMine(GoldMineNode<Vector2Int> mine)
        {
            currentMine = mine;
        }

        public GoldMineNode<Vector2Int> GetCurrentMine()
        {
            return currentMine;
        }

       

    }
