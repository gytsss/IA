using System;
using System.Collections.Generic;
using UnityEngine;


struct Transition<NodeType>
{
    public NodeType to;
    public int cost;
    public float distance;
}

public class AStarPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode<Vector2Int>, INode, new()
{
    private Dictionary<NodeType, List<Transition<NodeType>>> transitions =
        new Dictionary<NodeType, List<Transition<NodeType>>>();


    public AStarPathfinder(Vector2IntGraph<NodeType> graph)
    {
        this.graph = graph;

        graph.nodes.ForEach(node =>
        {
            List<Transition<NodeType>> transitionsList = new List<Transition<NodeType>>();

            List<NodeType> neighbors = GetNeighbors(node) as List<NodeType>;
            
            neighbors?.ForEach(neightbor =>
            {
                transitionsList.Add(new Transition<NodeType>
                {
                    to = neightbor,
                    cost = 0,
                    distance = Distance(node, neightbor)
                });
            });
            
            transitions.Add(node, transitionsList);
        });
    }

    public AStarPathfinder(int x, int y)
    {
        graph = new Vector2IntGraph<NodeType>(x, y);
    }

    protected override int Distance(NodeType A, NodeType B)
    {
        int distance = 0;

        distance += Mathf.Abs(A.GetCoordinate().x - B.GetCoordinate().x);
        distance += Mathf.Abs(A.GetCoordinate().y - B.GetCoordinate().y);

        return distance;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        List<NodeType> neighbors = new List<NodeType>();
        var nodeCoordinate = node.GetCoordinate();
        
        graph.nodes.ForEach(neighbor =>
        {
            var neighborCoordinate = neighbor.GetCoordinate();
            if ((neighborCoordinate.x == nodeCoordinate.x && Math.Abs(neighborCoordinate.y - nodeCoordinate.y) == 1) ||
                (neighborCoordinate.y == nodeCoordinate.y && Math.Abs(neighborCoordinate.x - nodeCoordinate.x) == 1 ||
                 (Math.Abs(neighborCoordinate.x - nodeCoordinate.x) == 1 && Math.Abs(neighborCoordinate.y - nodeCoordinate.y) == 1)))
            {
                neighbors.Add(neighbor);
            }
        });
        
        return neighbors;
    }

    protected override bool IsBlocked(NodeType node)
    {
        return node.IsBlocked();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B)
    {
        if(!GetNeighbors(A).Contains(B))
            throw new InvalidOperationException("B node has to be a neighbor of A node");

        int cost = 0;
        
        transitions.TryGetValue(A, out List<Transition<NodeType>> transition);
        
        transition?.ForEach(t =>
        {
            if (t.to.EqualsTo(B))
            {
                cost = t.cost;
            }
        });
        return cost;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.EqualsTo(B);
    }
}