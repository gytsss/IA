using System;
using System.Collections.Generic;
using UnityEngine;


struct Transition<NodeType>
{
    public NodeType to;
    public int cost;
    public float distance;
    public NodeTypes nodeType;
}

public class AStarPathfinder<NodeType> : Pathfinder<NodeType> 
    where NodeType : INode<Vec2Int>, INode, new()
{
    private Dictionary<NodeType, List<Transition<NodeType>>> transitions =
        new Dictionary<NodeType, List<Transition<NodeType>>>();
    
    private Dictionary<NodeTypes, int> agentNodeCosts;


    public AStarPathfinder(Graph<NodeType> graph, float distanceBetweenNodes, Dictionary<NodeTypes, int> agentNodeCosts)
    {
        this.graph = graph;
        this.agentNodeCosts = agentNodeCosts;

        graph.nodes.ForEach(node =>
        {
            List<Transition<NodeType>> transitionsList = new List<Transition<NodeType>>();
            List<NodeType> neighbors = GetNeighbors(node, distanceBetweenNodes) as List<NodeType>;

            neighbors?.ForEach(neighbor =>
            {
                transitionsList.Add(new Transition<NodeType>
                {
                    to = neighbor,
                    distance = Distance(node, neighbor),
                    cost = CalculateTransitionCost(node, neighbor),
                    nodeType = (node as Node<Vec2Int>).GetNodeType()
                });
            });

            transitions.Add(node, transitionsList);
        });
    }

    public AStarPathfinder(int x, int y, float distance)
    {
        graph = new Graph<NodeType>(x, y, distance);
    }

    private int CalculateTransitionCost(NodeType fromNode, NodeType toNode)
    {
        NodeTypes nodeType = (toNode as Node<Vec2Int>).GetNodeType();
        int cost = agentNodeCosts.ContainsKey(nodeType) ? agentNodeCosts[nodeType] : 1;

        // Log the cost and node type
       Debug.Log($"Moving to node of type: {nodeType}, cost: {cost}");
        return cost;
    }
    
    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        throw new NotImplementedException();
    }

    protected override int Distance(NodeType A, NodeType B)
    {
        int distance = 0;

        distance += Math.Abs(A.GetCoordinate().x - B.GetCoordinate().x);
        distance += Math.Abs(A.GetCoordinate().y - B.GetCoordinate().y);

        return distance;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node, float distance)
    {
        List<NodeType> neighbors = new List<NodeType>();
        var nodeCoordinate = node.GetCoordinate();

        graph.nodes.ForEach(neighbor =>
        {
            var neighborCoordinate = neighbor.GetCoordinate();
            if ((neighborCoordinate.x == nodeCoordinate.x &&
                 Math.Abs(neighborCoordinate.y - nodeCoordinate.y) <= distance) ||
                (neighborCoordinate.y == nodeCoordinate.y &&
                 Math.Abs(neighborCoordinate.x - nodeCoordinate.x) <= distance) ||
                (Math.Abs(neighborCoordinate.x - nodeCoordinate.x) <= distance &&
                 Math.Abs(neighborCoordinate.y - nodeCoordinate.y) <= distance))
            {
                neighbors.Add(neighbor);
            }
        });

        return neighbors;
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B)
    {
        throw new NotImplementedException();
    }

    protected override bool IsBlocked(NodeType node)
    {
        return node.IsBlocked();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B, float distanceBetweenNodes)
    {
        if (!GetNeighbors(A, distanceBetweenNodes).Contains(B))
            throw new InvalidOperationException("B node has to be a neighbor of A node");
        
        
        int baseCost = 0;

        transitions.TryGetValue(A, out List<Transition<NodeType>> transition);

        transition?.ForEach(t =>
        {
            if (t.to.EqualsTo(B))
            {
                baseCost = t.cost;
                Debug.Log($"Moving from node {A.GetCoordinate()} to node {B.GetCoordinate()}, cost: {baseCost}, type: {t.nodeType}");

            }
        });
        return baseCost;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.EqualsTo(B);
    }
}