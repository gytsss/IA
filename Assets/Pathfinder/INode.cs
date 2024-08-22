public interface INode
{
    public bool IsBloqued();
}

public interface INode<Coordinate> 
{
    public void SetCoordinate(Coordinate coordinateType);
    public Coordinate GetCoordinate();
}
