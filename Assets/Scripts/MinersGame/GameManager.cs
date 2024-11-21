// using System;
// using System.Collections.Generic;
// using GeneticAlgGame.Graph;
// using TMPro;
// using UnityEngine;
// using Voronoi;
// using Random = UnityEngine.Random;
//
// namespace Pathfinder
// {
//     public class GameManager : MonoBehaviour
//     {
//         public GraphView graphView;
//
//         //public Miner miner;
//         //public Miner miner1;
//         // public Caravan caravan;
//         public List<BaseAgent> agents;
//         public GoldMineManager goldMineManager;
//
//         public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;
//         public TMP_Text urbanCenterText;
//
//         private UrbanCenterNode<Vector2> urbanCenter;
//         public Voronoi<NodeVoronoi, Vector2> voronoi;
//
//         private bool alarm = false;
//         private bool disableAlarm = false;
//         private float distanceBetweenNodes;
//         private int height;
//         private int width;
//
//
//         private void OnEnable()
//         {
//             // agents = new List<BaseAgent>();
//         }
//
//         public void GetMapInputValues()
//         {
//             height = int.Parse(heightInputField.text);
//             width = int.Parse(widthInputField.text);
//             string goldMines = goldMinesInputField.text;
//             distanceBetweenNodes = float.Parse(distanceBetweenNodesInputField.text.Replace(',', '.'));
//
//             Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines + " Distance: " +
//                       distanceBetweenNodes);
//
//             graphView.CreateGraph(height, width, distanceBetweenNodes);
//             goldMineManager.SetGoldMines(int.Parse(goldMines), distanceBetweenNodes);
//
//             InitGame();
//         }
//
//         private void InitGame()
//         {
//             urbanCenter = new UrbanCenterNode<Vector2>();
//             int randomNode = Random.Range(0, graphView.Graph.nodes.Count);
//             urbanCenter.SetCoordinate(graphView.Graph.nodes[randomNode].GetCoordinate());
//             //urbanCenter.SetCoordinate(new Vector2(0,0));
//             urbanCenter.SetNeighbors(graphView.Graph.GetNeighborsNode(urbanCenter.GetCoordinate()));
//             //graphView.Graph.nodes.Remove(graphView.Graph.nodes[0]);
//             graphView.Graph.nodes.Remove(graphView.Graph.nodes[randomNode]);
//             graphView.Graph.nodes.Add(urbanCenter);
//             urbanCenterText.text = "Urban Center gold: " + urbanCenter.GetGold();
//             voronoi = new Voronoi<NodeVoronoi, Vector2>();
//             voronoi.Init(new Vector2(height,width), distanceBetweenNodes,new Vector2(-0.5f,-0.5f) );
//             voronoi.SetVoronoi(goldMineManager.goldMinesVoronois, GetNodeVoronoiMapSize());
//             
//             foreach (BaseAgent agent in agents)
//             {
//                 agent.InitAgent();
//                 agent.SetStart(true);
//             }
//         }
//
//         public Vec2Int GetMapSize()
//         {
//             return new Vec2Int(height, width);
//         }
//         public NodeVoronoi GetNodeVoronoiMapSize()
//         {
//             return new NodeVoronoi(height, width);
//         }
//
//         public int GetMineCount()
//         {
//             return int.Parse(goldMinesInputField.text);
//         }
//
//         public Node<Vector2> GetUrbanCenterNode()
//         {
//             return urbanCenter;
//         }
//
//         public void ActivateAlarm()
//         {
//             alarm = true;
//             disableAlarm = false;
//         }
//
//         public void DisableAlarm()
//         {
//             alarm = false;
//
//             foreach (BaseAgent agent in agents)
//             {
//                 agent.SetStart(true);
//             }
//         }
//
//         public bool GetDisableAlarm()
//         {
//             return disableAlarm;
//         }
//
//         public bool GetAlarm()
//         {
//             return alarm;
//         }
//
//         public float GetDistanceBetweenNodes()
//         {
//             return distanceBetweenNodes;
//         }
//
//         public GoldMineManager GetGoldMineManager()
//         {
//             return goldMineManager;
//         }
//
//         public void UpdateUrbanCenterGoldText()
//         {
//             urbanCenterText.text = "Urban Center gold: " + urbanCenter.GetGold();
//         }
//     }
// }