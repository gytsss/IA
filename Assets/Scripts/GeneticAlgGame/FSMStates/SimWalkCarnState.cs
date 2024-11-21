using System;
using FSM;
using GeneticAlgGame.Graph;
using StateMachine.Agents.Simulation;
using Utils;

namespace GeneticAlgGame.FSMStates
{
    public class SimWalkCarnState : State
    {
        public override BehaviourActions GetTickBehaviour(params object[] parameters)
        {
            var behaviours = new BehaviourActions();

            var currentNode = parameters[0] as SimNode<IVector>;
            var foodTarget = (SimNodeType)parameters[1];
            var onMove = parameters[2] as Action;
            var outputBrain1 = (float[])parameters[3];
            var outputBrain2 = (float[])parameters[4];

            behaviours.AddMultiThreadableBehaviours(0, () =>
            {
                onMove?.Invoke();
            });

            
            //behaviours.AddMainThreadBehaviours(1, () =>
            //{
            //    if (currentNode == null) return;

            //    position.position = new Vector3(currentNode.GetCoordinate().x, currentNode.GetCoordinate().y);
            //});

            behaviours.SetTransitionBehaviour(() =>
            {
                if (outputBrain1[0] > 0.5f && currentNode != null && currentNode.NodeType == foodTarget) OnFlag?.Invoke(Flags.OnEat);
                if (outputBrain2[0] > 0.5f) OnFlag?.Invoke(Flags.OnAttack);
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