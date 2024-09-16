
    using System.Collections.Generic;
    using DefaultNamespace;
    using UnityEngine;

    public class BaseAgent : MonoBehaviour
    {
        private AStarPathfinder<Node<Vec2Int>> Pathfinder;
        //private DijkstraPathfinder<Node<Vec2Int>> Pathfinder;
        //private DepthFirstPathfinder<Node<Vec2Int>> Pathfinder;
        //private BreadthPathfinder<Node<Vec2Int>> Pathfinder;

        private GoldMineNode<Vec2Int> currentMine;
        private UrbanCenterNode<Vec2Int> urbanCenter;

        private FSM<MinerStates, MinerFlags> fsm;
        private Node<Vec2Int> startNode;
        private Node<Vec2Int> destinationNode;

        private Node<Vec2Int> currentNode;
        private List<Node<Vec2Int>> path;
        private float distanceBetweenNodes = 0;


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

        // public GoldMineNode<Vec2Int> GetClosestGoldMineNode(Node<Vec2Int> startNode)
        // {
        //     return goldMineManager.FindClosestGoldMine(startNode);
        // }

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
