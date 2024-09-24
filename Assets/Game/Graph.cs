using System.Collections.Generic;

public class Graph<NodeType> 
    where NodeType : INode<Vec2Int>, INode, new()
{ 
    public List<NodeType> nodes = new List<NodeType>();

    public Graph(int x, int y, float distance) 
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new Vec2Int((int)(i * distance), (int)(j * distance)));
                nodes.Add(node);
            }
        }
    }

}
