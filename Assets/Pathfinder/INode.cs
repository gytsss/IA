public interface INode
{
    public bool EqualsTo(INode other);
    public bool IsBloqued();
}

public interface INode<Coordinate> 
{
    public void SetCoordinate(Coordinate coordinateType);
    public Coordinate GetCoordinate();
}
