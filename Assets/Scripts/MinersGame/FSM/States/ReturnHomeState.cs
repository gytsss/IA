using System;
using System.Collections.Generic;
using MinersGame.FSM.States;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class ReturnHomeState : State
    {
        private List<Node<Vector2>> pathToUrbanCenter;
        private GoldMineNode<Vector2> mine;
        private bool alreadyDeposited = false;
        private float timeSinceLastMove;
        private int currentNodeIndex;
        private bool isMoving;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            BaseAgent baseAgent = parameters[0] as BaseAgent;
            Node<Vector2> currentNode = parameters[1] as Node<Vector2>;
            UrbanCenterNode<Vector2> urbanCenter = parameters[2] as UrbanCenterNode<Vector2>;
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
            
            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                if (pathToUrbanCenter == null || pathToUrbanCenter.Count == 0)
                {
                    pathToUrbanCenter = baseAgent.GetAStarPathfinder()
                        .FindPath(baseAgent.GetStartNode(), urbanCenter, distanceBetweenNodes);
                    Debug.Log("Path to urban center calculated From: " + baseAgent.GetStartNode().GetCoordinate());
                }
            });


            behaviours.AddMainThreadBehaviour(1, () =>
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