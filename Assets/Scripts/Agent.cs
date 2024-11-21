using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace DefaultNamespace
{
    public enum FLags
    {
        OnTargetReach,
        OnTargetLost,
        OnTargetNear,
        OnTargetShotDistance,
    }

    public enum Behaviours
    {
        Chase,
        Patrol,
        Explode,
        Shoot,
    }

    public class Agent : MonoBehaviour
    {
        //     [SerializeField] private Transform targetTransform;
        //     [SerializeField] private Transform wayPoint1;
        //     [SerializeField] private Transform wayPoint2;
        //     [SerializeField] private float speed;
        //     [SerializeField] private float arrowSpeed;
        //     [SerializeField] private float shotDelay;
        //     [SerializeField] private float chaseDistance;
        //     [SerializeField] private float explodeDistance;
        //     [SerializeField] private float shootDistance;
        //     [SerializeField] private float lostDistance;
        //     [SerializeField] private bool isCreeper = false;
        //     private FSM<Behaviours, FLags> fsm;
        //
        //     private void Start()
        //     {
        //         fsm = new FSM<Behaviours, FLags>();
        //
        //         
        //         //fsm.AddBehaviour<PatrolState>(Behaviours.Patrol, onTickParameters:PatrolTickParameters);
        //         //fsm.AddBehaviour<ChaseState>(Behaviours.Chase, onTickParameters:ChaseTickParameters);
        //         //fsm.AddBehaviour<ExplodeState>(Behaviours.Explode, onTickParameters:ExplodeTickParameters);
        //         //fsm.AddBehaviour<ShootState>(Behaviours.Shoot, onTickParameters:ShootTickParameters);
        //
        //
        //         fsm.SetTransition(Behaviours.Patrol, FLags.OnTargetNear, Behaviours.Chase, () => {Debug.Log("Te vi!");});
        //         fsm.SetTransition(Behaviours.Chase, FLags.OnTargetReach, Behaviours.Explode);
        //         fsm.SetTransition(Behaviours.Chase, FLags.OnTargetLost, Behaviours.Patrol);
        //         fsm.SetTransition(Behaviours.Chase, FLags.OnTargetShotDistance, Behaviours.Shoot);
        //         fsm.SetTransition(Behaviours.Shoot, FLags.OnTargetLost, Behaviours.Chase);
        //         
        //         fsm.ForceState(Behaviours.Patrol);
        //     }
        //
        //     private object[] ChaseTickParameters()
        //     {
        //         object[] objects =
        //         {
        //             transform, targetTransform, speed, this.explodeDistance, this.lostDistance, isCreeper, shootDistance
        //         };
        //         return objects;
        //     }
        //
        //     private object[] PatrolTickParameters()
        //     {
        //         object[] objects =
        //             { transform, wayPoint1, wayPoint2, this.targetTransform, this.speed, this.chaseDistance, isCreeper };
        //         return objects;
        //     }
        //
        //     private object[] ExplodeTickParameters()
        //     {
        //         object[] objects = { this.gameObject };
        //         return objects;
        //     }
        //
        //     private object[] ShootTickParameters()
        //     {
        //         object[] objects = { transform, targetTransform, this.arrowSpeed, shotDelay, shootDistance };
        //         return objects;
        //     }
        //
        //     private void Update()
        //     {
        //         fsm.Tick();
        //     }
        // }
    }
}