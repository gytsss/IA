using Game.Agents;
using Game.Managers;
using Game.Nodes;
using UnityEngine;

namespace Game.FSM.States
{
    public sealed class WaitMineState : State
    {
        private GoldMineNode<Vec2Int> mine;

        public override BehavioursActions GetTickBehaviour(params object[] parameters)
        {
            BehavioursActions behaviours = new BehavioursActions();

            GameManager gameManager = parameters[0] as GameManager;

            behaviours.AddMultithreadbleBehaviours(0, () =>
            {
                if (mine == null)
                    Debug.Log("No mines being mined");
            });

            behaviours.AddMainThreadBehaviour(0,
                () =>
                {
                    mine = gameManager.goldMineManager.FindClosestGoldMineBeingMined(gameManager.GetUrbanCenterNode());
                });

            behaviours.SetTransitionBehavior(() =>
            {
                if (gameManager.GetAlarm())
                {
                    OnFlag?.Invoke(Flags.OnAlarmTrigger);
                }
                else if (mine != null)
                {
                    Debug.Log("Mine is being mined!");
                    OnFlag?.Invoke(Flags.OnMineFind);
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