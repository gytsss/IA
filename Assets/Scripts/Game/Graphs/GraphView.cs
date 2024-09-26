using System.Collections.Generic;
using Game.Managers;
using Game.Nodes;
using UnityEngine;

namespace Game.Graphs
{
    public class GraphView : MonoBehaviour
    {
        public GameManager GameManager;
        public Vector2IntGraph<Node<Vec2Int>> Graph;
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
                
                    if (goldMines.Contains(node))
                    {
                        renderer.sprite = goldMineSprite;
                    
                    }
                    else if (node.IsBlocked())
                    {
                        renderer.sprite = blockedNodeSprite;
                    }
 
                    if (node.GetCoordinate().x == GameManager.GetUrbanCenterNode().GetCoordinate().x && node.GetCoordinate().y == GameManager.GetUrbanCenterNode().GetCoordinate().y)
                    {
                        renderer.sprite = urbanCenterNodeSprite;
                    }

                    nodeObject.transform.localScale = new Vector3(nodeObject.transform.localScale.x * GameManager.GetDistanceBetweenNodes(), nodeObject.transform.localScale.y * GameManager.GetDistanceBetweenNodes(), nodeObject.transform.localScale.z * GameManager.GetDistanceBetweenNodes());
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
}