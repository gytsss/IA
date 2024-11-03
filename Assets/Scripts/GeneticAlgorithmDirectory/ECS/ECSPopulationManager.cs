﻿using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ECS.Patron;
using Flocking;
using GeneticAlgGame.Agents;
using GeneticAlgorithmDirectory.ECS;
using NeuralNetworkDirectory.DataManagement;
using NeuralNetworkDirectory.NeuralNet;
using Pathfinder;
using StateMachine.Agents.Simulation;

namespace NeuralNetworkDirectory.ECS
{
    public class EcsPopulationManager : MonoBehaviour
    {
        public int entityCount = 100;
        public GameObject prefab;

        private Dictionary<uint, GameObject> entities;
        private static Dictionary<uint, SimulationAgent> agents;

        private void Start()
        {
            ECSManager.Init();
            entities = new Dictionary<uint, GameObject>();
            for (var i = 0; i < entityCount; i++)
            {
                var entityID = ECSManager.CreateEntity();
                ECSManager.AddComponent(entityID, new InputComponent());
                ECSManager.AddComponent(entityID, new NeuralNetComponent());
                ECSManager.AddComponent(entityID, new OutputComponent());
            }
        }

        private void Update()
        {
            ECSManager.Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            Parallel.ForEach(entities, entity =>
            {
                var outputComponent = ECSManager.GetComponent<OutputComponent>(entity.Key);
                var boid = agents[entity.Key].boid;

                if (boid)
                {
                    UpdateBoidOffsets(boid, outputComponent.outputs[(int)BrainType.Flocking]);
                }
            });

            Parallel.ForEach(entities, entity =>
            {
                ECSManager.GetComponent<InputComponent>(entity.Key).inputs = agents[entity.Key].input;

                agents[entity.Key].output = ECSManager.GetComponent<OutputComponent>(entity.Key).outputs;

                agents[entity.Key].Tick();
            });
        }

        private void UpdateBoidOffsets(Boid boid, float[] outputs)
        {
            boid.cohesionOffset = outputs[0];
            boid.separationOffset = outputs[1];
            boid.directionOffset = outputs[2];
            boid.alignmentOffset = outputs[3];
        }

        public void Save(string directoryPath, int generation)
        {
            var agentsData = new List<AgentNeuronData>();

            Parallel.ForEach(entities, entity =>
            {
                var netComponent = ECSManager.GetComponent<NeuralNetComponent>(entity.Key);
                for (int i = 0; i < netComponent.Layers.Count; i++)
                {
                    for (int j = 0; j < netComponent.Layers[i].Count; j++)
                    {
                        var layer = netComponent.Layers[i][j];
                        var neuronData = new AgentNeuronData
                        {
                            AgentType = layer.AgentType,
                            BrainType = layer.BrainType,
                            TotalWeights = layer.GetWeights().Length,
                            Bias = layer.Bias,
                            NeuronWeights = layer.GetWeights(),
                            Fitness = netComponent.Fitness[i]
                        };
                        agentsData.Add(neuronData);
                    }
                }

                NeuronDataSystem.SaveNeurons(agentsData, directoryPath, generation);
            });
        }

        public void Load(string directoryPath)
        {
            var loadedData = NeuronDataSystem.LoadLatestNeurons(directoryPath);

            Parallel.ForEach(entities, entity =>
            {
                var netComponent = ECSManager.GetComponent<NeuralNetComponent>(entity.Key);
                var agent = agents[entity.Key];

                if (loadedData.TryGetValue(agent.agentType, out var brainData))
                {
                    foreach (var brainType in agent.brainTypes)
                    {
                        if (brainData.TryGetValue(brainType, out var neuronDataList))
                        {
                            foreach (var neuronData in neuronDataList)
                            {
                                for (int i = 0; i < netComponent.Layers.Count; i++)
                                {
                                    for (int j = 0; j < netComponent.Layers[i].Count; j++)
                                    {
                                        var layer = netComponent.Layers[i][j];
                                        layer.AgentType = neuronData.AgentType;
                                        layer.BrainType = neuronData.BrainType;
                                        layer.Bias = neuronData.Bias;
                                        layer.SetWeights(neuronData.NeuronWeights, 0);
                                    }
                                }

                                netComponent.Fitness = new float[] { neuronData.Fitness };
                            }
                        }
                    }
                }
            });
        }
        public static SimulationAgent GetNearestEntity(SimulationAgentTypes entityType, NodeVoronoi position)
        {
            SimulationAgent nearestAgent = null;
            float minDistance = float.MaxValue;

            foreach (var agent in agents.Values)
            {
                if (agent.agentType != entityType) continue;

                float distance = Vector2.Distance(position.GetCoordinate(), agent.CurrentNode.GetCoordinate());

                if (minDistance > distance) continue;

                minDistance = distance;
                nearestAgent = agent;
            }

            return nearestAgent;
        }

        public static SimulationAgent GetEntity(SimulationAgentTypes entityType, SimNode<Vector2> position)
        {
            SimulationAgent target = null;

            foreach (var agent in agents.Values)
            {
                if (agent.agentType != entityType) continue;

                if (!position.GetCoordinate().Equals(agent.CurrentNode.GetCoordinate())) continue;

                target = agent;
                break;
            }

            return target;
        }

        public static SimulationAgent GetEntity(SimulationAgentTypes entityType, NodeVoronoi position)
        {
            SimulationAgent target = null;

            foreach (var agent in agents.Values)
            {
                if (agent.agentType != entityType) continue;

                if (!position.GetCoordinate().Equals(agent.CurrentNode.GetCoordinate())) continue;

                target = agent;
                break;
            }

            return target;
        }

        public static SimNode<Vector2> CoordinateToNode(NodeVoronoi coordinate)
        {
            return SimulationAgent.graph.NodesType
                .FirstOrDefault(node => node.GetCoordinate().Equals(coordinate.GetCoordinate()));
        }
    }
}