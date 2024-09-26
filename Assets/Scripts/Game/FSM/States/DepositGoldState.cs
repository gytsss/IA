using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Nodes;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class DepositGoldState : State
    {
        private List<Node<Vec2Int>> pathToUrbanCenter;
        private GoldMineNode<Vec2Int> mine;
        private bool alreadyDeposited = false;
        private float timeSinceLastMove;
        private int currentNodeIndex;
        private bool isMoving;
        private bool noMoreMines = false;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            Miner miner = parameters[0] as Miner;
            Node<Vec2Int> currentNode = parameters[1] as Node<Vec2Int>;
            UrbanCenterNode<Vec2Int> urbanCenter = parameters[2] as UrbanCenterNode<Vec2Int>;
            float travelTime = Convert.ToSingle(parameters[3]);
            float distanceBetweenNodes = Convert.ToSingle(parameters[4]);

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
                if (pathToUrbanCenter == null || pathToUrbanCenter.Count == 0)
                {
                    pathToUrbanCenter = miner.GetAStarPathfinder()
                        .FindPath(miner.GetStartNode(), urbanCenter, distanceBetweenNodes, miner.GetTransitionCost);
                    Debug.Log("Path to urban center calculated From: " + miner.GetStartNode().GetCoordinate());
                }

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
                            Node<Vec2Int> nextNode = pathToUrbanCenter[currentNodeIndex];
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
                    OnFlag?.Invoke(Flags.OnAlarmTrigger);
                }
                else if (noMoreMines)
                {
                    Debug.Log("No more gold mines to mine....");
                    noMoreMines = false;
                    miner.SetStart(false);
                    OnFlag?.Invoke(Flags.OnNoMoreMines);
                }
                else if (alreadyDeposited && !noMoreMines)
                {
                    alreadyDeposited = false;
                    OnFlag?.Invoke(Flags.OnGoldDeposit);
                }
            });

            return behaviours;
        }

        public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
        {
            return default;
        }

        public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
        {
            return default;
        }
    }
}