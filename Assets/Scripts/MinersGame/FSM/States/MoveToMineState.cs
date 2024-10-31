using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class MoveToMineState : State
    {
        private BaseAgent agent;
        private float timeSinceLastMove;
        private int currentNodeIndex;
        private List<Node<Vec2Int>> path;
        GoldMineNode<Vec2Int> destinationNode;
        Node<Vec2Int> startNode;
        private Transform agentTransform;
        private float travelTime;
        private bool isMoving;
        private float distanceBetweenNodes;
        private bool noDestination = false;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            agentTransform = parameters[0] as Transform;
            travelTime = Convert.ToSingle(parameters[1]);


            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                if (agent == null)
                    Debug.Log("Null agent in MoveToMineState");
                if (destinationNode == null)
                {
                    Debug.Log("Null destinationNode in MoveToMineState");
                    noDestination = true;
                }
                if (startNode == null)
                    Debug.Log("Null startNode in MoveToMineState");
            });


            behaviours.AddMainThreadBehaviour(0, () =>
            {
                if (path != null && path.Count > 0 && !noDestination)
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
                        if (currentNodeIndex < path.Count)
                        {
                            Node<Vec2Int> node = path[currentNodeIndex];
                            agentTransform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
                            agent.SetCurrentNode(node);
                            currentNodeIndex++;
                            timeSinceLastMove = 0f;
                        }
                        else
                        {
                            isMoving = false;
                            Debug.Log("Destination reached! x: " + destinationNode.GetCoordinate().x + " y: " +
                                      destinationNode.GetCoordinate().y);
                            //miner.SetCurrentMine(destinationNode);
                        }
                    }
                }
            });

            behaviours.SetTransitionBehavior(() =>
            {
                if (agent.gameManager.GetAlarm())
                {
                    Debug.Log("Alarm when moving to mine!");
                    OnFlag?.Invoke(Flags.OnAlarmTrigger);
                }
                else if (noDestination)
                {
                    agent.SetStartNode(agent.GetCurrentNode());
                    noDestination = false;
                    OnFlag?.Invoke(Flags.OnNoMoreMines);
                }
                else if (agent.IsAtMine(destinationNode))
                {
                    //miner.SetCurrentMine(destinationNode);
                    agent.SetStartNode(destinationNode);
                    Debug.Log("Start mining! x: " + destinationNode.GetCoordinate().x + " y: " +
                              destinationNode.GetCoordinate().y);
                    OnFlag?.Invoke(Flags.OnMineFind);
                    OnFlag?.Invoke(Flags.OnFoodDeposit);
                }
               
            });

            return behaviours;
        }

        public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            agent = parameters[0] as BaseAgent;
            distanceBetweenNodes = Convert.ToSingle(parameters[1]);
            startNode = parameters[2] as Node<Vec2Int>;

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                if (agent == null)
                    Debug.Log("Agent is null in MoveToMineState");
            });

            behaviours.AddMainThreadBehaviour(0, () =>
            {
                if (agent.GetIsMiner())
                    agent.SetDestinationNode(agent.GetClosestGoldMineNode(startNode));
                else
                    agent.SetDestinationNode(agent.GetClosestGoldMineNodeBeingMined(startNode));

                destinationNode = agent.GetDestinationNode() as GoldMineNode<Vec2Int>;

                path = agent.GetAStarPathfinder().FindPath(startNode, destinationNode, distanceBetweenNodes);

                if (path == null)
                    Debug.Log("Path is null. No valid path found.");
                
                if(destinationNode == null)
                    Debug.Log("Destination node is null onEnter");
            });

            return behaviours;
        }

        public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
        {
            return default;
        }
    }
}