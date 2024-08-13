using System;
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

    public enum Directions
    {
        Chase,
        Patrol,
        Explode,
        Shoot,
    }

    public class Agent : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform wayPoint1;
        [SerializeField] private Transform wayPoint2;
        [SerializeField] private float speed;
        [SerializeField] private float arrowSpeed;
        [SerializeField] private float shotDelay;
        [SerializeField] private float chaseDistance;
        [SerializeField] private float explodeDistance;
        [SerializeField] private float shootDistance;
        [SerializeField] private float lostDistance;
        [SerializeField] private bool isCreeper = false;
        private FSM _fsm;
        
        private void Start()
        {
            _fsm = new FSM(Enum.GetValues(typeof(Directions)).Length, Enum.GetValues(typeof(FLags)).Length);
            
            
            
            _fsm.AddBehaviour<PatrolState>((int)Directions.Patrol, PatrolTickParameters);
            _fsm.AddBehaviour<ChaseState>((int)Directions.Chase, ChaseTickParameters);
            _fsm.AddBehaviour<ExplodeState>((int)Directions.Explode, ExplodeTickParameters);
            _fsm.AddBehaviour<ShootState>((int)Directions.Shoot, ShootTickParameters);
            
            
            

            _fsm.SetTransition((int)Directions.Patrol, (int)FLags.OnTargetNear, (int)Directions.Chase);
            _fsm.SetTransition((int)Directions.Chase, (int)FLags.OnTargetReach, (int)Directions.Explode);
            _fsm.SetTransition((int)Directions.Chase, (int)FLags.OnTargetLost, (int)Directions.Patrol);
            _fsm.SetTransition((int)Directions.Chase, (int)FLags.OnTargetShotDistance, (int)Directions.Shoot);
            _fsm.SetTransition((int)Directions.Shoot, (int)FLags.OnTargetLost, (int)Directions.Chase);
            
        }

        private object[] ChaseTickParameters()
        {
            object[] objects = {transform, targetTransform, speed, this.explodeDistance, this.lostDistance, isCreeper, shootDistance};
            return objects;
        }
            
        private object[] PatrolTickParameters()
        {
            object[] objects = {transform, wayPoint1, wayPoint2, this.targetTransform, this.speed, this.chaseDistance, isCreeper};
            return objects;
        }
        
        private object[] ExplodeTickParameters()
        {
            object[] objects = {this.gameObject};
            return objects;
        }
        private object[] ShootTickParameters()
        {
            object[] objects = {transform, targetTransform, this.arrowSpeed, shotDelay, shootDistance };
            return objects;
        }
        private void Update()
        {
            _fsm.Tick();
        }
    }
}