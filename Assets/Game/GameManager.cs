using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinder
{
    public class GameManager : MonoBehaviour
    {
        public GraphView graphView;
        //public Miner miner;
        //public Miner miner1;
       // public Caravan caravan;
        public List<BaseAgent> agents;
        public GoldMineManager goldMineManager;
    
        public TMP_InputField heightInputField, widthInputField, goldMinesInputField, distanceBetweenNodesInputField;
        public TMP_Text urbanCenterText;

        private UrbanCenterNode<Vec2Int> urbanCenter;

        private bool alarm = false;
        private bool disableAlarm = false;
        private float distanceBetweenNodes;

        private void OnEnable()
        {
           // agents = new List<BaseAgent>();
        }

        public void GetMapInputValues()
        {
            string height = heightInputField.text;
            string width = widthInputField.text;
            string goldMines = goldMinesInputField.text;
            distanceBetweenNodes = float.Parse(distanceBetweenNodesInputField.text.Replace(',', '.'));

            Debug.Log("Height: " + height + " Width: " + width + " GoldMines: " + goldMines + " Distance: " +
                      distanceBetweenNodes);
        
           graphView.CreateGraph(int.Parse(height), int.Parse(width), distanceBetweenNodes);
            goldMineManager.SetGoldMines(int.Parse(goldMines), distanceBetweenNodes);

            InitGame();
        }

        private void InitGame()
        {
            urbanCenter = new UrbanCenterNode<Vec2Int>();
            urbanCenter.SetCoordinate(new Vec2Int(Random.Range(0, graphView.size.x), Random.Range(0, graphView.size.y)));
            urbanCenterText.text = "Urban Center gold: " + urbanCenter.GetGold();
            

            foreach (BaseAgent agent in agents)
            {
                agent.InitAgent();
                agent.SetStart(true);
            }
            
        }

        private void Update()
        {
            // currentGoldText.text = "Miner Gold: " + agents..GetGoldCollected();
            // currentEnergyText.text = "Miner Energy: " + miner.GetEnergy();
        }
    
        public Vector2 GetMapSize()
        {
            return new Vector2(float.Parse(widthInputField.text), float.Parse(heightInputField.text));
        }
    
        public int GetMineCount()
        {
            return int.Parse(goldMinesInputField.text);
        }
        
        public UrbanCenterNode<Vec2Int> GetUrbanCenterNode()
        {
            return urbanCenter;
        }
        
        public void ActivateAlarm()
        {
            alarm = true;
            disableAlarm = false;
        }
        
        public void DisableAlarm()
        {
            alarm = false;

            foreach (BaseAgent agent in agents)
            {
                agent.SetStart(true);
            }
            
        }

        public bool GetDisableAlarm()
        {
            return disableAlarm;
        }
        
        public bool GetAlarm()
        {
            return alarm;
        }

        public float GetDistanceBetweenNodes()
        {
            return distanceBetweenNodes;
        }
        
        public GoldMineManager GetGoldMineManager()
        {
            return goldMineManager;
        }

        public void UpdateUrbanCenterGoldText()
        {
            urbanCenterText.text = "Urban Center gold: " + urbanCenter.GetGold();

        }
    }
}
