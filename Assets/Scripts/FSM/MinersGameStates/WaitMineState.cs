﻿// using FSM;
// using MinersGame.FSM.States;
// using Pathfinder;
// using UnityEngine;
//
// namespace Game.FSM.States
// {
//     public sealed class WaitMineState : State
//     {
//         private GoldMineNode<Vector2> mine;
//
//         public override BehavioursActions GetTickBehaviour(params object[] parameters)
//         {
//             BehavioursActions behaviours = new BehavioursActions();
//
//             GameManager gameManager = parameters[0] as GameManager;
//
//             behaviours.AddMultithreadbleBehaviours(0, () =>
//             {
//                 if (mine == null)
//                     Debug.Log("No mines being mined");
//             });
//
//
//             behaviours.AddMultithreadbleBehaviours(0, () =>
//             {
//                 mine = gameManager.goldMineManager.FindClosestGoldMineBeingMined(gameManager.GetUrbanCenterNode());
//             });
//
//             behaviours.SetTransitionBehavior(() =>
//             {
//                 if (gameManager.GetAlarm())
//                 {
//                     OnFlag?.Invoke(BaseAgentsFlags.OnAlarmTrigger);
//                 }
//                 else if (mine != null)
//                 {
//                     Debug.Log("Mine is being mined!");
//                     OnFlag?.Invoke(BaseAgentsFlags.OnMineFind);
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
// }