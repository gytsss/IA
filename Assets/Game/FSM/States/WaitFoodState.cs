using UnityEngine;

namespace Game.FSM.States
{
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
                    OnFlag?.Invoke(Flags.OnAlarmTrigger);
                }
                else if (miner.GetCurrentMine().HasFood())
                {
                    OnFlag?.Invoke(Flags.OnFoodAvailable);
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

}