using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    public Vector2IntGraph<Node<Vec2Int>> Graph;
    public Node<Vec2Int> startNode;
    public Node<Vec2Int> destinationNode;
    public UrbanCenterNode<Vec2Int> urbanCenterNode;
    public List<Node<Vec2Int>> pathNodes = new List<Node<Vec2Int>>();
    public List<Node<Vec2Int>> goldMines = new List<Node<Vec2Int>>();
    
    public Vec2Int size = new Vec2Int(0,0);
    public bool isActive = false;
    
    public Sprite goldMineSprite;
    public Sprite blockedNodeSprite;
    public Sprite defaultNodeSprite;
    public Sprite urbanCenterNodeSprite;

    private void OnDrawGizmos()
    {
        if (isActive)
        {
            if (!Application.isPlaying)
                return;
            foreach (Node<Vec2Int> node in Graph.nodes)
            {
                GameObject nodeObject = new GameObject("Node");
                nodeObject.transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y, 1f);
                SpriteRenderer renderer = nodeObject.AddComponent<SpriteRenderer>();
                renderer.sprite = defaultNodeSprite;

                if (node.EqualsTo(startNode) && !goldMines.Contains(node))
                {
                    
                }
                else if (node.EqualsTo(destinationNode) && !goldMines.Contains(node))
                {
                   
                }
                
                if (goldMines.Contains(node))
                {
                    renderer.sprite = goldMineSprite;

                    Debug.Log($"Gold mine node at: {node.GetCoordinate()}");
                }
                else if (node.IsBlocked())
                {
                    renderer.sprite = blockedNodeSprite;
                }
 
                if (node.GetCoordinate().x == urbanCenterNode.GetCoordinate().x && node.GetCoordinate().y == urbanCenterNode.GetCoordinate().y)
                {
                    renderer.sprite = urbanCenterNodeSprite;
                }

               // nodeObject.transform.localScale = new Vector3(6.3f, 6.3f, 6.3f);
            }
        }
    }

    public void CreateGraph(int x, int y, float distance)
    {
        Graph = new Vector2IntGraph<Node<Vec2Int>>(x, y, distance);
        size.x = x;
        size.y = y;

        isActive = true;
    }
}