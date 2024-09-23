using System;
using Pathfinder;
using UnityEngine;

namespace Game.FSM.States
{
   public sealed class DepositFoodState : State
{
    private GoldMineNode<Vec2Int> mine;
    private bool alreadyDeposited = false;
    private bool backToUrbanCenter = false;

    public override BehavioursActions GetTickBehaviour(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        BaseAgent agent = parameters[0] as BaseAgent;
        GameManager gameManager = parameters[1] as GameManager;
        int food = Convert.ToInt32(parameters[2]);

        behaviours.AddMultithreadbleBehaviours(0, () =>
        {
            if (mine == null)
            {
                Debug.Log("No mines being mined");
                if (agent.GetCurrentNode() != gameManager.GetUrbanCenterNode())
                    backToUrbanCenter = true;
            }

            if (agent == null)
                Debug.Log("Agent is null in DepositFoodState.");
        });

        behaviours.AddMainThreadBehaviour(0, () =>
        {
            mine = gameManager.goldMineManager.FindClosestGoldMineBeingMined(gameManager.GetUrbanCenterNode());

            if (mine != null && !alreadyDeposited)
            {
                mine.SetFoodAmount(mine.GetFoodAmount() + food);
                Debug.Log("Food deposited! Food amount: " + mine.GetFoodAmount());
                alreadyDeposited = true;
            }
        });

        behaviours.SetTransitionBehavior(() =>
        {
            if (alreadyDeposited)
            {
                alreadyDeposited = false;
                OnFlag?.Invoke(Flags.OnFoodDeposit);
            }
            else if (mine == null && backToUrbanCenter)
            {
                backToUrbanCenter = false;
                OnFlag?.Invoke(Flags.OnNoMoreMines);
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