using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DefaultNamespace;
using Pathfinder;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public struct BehavioursActions
{
    private Dictionary<int, List<Action>> mainThreadBehaviours;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multithreadablesBehaviours;
    private Action transitionBehavior;

    public void AddMainThreadBehaviour(int executionOrder, Action behaviour)
    {
        if (mainThreadBehaviours == null)
            mainThreadBehaviours = new Dictionary<int, List<Action>>();

        if (!mainThreadBehaviours.ContainsKey(executionOrder))
            mainThreadBehaviours.Add(executionOrder, new List<Action>());

        mainThreadBehaviours[executionOrder].Add(behaviour);
    }

    public void AddMultithreadbleBehaviours(int executionOrder, Action behaviour)
    {
        if (multithreadablesBehaviours == null)
            multithreadablesBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();

        if (!multithreadablesBehaviours.ContainsKey(executionOrder))
            multithreadablesBehaviours.TryAdd(executionOrder, new ConcurrentBag<Action>());

        multithreadablesBehaviours[executionOrder].Add(behaviour);
    }

    public void SetTransitionBehavior(Action behaviour)
    {
        transitionBehavior = behaviour;
    }

    public Dictionary<int, List<Action>> MainThreadBehaviours => mainThreadBehaviours;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultithreadblesBehaviours => multithreadablesBehaviours;
    public Action TransitionBehavior => transitionBehavior;
}

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehavioursActions GetTickBehaviour(params object[] parameters);
    public abstract BehavioursActions GetOnEnterBehaviour(params object[] parameters);
    public abstract BehavioursActions GetOnExitBehaviour(params object[] parameters);
}

public sealed class IdleState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        BaseAgent<MinerStates, MinerFlags> miner = parameters[0] as BaseAgent<MinerStates, MinerFlags>;
        bool start = Convert.ToBoolean(parameters[1]);

        //behaviours.AddMultithreadbleBehaviours(0, () => { Debug.Log("Idle..."); });


        behaviours.SetTransitionBehavior(() =>
        {
            if (start)
            {
                OnFlag?.Invoke(MinerFlags.OnStart);
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

public sealed class MoveToMineState : State
{
    private float timeSinceLastMove;
    private int currentNodeIndex;
    private List<Node<Vec2Int>> path;
    GoldMineNode<Vec2Int> destinationNode;
    Node<Vec2Int> startNode;
    private Transform minerTransform;
    private float travelTime;
    private bool isMoving;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;
        minerTransform = parameters[1] as Transform;
        travelTime = Convert.ToSingle(parameters[2]);
        float distanceBetweenNodes = Convert.ToSingle(parameters[3]);


        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            startNode = miner.GetStartNode();
            miner.SetDestinationNode(miner.GetClosestGoldMineNode(startNode));
            destinationNode = miner.GetDestinationNode() as GoldMineNode<Vec2Int>;
            
            if (miner == null)
                Debug.Log("Null miner in MoveToMineState");
            if (destinationNode == null)
                Debug.Log("Null mine in MoveToMineState");
            if (startNode == null)
                Debug.Log("Null startNode in MoveToMineState");
        });


        behaviours.AddMainThreadBehaviour(0, () =>
        {
            startNode = miner.GetStartNode();
            miner.SetDestinationNode(miner.GetClosestGoldMineNode(startNode));
            destinationNode = miner.GetDestinationNode() as GoldMineNode<Vec2Int>;
            
            path = miner.GetAStarPathfinder().FindPath(startNode, destinationNode, distanceBetweenNodes);

            if (path == null)
                Debug.Log("Path is null. No valid path found.");

            if (path != null && path.Count > 0)
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
                        minerTransform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
                        miner.SetCurrentNode(node);
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
            if (miner.gameManager.GetAlarm())
            {
                Debug.Log("Alarm when moving to mine!");
                OnFlag?.Invoke(MinerFlags.OnAlarmTrigger);
            }
            else if (miner.IsAtMine(destinationNode))
            {
                //miner.SetCurrentMine(destinationNode);
                miner.SetStartNode(destinationNode);
                Debug.Log("Start mining! x: " + destinationNode.GetCoordinate().x + " y: " +
                          destinationNode.GetCoordinate().y);
                OnFlag?.Invoke(MinerFlags.OnMineFind);
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

public sealed class MineGoldState : State
{
    private GoldMineNode<Vec2Int> mine;
    private Miner miner;
    private float timeSinceLastExtraction = 0f;
    private int goldCount = 0;
    private bool noMoreMines = false;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        float goldExtractionSpeed = Convert.ToSingle(parameters[0]);
        int maxGold = Convert.ToInt32(parameters[1]);

        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            if (mine == null)
            {
                Debug.Log("Mine is null in MineGoldState because no mine with gold was found");
                noMoreMines = true;
            }

            if (miner == null)
                Debug.Log("Miner is null in MineGoldState");
        });

        behaviours.AddMainThreadBehaviour(0, () =>
        {
            if (mine != null)
            {
                timeSinceLastExtraction += Time.deltaTime;

                if (timeSinceLastExtraction >= goldExtractionSpeed)
                {
                    if (mine.HasGold() && miner.GetEnergy() > 0)
                    {
                        mine.MineGold();
                        goldCount++;
                        miner.goldCollected ++;
                        miner.SetEnergy(miner.GetEnergy() - 1);
                        timeSinceLastExtraction = 0f;
                        Debug.Log("Mine Gold amount: " + mine.GetGoldAmount() +" at mine " + mine.GetCoordinate());
                        Debug.Log("Gold mined: " + goldCount);
                    }
                }
            }
        });


        behaviours.SetTransitionBehavior(() =>
        {
            if (noMoreMines)
            {
                miner.SetStartNode(miner.GetCurrentNode());
                Debug.Log("No more mines! Back to urban center! From: " + miner.GetCurrentNode().GetCoordinate());
                goldCount = 0;
                noMoreMines = false;
                OnFlag?.Invoke(MinerFlags.OnFullInventory);
            }
            else if (miner.gameManager.GetAlarm())
            {
                Debug.Log("Alarm when mining!");
                OnFlag?.Invoke(MinerFlags.OnAlarmTrigger);
            }
            else if (miner.goldCollected >= maxGold)
            {
                Debug.Log("Full inventory!");
                goldCount = 0;
                OnFlag?.Invoke(MinerFlags.OnFullInventory);
            }
            else if (!mine.HasGold())
            {
                Debug.Log("Mine empty!");
                OnFlag?.Invoke(MinerFlags.OnMineEmpty);
            }
            else if (miner.GetEnergy() <= 0)
            {
                Debug.Log("Food needed! Gold mined: " + goldCount);
                OnFlag?.Invoke(MinerFlags.OnFoodNeed);
            }
            
        });

        return behaviours;
    }

    public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        
        miner = parameters[0] as Miner;
        
        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            miner.SetStartNode(miner.GetCurrentNode());
            mine = miner.GetClosestGoldMineNode(miner.GetStartNode());

            if (mine == null)
            {
                Debug.Log("Mine is null in MineGoldState because no mine with gold was found");
                noMoreMines = true;
            }

            if (miner == null)
                Debug.Log("Miner is null in MineGoldState");
        });
        
        return behaviours;
    }

    public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class EatFoodState : State
{
    private GoldMineNode<Vec2Int> mine;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;


        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            mine = miner.GetClosestGoldMineNode(miner.GetStartNode());

            if (miner == null || mine == null)
                Debug.Log("Miner or mine is null in EatFoodState");
        });

        behaviours.AddMainThreadBehaviour(0, () =>
        {
            if (mine != null && mine.HasFood())
            {
                mine.ConsumeFood();
                Debug.Log("Food consumed! Food left: " + mine.GetFoodAmount());

                miner.ResetEnergy();
            }
            else
            {
                Debug.Log("No food available!");
            }
        });

        behaviours.SetTransitionBehavior(() =>
        {
            if (miner.GetEnergy() >= miner.GetMaxEnergy())
            {
                Debug.Log("Back to work! Food left: " + mine.GetFoodAmount());
                OnFlag?.Invoke(MinerFlags.OnFoodEaten);
            }
            else if (!mine.HasFood())
            {
                Debug.Log("Mine empty of food!");
                OnFlag?.Invoke(MinerFlags.OnMineEmptyOfFood);
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

public sealed class WaitFoodState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;


        behaviours.AddMultithreadbleBehaviours(0, () => { Debug.Log("Waiting for food..."); });

        behaviours.AddMainThreadBehaviour(0, () => { });

        behaviours.SetTransitionBehavior(() =>
        {
            if (miner.gameManager.GetAlarm())
            {
                OnFlag?.Invoke(MinerFlags.OnAlarmTrigger);
            }
            else if (miner.GetCurrentMine().HasFood())
            {
                OnFlag?.Invoke(MinerFlags.OnFoodAvailable);
            }
            else
            {
                // MinerEvents.OnNeedFood?.Invoke(miner);
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
                    .FindPath(miner.GetStartNode(), urbanCenter, distanceBetweenNodes);
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
                        miner.transform.position = new Vector3(nextNode.GetCoordinate().x, nextNode.GetCoordinate().y);
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
                OnFlag?.Invoke(MinerFlags.OnAlarmTrigger);
            }
            else if (noMoreMines)
            {
                Debug.Log("No more gold mines to mine....");
                noMoreMines = false;
                miner.SetStart(false);
                OnFlag?.Invoke(MinerFlags.OnNoMoreMines);
            }
            else if (alreadyDeposited && !noMoreMines)
            {
                alreadyDeposited = false;
                OnFlag?.Invoke(MinerFlags.OnGoldDeposit);
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

public sealed class AlarmState : State
{
    private List<Node<Vec2Int>> pathToUrbanCenter;
    private float timeSinceLastMove;
    private int currentNodeIndex;
    private bool isMoving;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        BaseAgent<MinerStates, MinerFlags> agent = parameters[0] as BaseAgent<MinerStates, MinerFlags>;
        UrbanCenterNode<Vec2Int> urbanCenter = parameters[1] as UrbanCenterNode<Vec2Int>;
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
                        Node<Vec2Int> nextNode = pathToUrbanCenter[currentNodeIndex];
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
                OnFlag?.Invoke(MinerFlags.OnBackToWork);
            }

            if (agent.IsAtUrbanCenter())
            {
                agent.SetStartNode(urbanCenter);
                pathToUrbanCenter = null;
                agent.SetStart(false);
                Debug.Log("Alarm state finished. Agent is now at the urban center.");
                OnFlag?.Invoke(MinerFlags.OnHome);
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


// public sealed class ChaseState : State
// {
//     public override BehavioursActions GetTickBehaviour(params object[] parameters)
//     {
//         Transform OwnerTransform = parameters[0] as Transform;
//         Transform TargetTransform = parameters[1] as Transform;
//         float speed = Convert.ToSingle(parameters[2]);
//         float explodeDistance = Convert.ToSingle(parameters[3]);
//         float lostDistance = Convert.ToSingle(parameters[4]);
//         bool isCreeper = Convert.ToBoolean(parameters[5]);
//         float shootDistance = Convert.ToSingle(parameters[6]);
//
//
//         BehavioursActions behaviour = new BehavioursActions();
//
//         behaviour.AddMultithreadbleBehaviours(0, () => { Debug.Log("Whistle!"); });
//
//         behaviour.AddMainThreadBehaviour(0, () =>
//         {
//             OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized *
//                                        (speed * Time.deltaTime);
//         });
//         
//
//         behaviour.SetTransitionBehavior(() =>
//         {
//             if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < shootDistance && !isCreeper)
//             {
//                 OnFlag?.Invoke(FLags.OnTargetShotDistance);
//             }
//             
//             if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance && isCreeper)
//             {
//                 OnFlag?.Invoke(FLags.OnTargetReach);
//             }
//             
//             if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
//             {
//                 OnFlag?.Invoke(FLags.OnTargetLost);
//             }
//         });
//         
//
//         return behaviour;
//     }
//
//     public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
//     {
//         return default;
//     }
//
//     public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//     {
//         return default;
//     }
// }
//
// public sealed class PatrolState : State
// {
//     private bool direction;
//
//     public override BehavioursActions GetTickBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviour = new BehavioursActions();
//
//         Transform ownerTransform = parameters[0] as Transform;
//         Transform wayPoint1 = parameters[1] as Transform;
//         Transform wayPoint2 = parameters[2] as Transform;
//         Transform chaseTarget = parameters[3] as Transform;
//         float speed = Convert.ToSingle(parameters[4]);
//         float chaseDistance = Convert.ToSingle(parameters[5]);
//
//         behaviour.AddMainThreadBehaviour(0, () =>
//         {
//             if (Vector3.Distance(ownerTransform.position, direction ? wayPoint1.position : wayPoint2.position) < 0.2f)
//             {
//                 direction = !direction;
//             }
//
//             ownerTransform.position +=
//                 (direction ? wayPoint1.position : wayPoint2.position - ownerTransform.position).normalized *
//                 (speed * Time.deltaTime);
//         });
//
//         behaviour.SetTransitionBehavior(() =>
//         {
//             if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance)
//             {
//                 OnFlag?.Invoke(FLags.OnTargetNear);
//             }
//         });
//
//         return behaviour;
//     }
//
//     public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
//     {
//         return default;
//     }
//
//     public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//     {
//         return default;
//     }
// }
//
// public sealed class ExplodeState : State
// {
//     public override BehavioursActions GetTickBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviour = new BehavioursActions();
//         behaviour.AddMultithreadbleBehaviours(0, () => { Debug.Log("F"); });
//
//         GameObject ownerObject = parameters[0] as GameObject;
//
//         behaviour.AddMainThreadBehaviour(0, () => { ownerObject.SetActive(false); });
//
//         return behaviour;
//     }
//
//     public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviours = new BehavioursActions();
//         behaviours.AddMultithreadbleBehaviours(0, () => { Debug.Log("Explode!"); });
//
//
//         return behaviours;
//     }
//
//     public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//     {
//         return default;
//     }
// }
//
// public sealed class ShootState : State
// {
//     float shootDelay;
//     float time;
//
//     public override BehavioursActions GetTickBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviours = new BehavioursActions();
//
//         Transform ownerTransform = parameters[0] as Transform;
//         Transform targetTransform = parameters[1] as Transform;
//         float arrowSpeed = Convert.ToSingle(parameters[2]);
//         shootDelay = Convert.ToSingle(parameters[3]);
//         float shootDistance = Convert.ToSingle(parameters[4]);
//
//         behaviours.AddMainThreadBehaviour(0, () =>
//         {
//             time -= Time.deltaTime;
//
//             if (time <= 0)
//             {
//                 Debug.Log("Shot!");
//                 time = shootDelay;
//             }
//         });
//
//
//         behaviours.SetTransitionBehavior(() =>
//         {
//             if (Vector3.Distance(targetTransform.position, ownerTransform.position) > shootDistance)
//             {
//                 OnFlag?.Invoke(FLags.OnTargetLost);
//             }
//         });
//
//         return behaviours;
//     }
//
//     public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviours = new BehavioursActions();
//
//         behaviours.AddMultithreadbleBehaviours(0, () => { time = shootDelay; });
//
//         return behaviours;
//     }
//
//     public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//     {
//         return default;
//     }
// }