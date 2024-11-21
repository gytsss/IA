// using FSM.States;
// using MinersGame.FSM.States;
// using UnityEngine;
//
// namespace Game.FSM.States
// {
//     public sealed class WaitFoodState : State
//     {
//         public override BehavioursActions GetTickBehaviour(params object[] parameters)
//         {
//             BehavioursActions behaviours = new BehavioursActions();
//
//             Miner miner = parameters[0] as Miner;
//
//
//             behaviours.AddMultithreadbleBehaviours(0, () => { Debug.Log("Waiting for food..."); });
//             
//
//             behaviours.SetTransitionBehavior(() =>
//             {
//                 if (miner.gameManager.GetAlarm())
//                 {
//                     OnFlag?.Invoke(BaseAgentsFlags.OnAlarmTrigger);
//                 }
//                 else if (miner.GetCurrentMine().HasFood())
//                 {
//                     OnFlag?.Invoke(BaseAgentsFlags.OnFoodAvailable);
//                 }
//                 else
//                 {
//                     // MinerEvents.OnNeedFood?.Invoke(miner);
//                 }
//             });
//
//             return behaviours;
//         }
//
//         public override BehavioursActions GetOnEnterBehaviour(params object[] parameters)
//         {
//             return default;
//         }
//
//         public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//         {
//             return default;
//         }
//     }
//
// }