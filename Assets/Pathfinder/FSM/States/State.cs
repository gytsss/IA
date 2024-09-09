using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DefaultNamespace;
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

        bool start = Convert.ToBoolean(parameters[0]);

        behaviours.AddMultithreadbleBehaviours(0, () => { Debug.Log("Idle..."); });


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
    private List<Node<Vector2Int>> path;
    private Transform minerTransform;
    private float travelTime;
    private bool isMoving;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;
        minerTransform = parameters[1] as Transform;
        Node<Vector2Int> mine = parameters[2] as Node<Vector2Int>;
        Node<Vector2Int> startNode = parameters[3] as Node<Vector2Int>;
        travelTime = Convert.ToSingle(parameters[4]);
        float distanceBetweenNodes = Convert.ToSingle(parameters[5]);
        path = parameters[6] as List<Node<Vector2Int>>;

        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            if (miner == null || mine == null || startNode == null)
                Debug.Log("Null parameters in MoveToMineState");
        });

        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            if (path == null)
                Debug.Log("Path is null. No valid path found.");
        });


        behaviours.AddMainThreadBehaviour(0, () =>
        {
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
                        Node<Vector2Int> node = path[currentNodeIndex];
                        minerTransform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
                        currentNodeIndex++;
                        timeSinceLastMove = 0f;
                    }
                    else
                    {
                        isMoving = false;
                        Debug.Log("Destination reached! x: " + mine.GetCoordinate().x + " y: " +
                                  mine.GetCoordinate().y);
                    }
                }
            }
        });

        behaviours.SetTransitionBehavior(() =>
        {
            if (miner.IsAtMine(mine))
            {
                miner.SetCurrentMine(mine);
                Debug.Log("Start mining!");
                // OnFlag?.Invoke(MinerFlags.OnGoldExtract);
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
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;
        //GoldMine mine = parameters[1] as GoldMine;

        behaviours.AddMainThreadBehaviour(0, () =>
        {
            // if (mine.HasGold() && miner.goldCollected < miner.maxGold)
            // {
            //     mine.MineGold();  // Extrae oro de la mina
            //     miner.goldCollected++;
            // }
        });


        behaviours.SetTransitionBehavior(() =>
        {
            // if (miner.goldCollected >= miner.maxGold)
            // {
            //     OnFlag?.Invoke(MinerFlags.OnGoldFull);
            // }
            // else if (miner.foodCollected >= miner.foodNeededAfterGold)
            // {
            //     OnFlag?.Invoke(MinerFlags.OnFoodNeeded);
            // }
            // else if (!mine.HasGold())
            // {
            //     OnFlag?.Invoke(MinerFlags.OnMineEmpty);
            // }
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

public sealed class EatFoodState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Miner miner = parameters[0] as Miner;
        //GoldMine mine = parameters[1] as GoldMine;


        behaviours.AddMainThreadBehaviour(0, () =>
        {
            // if (mine.foodAvailable > 0)
            // {
            //     mine.ConsumeFood();
            //     miner.foodCollected++;
            // }
        });

        behaviours.SetTransitionBehavior(() =>
        {
            // if (miner.foodCollected >= miner.foodNeededAfterGold)
            // {
            //     OnFlag?.Invoke(MinerFlags.OnFoodAvailable);
            // }
            // else if (mine.IsOutOfFood())
            // {
            //     OnFlag?.Invoke(MinerFlags.OnMineEmpty);
            // }
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
        // GoldMine mine = parameters[1] as GoldMine;


        behaviours.AddMainThreadBehaviour(0, () => { Debug.Log("Esperando a que la caravana traiga comida."); });

        behaviours.SetTransitionBehavior(() =>
        {
            // if (mine.foodAvailable > 0)
            // {
            //     OnFlag?.Invoke(MinerFlags.OnFoodAvailable);
            // }
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
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        Transform ownerTransform = parameters[0] as Transform;
        Node<Vector2Int> urbanCenter = parameters[1] as Node<Vector2Int>;
        float speed = Convert.ToSingle(parameters[2]);


        behaviours.AddMainThreadBehaviour(0, () =>
        {
            // ownerTransform.position += (urbanCenter.Position - (Vector2Int)ownerTransform.position).normalized *
            //                            (speed * Time.deltaTime);
        });

        behaviours.SetTransitionBehavior(() =>
        {
            // if (Vector2Int.Distance((Vector2Int)ownerTransform.position, urbanCenter.Position) < 0.1f)
            // {
            //     OnFlag?.Invoke(MinerFlags.OnMineEmpty);  // Cambiar a OnGoldDeposited si es necesario
            // }
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

public sealed class ReturnHomeState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMainThreadBehaviour(0, () => { });

        behaviours.SetTransitionBehavior(() => { });

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
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMainThreadBehaviour(0, () => { });

        behaviours.SetTransitionBehavior(() => { });

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