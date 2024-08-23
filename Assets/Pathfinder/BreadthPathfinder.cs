using System;
using System.Collections.Generic;
using UnityEngine;

public class BreadthPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode, INode<Vector2Int>, new()
{
    
    public BreadthPathfinder(Vector2IntGraph<NodeType> graph)
    {
        this.graph = graph;
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
        
        for (int i = graph.nodes.Count - 1; i > 0; i--)
        {
            var neighborCoor = graph.nodes[i].GetCoordinate();
            
            if ((neighborCoor.x == nodeCoordinate.x && Math.Abs(neighborCoor.y - nodeCoordinate.y) == 1) ||
                (neighborCoor.y == nodeCoordinate.y && Math.Abs(neighborCoor.x - nodeCoordinate.x) == 1))
            {
                neighbors.Add(graph.nodes[i]);
            }
        }
        
        return neighbors;
    }

    protected override bool IsBlocked(NodeType node)
    {
        return node.IsBlocked();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.EqualsTo(B);

    }
}
