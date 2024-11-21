// using System;
// using MinersGame.FSM.States;
// using UnityEngine;
//
// namespace Game.FSM.States
// {
//    public sealed class MineGoldState : State
// {
//     private GoldMineNode<Vector2> mine;
//     private Miner miner;
//     private float timeSinceLastExtraction = 0f;
//     private int goldCount = 0;
//     private bool noMoreMines = false;
//
//     public override BehavioursActions GetTickBehaviour(params object[] parameters)
//     {
//         BehavioursActions behaviours = new BehavioursActions();
//
//         float goldExtractionSpeed = Convert.ToSingle(parameters[0]);
//         int maxGold = Convert.ToInt32(parameters[1]);
//
//         behaviours.AddMultithreadbleBehaviours(0, () =>
//         {
//             if (mine == null)
//             {
//                 Debug.Log("Mine is null in MineGoldState because no mine with gold was found");
//                 noMoreMines = true;
//             }
//
//             if (miner == null)
//                 Debug.Log("Miner is null in MineGoldState");
//         });
//
//         behaviours.AddMainThreadBehaviour(0, () =>
//         {
//             if (mine != null)
//             {
//                 timeSinceLastExtraction += Time.deltaTime;
//
//                 if (timeSinceLastExtraction >= goldExtractionSpeed)
//                 {
//                     if (mine.HasGold() && miner.GetEnergy() > 0)
//                     {
//                         mine.SetBeingMined(true);
//                         mine.MineGold();
//                         goldCount++;
//                         miner.goldCollected++;
//                         miner.SetEnergy(miner.GetEnergy() - 1);
//                         timeSinceLastExtraction = 0f;
//                         Debug.Log("Mine Gold amount: " + mine.GetGoldAmount() + " at mine " + mine.GetCoordinate() +
//                                   "is being mined: " + mine.IsBeingMined());
//                         Debug.Log("Gold mined: " + goldCount);
//
//                         if (!mine.HasGold() && miner.GetClosestGoldMineNode(miner.GetCurrentNode()) == null)
//                         {
//                             noMoreMines = true;
//                             mine.SetBeingMined(false);
//                         }
//                     }
//                 }
//             }
//         });
//
//
//         behaviours.SetTransitionBehavior(() =>
//         {
//             if (noMoreMines)
//             {
//                 miner.SetStartNode(miner.GetCurrentNode());
//                 Debug.Log("No more mines! Back to urban center! From: " + miner.GetCurrentNode().GetCoordinate());
//                 goldCount = 0;
//                 noMoreMines = false;
//                 OnFlag?.Invoke(BaseAgentsFlags.OnFullInventory);
//             }
//             else if (miner.gameManager.GetAlarm())
//             {
//                 mine.SetBeingMined(false);
//                 Debug.Log("Alarm when mining!");
//                 OnFlag?.Invoke(BaseAgentsFlags.OnAlarmTrigger);
//             }
//             else if (miner.goldCollected >= maxGold)
//             {
//                 if (!mine.HasGold())
//                 {
//                     miner.gameManager.goldMineManager.goldMinesVoronois.Remove(new NodeVoronoi(mine.GetCoordinate()));
//                     miner.gameManager.goldMineManager.goldMines.Remove(mine);
//                     miner.gameManager.voronoi.SetVoronoi(miner.gameManager.goldMineManager.goldMinesVoronois, miner.gameManager.GetNodeVoronoiMapSize());
//                     mine.SetBeingMined(false);
//                 }
//                 
//                 mine.SetBeingMined(false);
//                 Debug.Log("Full inventory!");
//                 goldCount = 0;
//                 OnFlag?.Invoke(BaseAgentsFlags.OnFullInventory);
//             }
//             else if (!mine.HasGold())
//             {
//                 miner.gameManager.goldMineManager.goldMinesVoronois.Remove(new NodeVoronoi(mine.GetCoordinate()));
//                 miner.gameManager.goldMineManager.goldMines.Remove(mine);
//                 miner.gameManager.voronoi.SetVoronoi(miner.gameManager.goldMineManager.goldMinesVoronois, miner.gameManager.GetNodeVoronoiMapSize());
//                 mine.SetBeingMined(false);
//                 Debug.Log("Mine empty!");
//                 OnFlag?.Invoke(BaseAgentsFlags.OnMineEmpty);
//             }
//             else if (miner.GetEnergy() <= 0)
//             {
//                 Debug.Log("Food needed! Gold mined: " + goldCount);
//                 OnFlag?.Invoke(BaseAgentsFlags.OnFoodNeed);
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
//         miner = parameters[0] as Miner;
//
//         behaviours.AddMultithreadbleBehaviours(0, () =>
//         {
//             miner.SetStartNode(miner.GetCurrentNode());
//             mine = miner.GetClosestGoldMineNode(miner.GetStartNode());
//
//             if (mine == null)
//             {
//                 Debug.Log("Mine is null in MineGoldState because no mine with gold was found");
//                 noMoreMines = true;
//             }
//
//             if (miner == null)
//                 Debug.Log("Miner is null in MineGoldState");
//         });
//
//         return behaviours;
//     }
//
//     public override BehavioursActions GetOnExitBehaviour(params object[] parameters)
//     {
//         return default;
//     }
// }
// }