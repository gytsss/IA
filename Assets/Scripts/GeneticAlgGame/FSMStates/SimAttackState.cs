﻿using System;
using FSM;
using StateMachine.Agents.Simulation;

namespace GeneticAlgGame.FSMStates
{
    public class SimAttackState : State
    {
        public override BehaviourActions GetTickBehaviour(params object[] parameters)
        {
            if (parameters == null || parameters.Length < 9)
            {
                return default;
            }
            
            var behaviours = new BehaviourActions();

            var onAttack = parameters[5] as Action;
            var outputBrain1 = (float[])parameters[6];
            var outputBrain2 = (float[])parameters[7];
            var outputBrain3 = (float)parameters[8];

            if (outputBrain1 == null || outputBrain2 == null)
            {
                return default;
            }
            
            behaviours.AddMultiThreadableBehaviours(0, () =>
            {
                onAttack?.Invoke();
            });

            behaviours.SetTransitionBehaviour(() =>
            {
                if(outputBrain1[0] > 0.5f) OnFlag?.Invoke(Flags.OnEat);
                if(outputBrain2[0] > 0.5f) OnFlag?.Invoke(Flags.OnAttack);
                if(outputBrain3 > 0.5f) OnFlag?.Invoke(Flags.OnSearchFood);
            });
            return behaviours;
        }
        
        public override BehaviourActions GetOnEnterBehaviour(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetOnExitBehaviour(params object[] parameters)
        {
            return default;
        }
    }
}