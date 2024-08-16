using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DefaultNamespace;
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
        if(multithreadablesBehaviours == null)
            multithreadablesBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
        
        if(!multithreadablesBehaviours.ContainsKey(executionOrder))
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

public sealed class ChaseState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);
        bool isCreeper = Convert.ToBoolean(parameters[5]);
        float shootDistance = Convert.ToSingle(parameters[6]);

        
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultithreadbleBehaviours(0, () => { Debug.Log("Whistle!"); });

        behaviour.AddMainThreadBehaviour(0,() =>
        {
            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized *
                                       (speed * Time.deltaTime);
        });

        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance && isCreeper)
            {
                OnFlag?.Invoke(FLags.OnTargetReach);
            }
        }); 
        
        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < shootDistance && !isCreeper)
            {
                OnFlag?.Invoke(FLags.OnTargetShotDistance);
            }
        });

        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke(FLags.OnTargetLost);
            }
        });

        return behaviour;
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

public sealed class PatrolState : State
{
    private bool direction;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);

        behaviour.AddMultithreadbleBehaviours(0,() =>
        {
            if (Vector3.Distance(ownerTransform.position, direction ? wayPoint1.position : wayPoint2.position) < 0.2f)
            {
                direction = !direction;
            }

        });
        
        behaviour.AddMainThreadBehaviour(1, () =>
        {
            ownerTransform.position +=
                (direction ? wayPoint1.position : wayPoint2.position - ownerTransform.position).normalized *
                (speed * Time.deltaTime);
        });

        behaviour.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke(FLags.OnTargetNear);
            }
        });

        return behaviour;
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

public sealed class ExplodeState : State
{
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();
        behaviour.AddMultithreadbleBehaviours(0, () => { Debug.Log("F"); });

        GameObject ownerObject = parameters[0] as GameObject;

        behaviour.AddMainThreadBehaviour(0, () => { ownerObject.SetActive(false); });

        return behaviour;
    }

    public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        behaviours.AddMultithreadbleBehaviours(0,() => { Debug.Log("Explode!"); });


        return behaviours;
    }

    public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class ShootState : State
{
    float shootDelay;
    float time;
    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        //behaviours.Add(() => { Debug.Log("Shooting!"); });

        Transform ownerTransform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float arrowSpeed = Convert.ToSingle(parameters[2]);
        shootDelay = Convert.ToSingle(parameters[3]);
        float shootDistance = Convert.ToSingle(parameters[4]);
        
        behaviours.AddMultithreadbleBehaviours(0,() =>
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                Debug.Log("Shot!");
                time = shootDelay;
            }
        });
        

        behaviours.SetTransitionBehavior(() =>
        {
            if (Vector3.Distance(targetTransform.position, ownerTransform.position) > shootDistance)
            {
                OnFlag?.Invoke(FLags.OnTargetLost);
            }
        });

        return behaviours;
    }

    public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultithreadbleBehaviours(0,() => { time = shootDelay; });

         return behaviours;
    }

    public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
    {
        return default;
    }
}