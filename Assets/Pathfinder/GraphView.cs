using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    public Vector2IntGraph<Node<Vector2Int>> Graph;
    public Node<Vector2Int> startNode;
    public Node<Vector2Int> destinationNode;
    public List<Node<Vector2Int>> pathNodes = new List<Node<Vector2Int>>();
    public List<Node<Vector2Int>> goldMines = new List<Node<Vector2Int>>();

    public Vector2Int size;
    public bool isActive = false;

    private void OnDrawGizmos()
    {
        if (isActive)
        {
            if (!Application.isPlaying)
                return;
            foreach (Node<Vector2Int> node in Graph.nodes)
            {
                if (node.EqualsTo(startNode))
                    Gizmos.color = Color.gray;
                else if (node.EqualsTo(destinationNode))
                    Gizmos.color = Color.cyan;
                else if (pathNodes.Contains(node))
                    Gizmos.color = Color.magenta;
                else if (goldMines.Contains(node))
                {
                    Gizmos.color = Color.white;
                    Debug.Log($"Gold mine node at: {node.GetCoordinate()}");
                }
                else if (node.IsBlocked())
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
            }
        }
    }

    public void CreateGraph(int x, int y)
    {
        Graph = new Vector2IntGraph<Node<Vector2Int>>(x, y);
        size.x = x;
        size.y = y;

        isActive = true;
    }
}