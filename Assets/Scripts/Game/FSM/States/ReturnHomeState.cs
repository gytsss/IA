using System;
using System.Collections.Generic;
using Game.Agents;
using Game.Nodes;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class ReturnHomeState : State
    {
        private List<Node<Vec2Int>> pathToUrbanCenter;
        private GoldMineNode<Vec2Int> mine;
        private bool alreadyDeposited = false;
        private float timeSinceLastMove;
        private int currentNodeIndex;
        private bool isMoving;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            BaseAgent baseAgent = parameters[0] as BaseAgent;
            Node<Vec2Int> currentNode = parameters[1] as Node<Vec2Int>;
            UrbanCenterNode<Vec2Int> urbanCenter = parameters[2] as UrbanCenterNode<Vec2Int>;
            float travelTime = Convert.ToSingle(parameters[3]);
            float distanceBetweenNodes = Convert.ToSingle(parameters[4]);

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                mine = baseAgent.GetClosestGoldMineNode(baseAgent.GetStartNode());

                if (baseAgent == null)
                    Debug.Log("Agent is null in ReturnHomeState");

                if (urbanCenter == null)
                    Debug.Log("Urban center is null in ReturnHomeState");

                if (currentNode == null)
                    Debug.Log("Current node is null in ReturnHomeState");
            });


            behaviours.AddMainThreadBehaviour(0, () =>
            {
                if (pathToUrbanCenter == null || pathToUrbanCenter.Count == 0)
                {
                    pathToUrbanCenter = baseAgent.GetAStarPathfinder()
                        .FindPath(baseAgent.GetStartNode(), urbanCenter, distanceBetweenNodes, baseAgent.GetTransitionCost);
                    Debug.Log("Path to urban center calculated From: " + baseAgent.GetStartNode().GetCoordinate());
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
                            baseAgent.transform.position =
                                new Vector3(nextNode.GetCoordinate().x, nextNode.GetCoordinate().y);
                            baseAgent.SetCurrentNode(nextNode);
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


            behaviours.SetTransitionBehavior(() =>
            {
                if (baseAgent.gameManager.GetAlarm())
                {
                    OnFlag?.Invoke(Flags.OnAlarmTrigger);
                }
                else if (baseAgent.IsAtUrbanCenter())
                {
                    baseAgent.SetStartNode(urbanCenter);
                    pathToUrbanCenter = null;
                    OnFlag?.Invoke(Flags.OnHome);
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