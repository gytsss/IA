using System.Collections.Generic;
using UnityEngine;


struct Transition <NodeType>
{
    public NodeType to;
    public int cost;
    public float distance;
}
public class AStarPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    private Dictionary<NodeType, Transition<NodeType>> transitions = new Dictionary<NodeType, Transition<NodeType>>();
    
    
    protected override int Distance(NodeType A, NodeType B)
    {
        int distance = 0;

        distance += Mathf.Abs((A as INode<(int x, int y)>).GetCoordinate().x - (B as INode<(int x, int y)>).GetCoordinate().x );
        distance += Mathf.Abs((A as INode<(int x, int y)>).GetCoordinate().y - (B as INode<(int x, int y)>).GetCoordinate().y );
        
        return distance;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsBlocked(NodeType node)
    {
        return node.IsBlocked();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        throw new System.NotImplementedException();
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.Equals(B);
    }
}