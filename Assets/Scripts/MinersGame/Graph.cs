using System;
using System.Collections.Generic;

public abstract class Graph<TNodeType, TCoordinateNode, TCoordinateType>
    where TNodeType : INode<TCoordinateType>, new()
    where TCoordinateNode : ICoordinate<TCoordinateType>, new()
    where TCoordinateType : IEquatable<TCoordinateType>, new()
{
    public static List<Node<TCoordinateType>> mines = new();

    public static TCoordinateNode MapDimensions;
    public static TCoordinateNode OriginPosition;
    public static float Distance;
    public TCoordinateNode[,] CoordNodes;
    public readonly List<TNodeType> NodesType = new();
    
    public Graph(int x, int y, float distance)
    {
        Distance = distance;
        GenerateGraph(x, y, distance);
        GetNeighbors(distance);
    }

    public abstract void CreateGraph(int x, int y, float distance);
    public abstract void GenerateGraph(int x, int y, float distance);
    public abstract void GetNeighbors(float distance);
}