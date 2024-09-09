public enum NodeTypes
{
    Default,
    UrbanCenter,
    GoldMine
}

public class Node<Coordinate> : INode, INode<Coordinate>
{
    private bool isBlocked = false;
    private NodeTypes nodeType;
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

    public NodeTypes GetNodeType()
    {
        return nodeType;
    }

    public void SetNodeType(NodeTypes nodeType)
    {
        this.nodeType = nodeType;
    }
}

public class GoldMineNode<Coordinate> : Node<Coordinate>
{
    private int goldAmount;
    private int foodAmount;
    private int maxFoodAmount = 5;

    public GoldMineNode(Coordinate coordinate, int initialGoldAmount, int initialFoodAmount)
    {
        SetCoordinate(coordinate);
        goldAmount = initialGoldAmount;
        foodAmount = initialFoodAmount;
        SetNodeType(NodeTypes.GoldMine);
    }

    public GoldMineNode()
    {
       //default constructor
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }
    
    public void SetGoldAmount(int goldAmount)
    {
        this.goldAmount = goldAmount;
    }
    
    public int GetFoodAmount()
    {
        return foodAmount;
    }
    
    public void SetFoodAmount(int foodAmount)
    {
        this.foodAmount = foodAmount;
    }
    
    public int GetMaxFoodAmount()
    {
        return maxFoodAmount;
    }
    
    public bool HasFood()
    {
        return foodAmount > 0;
    }
    
    public void ConsumeFood()
    {
        foodAmount--;
        if (foodAmount < 0) foodAmount = 0; 
    }
    
    public bool HasGold()
    {
        return goldAmount > 0;
    }
    
    public void MineGold()
    {
        goldAmount--;
        if (goldAmount < 0) goldAmount = 0; 
    }
    
}