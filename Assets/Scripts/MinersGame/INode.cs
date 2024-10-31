using System;
using System.Collections.Generic;

public interface INode 
{
    public bool EqualsTo(INode other);
    public bool IsBlocked();
}

public interface INode<Coordinate> 
{
    public void SetCoordinate(Coordinate coordinateType);
    public Coordinate GetCoordinate();
    
    public void AddNeighbor(INode<Coordinate> neighbor);

    public List<INode<Coordinate>> GetNeighbors();

    public List<INode<Coordinate>> PassNeighbors();

    public void QuitNeighbor(INode<Coordinate> neighborToQuit, List<INode<Coordinate>> neighborsList);
    
    public void SetNeighbors(List<INode<Coordinate>> neighbors);
}
