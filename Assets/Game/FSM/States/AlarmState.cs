using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.States
{
   public sealed class AlarmState : State
{
    private List<Node<Vector2>> pathToUrbanCenter;
    private float timeSinceLastMove;
    private int currentNodeIndex;
    private bool isMoving;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        BaseAgent agent = parameters[0] as BaseAgent;
        UrbanCenterNode<Vector2> urbanCenter = parameters[1] as UrbanCenterNode<Vector2>;
        float distanceBetweenNodes = Convert.ToSingle(parameters[2]);

        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            agent.SetStartNode(agent.GetCurrentNode());
            if (agent == null)
                Debug.Log("Agent is null in AlarmState.");

            if (urbanCenter == null)
                Debug.Log("Urban center is null in AlarmState.");
        });

        behaviours.AddMainThreadBehaviour(0, () =>
        {
            if (pathToUrbanCenter == null || pathToUrbanCenter.Count == 0)
            {
                pathToUrbanCenter = agent.GetAStarPathfinder()
                    .FindPath(agent.GetStartNode(), urbanCenter, distanceBetweenNodes);
                Debug.Log("Path to urban center calculated for agent during alarm!");
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

                if (timeSinceLastMove >= agent.GetTravelTime())
                {
                    if (currentNodeIndex < pathToUrbanCenter.Count)
                    {
                        Node<Vector2> nextNode = pathToUrbanCenter[currentNodeIndex];
                        agent.transform.position = new Vector3(nextNode.GetCoordinate().x, nextNode.GetCoordinate().y);
                        agent.SetCurrentNode(nextNode);
                        currentNodeIndex++;
                        timeSinceLastMove = 0f;
                    }
                    else
                    {
                        isMoving = false;
                        Debug.Log("Agent reached the urban center during the alarm!");
                    }
                }
            }
        });

        behaviours.SetTransitionBehavior(() =>
        {
            if (!agent.gameManager.GetAlarm())
            {
                Debug.Log("Alarm disable!");
                agent.SetStartNode(agent.GetCurrentNode());
                OnFlag?.Invoke(Flags.OnBackToWork);
            }

            if (agent.IsAtUrbanCenter())
            {
                agent.SetStartNode(urbanCenter);
                pathToUrbanCenter = null;
                agent.SetStart(false);
                Debug.Log("Alarm state finished. Agent is now at the urban center.");
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