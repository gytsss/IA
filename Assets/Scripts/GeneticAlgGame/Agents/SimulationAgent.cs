using System;
using DefaultNamespace;
using NeuralNetworkDirectory.NeuralNet;
using UnityEngine;

namespace GeneticAlgGame.Agents
{
    public enum SimulationAgentTypes
    {
        Carnivorous,
        Herbivore,
        Scavenger
    }
    public class SimulationAgent : MonoBehaviour
    {
        
        public enum Behaviours
        {
            Walk,
            Escape,
            Eat,
            Attack
        }

        public enum Flags
        {
            OnTargetLost,
            OnEscape,
            OnEat,
            OnSearchFood,
            OnAttack
        }

        public static Graph<SimulationNode<Vector2>, NodeVoronoi, Vector2> graph;
        public NodeVoronoi CurrentNode;
        public bool CanReproduce() => Food >= FoodLimit;
        public SimulationAgentTypes agentType { get; protected set; }
        public Boid boid;

        protected int movement = 3;
        protected SimNodeType foodTarget;
        protected int FoodLimit = 5;
        protected int Food = 0;
        protected FSM<Behaviours, Flags> Fsm;
        protected Action OnMove;
        protected Action OnEat;

        protected SimulationNode<Vector2> TargetNode
        {
            get => targetNode;
            set => targetNode = value;
        }

        private SimulationNode<Vector2> targetNode;
        public float[][] output;
        public float[][] input;
        public BrainType[] brainTypes;

        public virtual void Init()
        {
            Fsm = new FSM<Behaviours, Flags>();

            OnMove += Move;
            OnEat += Eat;

            FsmBehaviours();

            FsmTransitions();

            UpdateInputs();
        }

        public virtual void Uninit()
        {
            OnMove -= Move;
            OnEat -= Eat;
        }

        public void Tick()
        {
            Fsm.Tick();
            UpdateInputs();
        }

        protected virtual void UpdateInputs()
        {
            FindFoodInputs();
            ExtraInputs();
            MovementInputs();
        }


        private void FindFoodInputs()
        {
            int brain = (int)BrainType.Eat;
            input[brain][0] = CurrentNode.GetCoordinate().x;
            input[brain][1] = CurrentNode.GetCoordinate().y;
            SimulationNode<Vector2> target = GetTarget(foodTarget);
            input[brain][2] = target.GetCoordinate().x;
            input[brain][3] = target.GetCoordinate().y;
        }

        protected virtual void MovementInputs()
        {
        }

        protected virtual void ExtraInputs()
        {
        }

        protected virtual void FsmTransitions()
        {
            WalkTransitions();
            EatTransitions();
            ExtraTransitions();
        }

        protected virtual void WalkTransitions()
        {
        }

        protected virtual void EatTransitions()
        {
        }

        protected virtual void ExtraTransitions()
        {
        }

        protected virtual void FsmBehaviours()
        {
            Fsm.AddBehaviour<SimWalkState>(Behaviours.Walk, WalkTickParameters);
            ExtraBehaviours();
        }

        protected virtual void ExtraBehaviours()
        {
        }

        protected virtual object[] WalkTickParameters()
        {
            int extraBrain = agentType == SimulationAgentTypes.Carnivorous ? (int)BrainType.Attack : (int)BrainType.Escape;
            object[] objects =
            {
                CurrentNode, TargetNode, transform, foodTarget, OnMove, output[(int)BrainType.Movement],
                output[extraBrain]
            };
            return objects;
        }

        protected virtual object[] WalkEnterParameters()
        {
            object[] objects = { };
            return objects;
        }


        protected virtual object[] EatTickParameters()
        {
            object[] objects = { CurrentNode, foodTarget, OnEat, output[0], output[1] };
            return objects;
        }

        private void Eat() => Food++;

        protected virtual void Move()
        {
            if (CurrentNode == null || TargetNode == null) return;

            if (CurrentNode.GetCoordinate().Equals(TargetNode.GetCoordinate())) return;

            int brain = (int)BrainType.Movement;
            var targetPos = CurrentNode.GetCoordinate();
            float speed = CalculateSpeed(output[brain][2]);

            targetPos = CalculateNewPosition(targetPos, output[brain], speed);

            if (targetPos != Vector2.zero) CurrentNode = GetNode(targetPos);
        }

        private float CalculateSpeed(float rawSpeed)
        {
            if (rawSpeed < 1) return movement;
            if (rawSpeed < 0) return movement - 1;
            if (rawSpeed < -0.6) return movement - 2;
            return rawSpeed;
        }

        private Vector2 CalculateNewPosition(Vector2 targetPos, float[] brainOutput, float speed)
        {
            if (brainOutput[0] > 0)
            {
                if (brainOutput[1] > 0.1) // Right
                {
                    targetPos.x += speed;
                }
                else if (brainOutput[1] < -0.1) // Left
                {
                    targetPos.x -= speed;
                }
            }
            else
            {
                if (brainOutput[1] > 0.1) // Up
                {
                    targetPos.y += speed;
                }
                else if (brainOutput[1] < -0.1) // Down
                {
                    targetPos.y -= speed;
                }
            }

            return targetPos;
        }
        protected virtual SimulationNode<Vector2> GetTarget(SimNodeType nodeType = SimNodeType.Empty)
        {
            Vector2 position = transform.position;
            SimulationNode<Vector2> nearestNode = null;
            float minDistance = float.MaxValue;

            foreach (var node in graph.NodesType)
            {
                if (node.NodeType != nodeType) continue;
                float distance = Vector2.Distance(position, node.GetCoordinate());
                if (!(distance < minDistance)) continue;

                minDistance = distance;
                nearestNode = node;
            }

            return nearestNode;
        }

        protected virtual NodeVoronoi GetNode(Vector2 position)
        {
            return graph.CoordNodes[(int)position.x, (int)position.y];
        }
    
    }
}