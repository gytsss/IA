public class Node<Coordinate> : INode, INode<Coordinate>
{
    private bool isBlocked = false;
    private bool isGoldMine = false;
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
        return coordinate.Equals(((Node<Coordinate>)other).coordinate);
    }

    public bool IsBlocked()
    {
        return isBlocked;
    }
    
    public bool IsGoldMine()
    {
        return isGoldMine;
    }
    public void SetIsGoldMine(bool isGoldMine)
    {
        this.isGoldMine = isGoldMine;
    }
    
}