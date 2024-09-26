using Game.Agents;
using Game.Nodes;
using UnityEngine;

namespace Game.FSM.States
{
   public sealed class EatFoodState : State
   {
       private GoldMineNode<Vec2Int> mine;
   
       public override BehavioursActions GetTickBehaviour(params object[] parameters)
       {
           BehavioursActions behaviours = new BehavioursActions();
   
           Miner miner = parameters[0] as Miner;
   
   
           behaviours.AddMultithreadbleBehaviours(0, () =>
           {
               mine = miner.GetClosestGoldMineNode(miner.GetStartNode());
   
               if (miner == null || mine == null)
                   Debug.Log("Miner or mine is null in EatFoodState");
           });
   
           behaviours.AddMainThreadBehaviour(0, () =>
           {
               if (mine != null && mine.HasFood())
               {
                   mine.ConsumeFood();
                   Debug.Log("Food consumed! Food left: " + mine.GetFoodAmount());
   
                   miner.ResetEnergy();
               }
               else
               {
                   Debug.Log("No food available!");
               }
           });
   
           behaviours.SetTransitionBehavior(() =>
           {
               if (miner.GetEnergy() >= miner.GetMaxEnergy())
               {
                   Debug.Log("Back to work! Food left: " + mine.GetFoodAmount());
                   OnFlag?.Invoke(Flags.OnFoodEaten);
               }
               else if (!mine.HasFood())
               {
                   Debug.Log("Mine empty of food!");
                   OnFlag?.Invoke(Flags.OnMineEmptyOfFood);
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