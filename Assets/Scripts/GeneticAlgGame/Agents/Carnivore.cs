﻿using System;
using GeneticAlgGame.FSMStates;
using NeuralNetworkDirectory.ECS;
using NeuralNetworkDirectory.NeuralNet;
using Pathfinder;
using StateMachine.Agents.Simulation;
using StateMachine.States.SimStates;
using Utils;

namespace GeneticAlgGame.Agents
{
    public class Carnivore<TVector, TTransform> : SimAgent<TVector, TTransform>
        where TTransform : ITransform<IVector>, new()
        where TVector : IVector, IEquatable<TVector>
    {
        public Action OnAttack { get; set; }
        public bool HasAttacked { get; private set; }
        public bool HasKilled { get; private set; }

        public int DamageDealt { get; private set; } = 0;

        public override void Init()
        {
            base.Init();
            foodTarget = SimNodeType.Corpse;
            FoodLimit = 1;
            movement = 2;

            CalculateInputs();
            OnAttack += Attack;
        }

        public override void Reset()
        {
            base.Reset();
            HasAttacked = false;
            HasKilled = false;
            DamageDealt = 0;
        }

        protected override void ExtraInputs()
        {
            int brain = GetBrainTypeKeyByValue(BrainType.Attack);
            var inputCount = GetInputCount(BrainType.Attack);
            input[brain] = new float[inputCount];

            input[brain][0] = CurrentNode.GetCoordinate().X;
            input[brain][1] = CurrentNode.GetCoordinate().Y;
            SimAgent<IVector, ITransform<IVector>> target =
                EcsPopulationManager.GetNearestEntity(SimAgentTypes.Herbivore, Transform.position);
            if (target == null)
            {
                input[brain][2] = NoTarget;
                input[brain][3] = NoTarget;
                return;
            }

            input[brain][2] = target.CurrentNode.GetCoordinate().X;
            input[brain][3] = target.CurrentNode.GetCoordinate().Y;
        }

        protected override void MovementInputs()
        {
            int brain = GetBrainTypeKeyByValue(BrainType.Movement);
            var inputCount = GetInputCount(BrainType.Movement);

            input[brain] = new float[inputCount];
            input[brain][0] = CurrentNode.GetCoordinate().X;
            input[brain][1] = CurrentNode.GetCoordinate().Y;
            SimAgent<IVector, ITransform<IVector>> target =
                EcsPopulationManager.GetNearestEntity(SimAgentTypes.Herbivore, Transform.position);
            INode<IVector> nodeTarget = GetTarget(foodTarget);


            if (target == null)
            {
                input[brain][2] = NoTarget;
                input[brain][3] = NoTarget;
            }
            else
            {
                input[brain][2] = target.CurrentNode.GetCoordinate().X;
                input[brain][3] = target.CurrentNode.GetCoordinate().Y;
            }

            if (nodeTarget == null)
            {
                input[brain][4] = NoTarget;
                input[brain][5] = NoTarget;
            }
            else
            {
                input[brain][4] = nodeTarget.GetCoordinate().X;
                input[brain][5] = nodeTarget.GetCoordinate().Y;
            }

            input[brain][6] = Food;
        }

        protected override void ExtraBehaviours()
        {
            Fsm.AddBehaviour<SimEatCarnState>(Behaviours.Eat, EatTickParameters);

            Fsm.AddBehaviour<SimWalkCarnState>(Behaviours.Walk, WalkTickParameters);

            Fsm.AddBehaviour<SimAttackState>(Behaviours.Attack, AttackEnterParameters);
        }

        private object[] AttackEnterParameters()
        {
            object[] objects =
            {
                OnAttack, 
                output[GetBrainTypeKeyByValue(BrainType.Eat)],
                output[GetBrainTypeKeyByValue(BrainType.Attack)], 
                output[GetBrainTypeKeyByValue(BrainType.Movement)][2]
            };
            return objects;
        }


        private void Attack()
        {
            SimAgent<IVector, ITransform<IVector>> target =
                EcsPopulationManager.GetEntity(SimAgentTypes.Herbivore, CurrentNode);
            if (target is not Herbivore<TVector, TTransform> herbivore ||
                !Approximatly(herbivore.Transform.position, transform.position, 0.1f)) return;

            herbivore.Hp--;
            HasAttacked = true;
            DamageDealt++;
            if (herbivore.Hp <= 0)
            {
                HasKilled = true;
            }
        }

        private bool Approximatly(IVector coord1, IVector coord2, float tolerance)
        {
            return Math.Abs(coord1.X - coord2.X) <= tolerance && Math.Abs(coord1.Y - coord2.Y) <= tolerance;
        }

        protected override void EatTransitions()
        {
            Fsm.SetTransition(Behaviours.Eat, Flags.OnEat, Behaviours.Eat);
            Fsm.SetTransition(Behaviours.Eat, Flags.OnSearchFood, Behaviours.Walk);
            Fsm.SetTransition(Behaviours.Eat, Flags.OnAttack, Behaviours.Attack);
        }

        protected override void WalkTransitions()
        {
            Fsm.SetTransition(Behaviours.Walk, Flags.OnEat, Behaviours.Eat);
            Fsm.SetTransition(Behaviours.Walk, Flags.OnAttack, Behaviours.Attack);
        }

        protected override void ExtraTransitions()
        {
            Fsm.SetTransition(Behaviours.Attack, Flags.OnAttack, Behaviours.Attack);
            Fsm.SetTransition(Behaviours.Attack, Flags.OnEat, Behaviours.Eat);
            Fsm.SetTransition(Behaviours.Attack, Flags.OnSearchFood, Behaviours.Walk);
        }
    }
}