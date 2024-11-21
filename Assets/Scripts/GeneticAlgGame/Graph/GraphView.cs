// using System.Collections.Generic;
// using Pathfinder;
// using Pathfinder.Graph;
// using UnityEditor;
// using UnityEngine;
// using Voronoi;
//
// public class GraphView : MonoBehaviour
// {
//     public GameManager GameManager;
//     public Vector2Graph Graph;
//     public List<Node<Vector2>> goldMines = new List<Node<Vector2>>();
//     //public Voronoi<NodeVoronoi, Vec2Int> voronoi;
//
//     public Vec2Int size = new Vec2Int(0, 0);
//     public bool isActive = false;
//
//     public Sprite goldMineSprite;
//     public Sprite blockedNodeSprite;
//     public Sprite defaultNodeSprite;
//     public Sprite urbanCenterNodeSprite;
//     
//     public GameObject spawnPrefab;
//
//     private void OnDrawGizmos()
//     {
//         if (isActive)
//         {
//             if (!Application.isPlaying)
//                 return;
//
//             
//             foreach (Node<Vector2> node in Graph.nodes)
//             {
//                 GameObject nodeObject = new GameObject("Node");
//                 nodeObject.transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y, 1f);
//                 SpriteRenderer renderer = nodeObject.AddComponent<SpriteRenderer>();
//                 renderer.sprite = defaultNodeSprite;
//             
//                 if (goldMines.Contains(node))
//                 {
//                     renderer.sprite = goldMineSprite;
//                 }
//                 else if (node.IsBlocked())
//                 {
//                     renderer.sprite = blockedNodeSprite;
//                 }
//             
//                 if (node.GetCoordinate().x == GameManager.GetUrbanCenterNode().GetCoordinate().x &&
//                     node.GetCoordinate().y == GameManager.GetUrbanCenterNode().GetCoordinate().y)
//                 {
//                     renderer.sprite = urbanCenterNodeSprite;
//                 }
//             
//                 nodeObject.transform.localScale = new Vector3(
//                     nodeObject.transform.localScale.x * GameManager.GetDistanceBetweenNodes(),
//                     nodeObject.transform.localScale.y * GameManager.GetDistanceBetweenNodes(),
//                     nodeObject.transform.localScale.z * GameManager.GetDistanceBetweenNodes());
//             }
//
//             foreach (var sector in GameManager.voronoi.SectorsToDraw())
//             {
//                 Color color = new Color(1,0,0,0.1f);
//                 Handles.color = color;
//                 
//                 List<Vector3> list = new List<Vector3>();
//                 foreach (var nodeVoronoi in sector.PointsToDraw())
//                 {
// //                    Debug.Log($"Point: {nodeVoronoi.GetX()}, {nodeVoronoi.GetY()}");
//                     list.Add(new Vector3(nodeVoronoi.GetX(), nodeVoronoi.GetY()));
//                     Gizmos.DrawSphere(new Vector3(nodeVoronoi.GetX(), nodeVoronoi.GetY()), 0.1f);
//                 }
//
//                 Handles.DrawAAConvexPolygon(list.ToArray());
//                 Handles.color = Color.black;
//                 Handles.DrawPolyLine(list.ToArray());
//             }
//         }
//     }
//
//     public void CreateGraph(int x, int y, float distance)
//     {
//         Graph = new Vector2Graph(x, y, distance);
//         size.x = x;
//         size.y = y;
//
//         isActive = true;
//     }
// }