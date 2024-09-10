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
    private int maxGoldAmount = 15;

    public GoldMineNode(Coordinate coordinate, int initialGoldAmount, int initialFoodAmount)
    {
        SetCoordinate(coordinate);
        goldAmount = initialGoldAmount;
        foodAmount = initialFoodAmount;
        SetNodeType(NodeTypes.GoldMine);
    }

    public GoldMineNode()
    {
        foodAmount = maxFoodAmount;
        goldAmount = maxGoldAmount;
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

    public void ConsumeFood(int amount = 1)
    {
        foodAmount -= amount;
        if (foodAmount < 0) foodAmount = 0;
    }

    public bool HasGold()
    {
        return goldAmount > 0;
    }

    public void MineGold(int amount = 1)
    {
        goldAmount -= amount;
        if (goldAmount < 0) goldAmount = 0;
    }
}

public class UrbanCenterNode<Coordinate> : Node<Coordinate>
{
    private int gold = 0;
    private int agentCapacity = 10;

    public UrbanCenterNode(Coordinate coordinate)
    {
        SetCoordinate(coordinate);
        gold = 0;
        SetNodeType(NodeTypes.UrbanCenter);
    }

    public UrbanCenterNode()
    {
        gold = 0;
    }

    public int GetGold()
    {
        return gold;
    }

    public void SetGold(int amount)
    {
        gold = amount;
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }
    
    public int GetAgentCapacity()
    {
        return agentCapacity;
    }
    
    public bool CanGenerateAgent()
    {
        return agentCapacity > 0;
    }
    
    public void GenerateAgent()
    {
        if (CanGenerateAgent())
        {
            agentCapacity--;
            // Logic for creating a new miner/caravan.
        }
    }
}
