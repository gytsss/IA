using System;
using System.Collections.Generic;

public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode, INode<Vec2Int>, new()
{
    
    public DepthFirstPathfinder(Graph<NodeType> graph)
    {
        this.graph = graph;
    }
    protected override int Distance(NodeType A, NodeType B)
    {
        int distance = 0;

        distance += Math.Abs(A.GetCoordinate().x - B.GetCoordinate().x);
        distance += Math.Abs(A.GetCoordinate().y - B.GetCoordinate().y);

        return distance;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        List<NodeType> neighbors = new List<NodeType>();
        
        var nodeCoordinate = node.GetCoordinate();

        for (int i = 0; i < graph.nodes.Count; i++)
        {
            var neighborCoordinate = graph.nodes[i].GetCoordinate();
            
            if ((neighborCoordinate.x == nodeCoordinate.x && Math.Abs(neighborCoordinate.y - nodeCoordinate.y) == 1) ||
                (neighborCoordinate.y == nodeCoordinate.y && Math.Abs(neighborCoordinate.x - nodeCoordinate.x) == 1 ))
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

    protected override ICollection<NodeType> GetNeighbors(NodeType node, float distance)
    {
        List<NodeType> neighbors = new List<NodeType>();
        var nodeCoordinate = node.GetCoordinate();
        
        graph.nodes.ForEach(neighbor =>
        {
            var neighborCoordinate = neighbor.GetCoordinate();
            if ((neighborCoordinate.x == nodeCoordinate.x && Math.Abs(neighborCoordinate.y - nodeCoordinate.y) == distance) ||
                (neighborCoordinate.y == nodeCoordinate.y && Math.Abs(neighborCoordinate.x - nodeCoordinate.x) == distance) ||
                (Math.Abs(neighborCoordinate.x - nodeCoordinate.x) == distance && Math.Abs(neighborCoordinate.y - nodeCoordinate.y) == distance))
            {
                neighbors.Add(neighbor);
            }
        });
        
        return neighbors;
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B, float distanceBetweenNodes)
    {
        return 0;
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
