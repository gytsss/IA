using System;
using GeneticAlgGame.Agents;
using MinersGame.FSM.States;
using StateMachine.Agents.Simulation;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace GeneticAlgGame.FSMStates
{
   public class SimulationWalkState : State
    {
        public override BehaviourActions GetTickBehaviour(params object[] parameters)
        {
            var behaviours = new BehaviourActions();

            var currentNode = parameters[0] as SimNode<Vector2>;
            var targetNode = parameters[1] as Node<Vector2>;
            var position = (Transform)parameters[2];
            var foodTarget = (SimNodeType)parameters[3];
            var onMove = parameters[4] as Action;
            var outputBrain1 = (float[])parameters[5];
            var outputBrain2 = (float[])parameters[6];

            behaviours.AddMultiThreadableBehaviours(0, () =>
            {
                onMove?.Invoke();
                
            });

            behaviours.AddMainThreadBehaviours(1, () =>
            {
                if (currentNode == null) return;

                position.position = new Vector3(currentNode.GetCoordinate().X, currentNode.GetCoordinate().Y);
            });

            behaviours.SetTransitionBehaviour(() =>
            {
                if(outputBrain1[0] > 0.5f && currentNode != null && currentNode.NodeType == foodTarget) OnFlag?.Invoke(SimAgent.Flags.OnEat);
                if(outputBrain1[1] > 0.5f) OnFlag?.Invoke(SimAgent.Flags.OnSearchFood);
                SpecialAction(outputBrain2);
                
            });
            return behaviours;
        }

        protected virtual void SpecialAction(float[] outputs)
        {
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
    
    public class SimulationWalkHerbState : SimulationWalkState
    {
        protected override void SpecialAction(float[] outputs)
        {
            if(outputs[0] > 0.5f) OnFlag?.Invoke(SimAgent.Flags.OnEscape);
        }
    }
    
    public class SimulationWalkCarnState : SimulationWalkState
    {
        protected override void SpecialAction(float[] outputs)
        {
            if(outputs[0] > 0.5f) OnFlag?.Invoke(SimAgent.Flags.OnAttack);
        }
    }
}