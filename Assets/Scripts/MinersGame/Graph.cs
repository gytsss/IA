using System;
using System.Collections.Generic;

public abstract class Graph<NodeType, CoordinateType>
    where NodeType : INode<CoordinateType>, INode, new()
    where CoordinateType : IEquatable<CoordinateType>

{
    public List<NodeType> nodes = new List<NodeType>();

    public Graph(int x, int y, float distance)
    {
        GenerateGraph(x, y, distance);
        GetNeighbors(distance);
    }

    public abstract void GenerateGraph(int x, int y, float distance);
    public abstract void GetNeighbors(float distance);
}