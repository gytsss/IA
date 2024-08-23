public class Node<Coordinate> : INode, INode<Coordinate>
{
    private bool isBloqued = false;
    private Coordinate coordinate;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool EqualsTo(INode other)
    {
        return coordinate.Equals((other as Node<Coordinate>).coordinate);
    }

    public bool IsBloqued()
    {
        return isBloqued;
    }
}