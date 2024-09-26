using System;
using System.Collections.Generic;


struct Transition<NodeType>
{
    public NodeType to;
    public int cost;
    public float distance;
}

public class AStarPathfinder<NodeType, CoordinateType, TCoordinate> : Pathfinder<NodeType, CoordinateType, TCoordinate>
    where NodeType : INode<CoordinateType>, INode, new()
    where CoordinateType : IEquatable<CoordinateType>
    where TCoordinate : ICoordinate<CoordinateType>, new()
{
    private Dictionary<NodeType, List<Transition<NodeType>>> transitions =
        new Dictionary<NodeType, List<Transition<NodeType>>>();


    public AStarPathfinder(Graph<NodeType, CoordinateType> graph, float distanceBetweenNodes)
    {
        this.graph = graph;

        graph.nodes.ForEach(node =>
        {
            List<Transition<NodeType>> transitionsList = new List<Transition<NodeType>>();

            List<NodeType> neighbors = GetNeighbors(node, distanceBetweenNodes) as List<NodeType>;

            TCoordinate nodeCoordinate = new TCoordinate();
            nodeCoordinate.SetCoordinate(node.GetCoordinate());
            
            neighbors?.ForEach(neighbor =>
            {
                TCoordinate neighborCoordinate = new TCoordinate();
                neighborCoordinate.SetCoordinate(neighbor.GetCoordinate());
                
                transitionsList.Add(new Transition<NodeType>
                {
                    to = neighbor,
                    cost = 0,
                    distance = Distance(nodeCoordinate, neighborCoordinate)
                });
            });

            transitions.Add(node, transitionsList);
        });
    }

    // public AStarPathfinder(int x, int y, float distance)
    // {
    //     graph = new Graph<NodeType,CoordinateType>(x, y, distance);
    // }
    

    protected override int Distance(TCoordinate A, TCoordinate B)
    {
        float distance = 0;

        distance += Math.Abs(A.GetX() - B.GetX());
        distance += Math.Abs(A.GetY() - B.GetY());

        return (int)distance;
    }

    protected override bool IsBlocked(NodeType node)
    {
        return node.IsBlocked();
    }

    protected override ICollection<INode<CoordinateType>> GetNeighbors(NodeType node, float distance)
    {
        return node.GetNeighbors();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B, float distanceBetweenNodes)
    {
        if (!GetNeighbors(A, distanceBetweenNodes).Contains(B))
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
        if (A == null || B == null)
        {
            return false;
        }
        
        return A.EqualsTo(B);
    }
}