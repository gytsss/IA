using System;
using System.Collections.Generic;
using MinersGame.FSM.States;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class DepositGoldState : State
    {
        Miner miner;
        private List<Node<Vector2>> pathToUrbanCenter;
        private GoldMineNode<Vector2> mine;
        UrbanCenterNode<Vector2> urbanCenter;
        private bool alreadyDeposited = false;
        private float timeSinceLastMove;
        private int currentNodeIndex;
        private bool isMoving;
        private bool noMoreMines = false;
        float distanceBetweenNodes;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            
            Node<Vector2> currentNode = parameters[0] as Node<Vector2>;
            float travelTime = Convert.ToSingle(parameters[1]);
           

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                mine = miner.GetClosestGoldMineNode(miner.GetStartNode());

                if (miner == null)
                    Debug.Log("Miner is null in DepositGoldState");

                if (urbanCenter == null)
                    Debug.Log("Urban center is null in DepositGoldState");

                if (currentNode == null)
                    Debug.Log("Current node is null in DepositGoldState");
                
            });

            

            behaviours.AddMainThreadBehaviour(0, () =>
            {
                if (pathToUrbanCenter != null && pathToUrbanCenter.Count > 0)
                {
                    if (!isMoving)
                    {
                        timeSinceLastMove = 0f;
                        currentNodeIndex = 0;
                        isMoving = true;
                    }

                    timeSinceLastMove += Time.deltaTime;

                    if (timeSinceLastMove >= travelTime)
                    {
                        if (currentNodeIndex < pathToUrbanCenter.Count)
                        {
                            Node<Vector2> nextNode = pathToUrbanCenter[currentNodeIndex];
                            miner.transform.position =
                                new Vector3(nextNode.GetCoordinate().x, nextNode.GetCoordinate().y);
                            miner.SetCurrentNode(nextNode);
                            currentNodeIndex++;
                            timeSinceLastMove = 0f;
                        }
                        else
                        {
                            isMoving = false;
                            Debug.Log("Urban Center reached!");
                        }
                    }
                }
            });


            behaviours.AddMainThreadBehaviour(0, () =>
            {
                if (miner.IsAtUrbanCenter())
                {
                    if (urbanCenter != null)
                    {
                        miner.SetStartNode(urbanCenter);
                        urbanCenter.AddGold(miner.goldCollected);
                        miner.gameManager.UpdateUrbanCenterGoldText();
                        Debug.Log("Gold deposited! Amount: " + urbanCenter.GetGold());
                        miner.goldCollected = 0;
                        alreadyDeposited = true;

                        if (mine == null)
                            noMoreMines = true;

                        pathToUrbanCenter = null;
                    }
                }
            });

            behaviours.SetTransitionBehavior(() =>
            {
                if (miner.gameManager.GetAlarm())
                {
                    OnFlag?.Invoke(BaseAgentsFlags.OnAlarmTrigger);
                }
                else if (noMoreMines)
                {
                    Debug.Log("No more gold mines to mine....");
                    noMoreMines = false;
                    miner.SetStart(false);
                    OnFlag?.Invoke(BaseAgentsFlags.OnNoMoreMines);
                }
                else if (alreadyDeposited && !noMoreMines)
                {
                    alreadyDeposited = false;
                    OnFlag?.Invoke(BaseAgentsFlags.OnGoldDeposit);
                }
            });

            return behaviours;
        }

        public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();
            
            miner = parameters[0] as Miner;
            urbanCenter = parameters[1] as UrbanCenterNode<Vector2>;
            distanceBetweenNodes = Convert.ToSingle(parameters[2]);

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                pathToUrbanCenter = miner.GetAStarPathfinder()
                    .FindPath(miner.GetStartNode(), urbanCenter, distanceBetweenNodes);
                
                if(pathToUrbanCenter != null)
                    Debug.Log("Path to urban center calculated From: " + miner.GetStartNode().GetCoordinate());

            });
                
            return behaviours;
        }

        public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
        {
            return default;
        }
    }
}