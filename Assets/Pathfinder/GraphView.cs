using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    public Vector2IntGraph<Node<Vector2Int>> Graph;
    public Node<Vector2Int> startNode;
    public Node<Vector2Int> destinationNode;
    public List<Node<Vector2Int>> pathNodes = new List<Node<Vector2Int>>();

    void Awake()
    {
        Graph = new Vector2IntGraph<Node<Vector2Int>>(10, 10);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in Graph.nodes)
        {
            if (node.EqualsTo(startNode))
                Gizmos.color = Color.black;
            else if (node.EqualsTo(destinationNode))
                Gizmos.color = Color.cyan;
            else if(pathNodes.Contains(node))
                Gizmos.color = Color.magenta;

            else if (node.IsBlocked())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
        }
    }
}