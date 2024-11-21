using MinersGame.FSM.States;
using UnityEngine;

namespace Game.FSM.States
{
   public sealed class EatFoodState : State
   {
       private GoldMineNode<Vector2> mine;
   
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
   
           behaviours.AddMultithreadbleBehaviours(0, () =>
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
                   OnFlag?.Invoke(BaseAgentsFlags.OnFoodEaten);
               }
               else if (!mine.HasFood())
               {
                   Debug.Log("Mine empty of food!");
                   OnFlag?.Invoke(BaseAgentsFlags.OnMineEmptyOfFood);
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