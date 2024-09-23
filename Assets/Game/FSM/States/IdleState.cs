using System;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class IdleState : State
    {
        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            bool start = Convert.ToBoolean(parameters[0]);

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                Debug.Log("Idle agent");
            });
        
            behaviours.SetTransitionBehavior(() =>
            {
                if (start)
                {
                    OnFlag?.Invoke(Flags.OnStart);
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