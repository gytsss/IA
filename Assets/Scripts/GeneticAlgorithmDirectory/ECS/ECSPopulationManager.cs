﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECS.Patron;
using Flocking;
using GeneticAlgGame.Agents;
using GeneticAlgGame.Graph;
using GeneticAlgorithmDirectory.AI;
using GeneticAlgorithmDirectory.DataManagement;
using GeneticAlgorithmDirectory.GeneticAlg;
using NeuralNetworkDirectory.NeuralNet;
using StateMachine.Agents.Simulation;
using UnityEngine;
using Utils;
using Voronoi;

namespace GeneticAlgorithmDirectory.ECS
{
    using SimAgentType = SimAgent<IVector, ITransform<IVector>>;
    using SimBoid = Boid<IVector, ITransform<IVector>>;

    public class EcsPopulationManager : MonoBehaviour
    {
        public struct NeuronInputCount
        {
            public SimAgentTypes agentType;
            public BrainType brainType;
            public int inputCount;
            public int outputCount;
            public int[] hiddenLayersInputs;
        }

        #region Variables

        [SerializeField] private Mesh carnivoreMesh;
        [SerializeField] private Material carnivoreMat;
        [SerializeField] private Mesh herbivoreMesh;
        [SerializeField] private Material herbivoreMat;
        [SerializeField] private Mesh scavengerMesh;
        [SerializeField] private Material scavengerMat;


        [SerializeField] private int carnivoreCount = 10;
        [SerializeField] private int herbivoreCount = 20;
        [SerializeField] private int scavengerCount = 10;

        [SerializeField] private int eliteCount = 4;
        [SerializeField] private float generationDuration = 20.0f;
        [SerializeField] private float mutationChance = 0.10f;
        [SerializeField] private float mutationRate = 0.01f;
        [SerializeField] public int Generation;

        public int gridWidth = 10;
        public int gridHeight = 10;
        public int generationTurns = 100;
        public float speed = 1.0f;
        public static Sim2Graph graph;
        public static NeuronInputCount[] inputCounts;
        public static Dictionary<(BrainType, SimAgentTypes), NeuronInputCount> InputCountCache;
        public static FlockingManager flockingManager = new();

        private const float Bias = 0.0f;
        private const float SigmoidP = .5f;
        private bool isRunning = true;
        private static int missingHerbivores;
        private static int missingCarnivores;
        private static int missingScavengers;
        private int plantCount;
        private int currentTurn;
        private int behaviourCount;
        private const int CellSize = 1;
        private float accumTime;
        private GeneticAlgorithm genAlg;
        private GraphManager<IVector, ITransform<IVector>> gridManager;
        private FitnessManager<IVector, ITransform<IVector>> fitnessManager;
        private static Dictionary<uint, SimAgentType> _agents = new();
        private static Dictionary<uint, Dictionary<BrainType, List<Genome>>> _population = new();
        private static Dictionary<uint, Scavenger<IVector, ITransform<IVector>>> _scavengers = new();
        private static Dictionary<int, BrainType> herbBrainTypes = new();
        private static Dictionary<int, BrainType> scavBrainTypes = new();
        private static Dictionary<int, BrainType> carnBrainTypes = new();
        private static readonly int BrainsAmount = Enum.GetValues(typeof(BrainType)).Length;

        private ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 32
        };

        #endregion

        private void Awake()
        {
            herbBrainTypes[0] = BrainType.Eat;
            herbBrainTypes[1] = BrainType.Movement;
            herbBrainTypes[2] = BrainType.Escape;

            scavBrainTypes[0] = BrainType.Eat;
            scavBrainTypes[1] = BrainType.ScavengerMovement;
            scavBrainTypes[2] = BrainType.Flocking;

            carnBrainTypes[0] = BrainType.Eat;
            carnBrainTypes[1] = BrainType.Movement;
            carnBrainTypes[2] = BrainType.Attack;


            inputCounts = new[]
            {
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Carnivore, brainType = BrainType.Eat, inputCount = 4, outputCount = 1,
                    hiddenLayersInputs = new[] { 1 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Carnivore, brainType = BrainType.Movement, inputCount = 7,
                    outputCount = 3, hiddenLayersInputs = new[] { 3 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Carnivore, brainType = BrainType.Attack, inputCount = 4,
                    outputCount = 1, hiddenLayersInputs = new[] { 1 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Herbivore, brainType = BrainType.Eat, inputCount = 4, outputCount = 1,
                    hiddenLayersInputs = new[] { 1 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Herbivore, brainType = BrainType.Movement, inputCount = 8,
                    outputCount = 2, hiddenLayersInputs = new[] { 3 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Herbivore, brainType = BrainType.Escape, inputCount = 4, outputCount = 1,
                    hiddenLayersInputs = new[] { 1 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Scavenger, brainType = BrainType.Eat, inputCount = 4, outputCount = 1,
                    hiddenLayersInputs = new[] { 1 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Scavenger, brainType = BrainType.ScavengerMovement, inputCount = 7,
                    outputCount = 2, hiddenLayersInputs = new[] { 3 }
                },
                new NeuronInputCount
                {
                    agentType = SimAgentTypes.Scavenger, brainType = BrainType.Flocking, inputCount = 16,
                    outputCount = 4,
                    hiddenLayersInputs = new[] { 12, 8, 6, 4 }
                },
            };

            InputCountCache = inputCounts.ToDictionary(input => (input.brainType, input.agentType));
            ECSManager.Init();
            gridManager = new GraphManager<IVector, ITransform<IVector>>(gridWidth, gridHeight);
            graph = new Sim2Graph(gridWidth, gridHeight, CellSize);
            StartSimulation();
            plantCount = _agents.Values.Count(agent => agent.agentType == SimAgentTypes.Herbivore) * 2;
            InitializePlants();
            fitnessManager = new FitnessManager<IVector, ITransform<IVector>>(_agents);
            behaviourCount = GetHighestBehaviourCount();
        }

        private void Update()
        {
            foreach (var id in _agents.Keys)
            {
                var pos = _agents[id].Transform.position;
                Mesh mesh = new Mesh();
                Material material = herbivoreMat;
                switch (_agents[id].agentType)
                {
                    case SimAgentTypes.Carnivore:
                        mesh = carnivoreMesh;
                        material = carnivoreMat;
                        break;
                    case SimAgentTypes.Herbivore:
                        mesh = herbivoreMesh;
                        material = herbivoreMat;
                        break;
                    case SimAgentTypes.Scavenger:
                        mesh = scavengerMesh;
                        material = scavengerMat;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Vector3 position = new Vector3(pos.X, pos.Y);
                var renderParams = new RenderParams(material);

                Graphics.RenderMesh(renderParams, mesh, 0, Matrix4x4.Translate(position));
            }
        }

        private void FixedUpdate()
        {
            if (!isRunning)
                return;

            var dt = Time.fixedDeltaTime;


            for (int i = 0; i < Mathf.Clamp(speed / 100.0f * 50, 1, 50); i++)
            {
                EntitiesTurn(dt);

                accumTime += dt;
                if (!(accumTime >= generationDuration)) return;
                accumTime -= generationDuration;
                Epoch();
            }
        }

        private void EntitiesTurn(float dt)
        {
            var agentsCopy = _agents.ToArray();

            Parallel.ForEach(agentsCopy, parallelOptions, entity =>
            {
                entity.Value.UpdateInputs();
                var inputComponent = ECSManager.GetComponent<InputComponent>(entity.Key);
                if (inputComponent != null && _agents.TryGetValue(entity.Key, out var agent))
                {
                    inputComponent.inputs = agent.input;
                }
            });

            ECSManager.Tick(dt);

            Parallel.ForEach(agentsCopy, parallelOptions, entity =>
            {
                var outputComponent = ECSManager.GetComponent<OutputComponent>(entity.Key);
                if (outputComponent == null || !_agents.TryGetValue(entity.Key, out var agent)) return;

                agent.output = outputComponent.outputs;

                if (agent.agentType != SimAgentTypes.Scavenger) return;

                var boid = _scavengers[entity.Key]?.boid;

                if (boid != null)
                {
                    UpdateBoidOffsets(boid, outputComponent.outputs
                        [GetBrainTypeKeyByValue(BrainType.Flocking, SimAgentTypes.Scavenger)]);
                }
            });

            int batchSize = 10;
            for (int i = 0; i < behaviourCount; i++)
            {
                var i1 = i;
                var tasks = new List<Task>();

                for (int j = 0; j < agentsCopy.Length; j += batchSize)
                {
                    var batch = agentsCopy.Skip(j).Take(batchSize).ToArray();
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var entity in batch)
                        {
                            entity.Value.Fsm.MultiThreadTick(i1);
                        }
                    }));
                }

                foreach (var entity in agentsCopy)
                {
                    entity.Value.Fsm.MainThreadTick(i);
                }

                Task.WaitAll(tasks.ToArray());
            }

            fitnessManager.Tick();
        }

        private void UpdateBoidOffsets(SimBoid boid, float[] outputs)
        {
            boid.cohesionOffset = outputs[0];
            boid.separationOffset = outputs[1];
            boid.directionOffset = outputs[2];
            boid.alignmentOffset = outputs[3];
        }


        private void GenerateInitialPopulation()
        {
            DestroyAgents();

            CreateAgents(herbivoreCount, SimAgentTypes.Herbivore);
            CreateAgents(carnivoreCount, SimAgentTypes.Carnivore);
            CreateAgents(scavengerCount, SimAgentTypes.Scavenger);

            accumTime = 0.0f;
        }

        private void CreateAgents(int count, SimAgentTypes agentType)
        {
            for (var i = 0; i < count; i++)
            {
                var entityID = ECSManager.CreateEntity();
                var neuralNetComponent = new NeuralNetComponent();
                var inputComponent = new InputComponent();
                ECSManager.AddComponent(entityID, inputComponent);
                ECSManager.AddComponent(entityID, neuralNetComponent);

                var num = agentType switch
                {
                    SimAgentTypes.Carnivore => carnBrainTypes,
                    SimAgentTypes.Herbivore => herbBrainTypes,
                    SimAgentTypes.Scavenger => scavBrainTypes,
                    _ => throw new ArgumentException("Invalid agent type")
                };

                ECSManager.AddComponent(entityID, new OutputComponent(agentType, num));

                var brains = CreateBrain(agentType);
                var genomes = new Dictionary<BrainType, List<Genome>>();

                foreach (var brain in brains)
                {
                    BrainType brainType = BrainType.Movement;
                    var genome =
                        new Genome(brain.Layers.Sum(layerList => layerList.Sum(layer => layer.GetWeights().Length)));
                    foreach (var layerList in brain.Layers)
                    {
                        foreach (var layer in layerList)
                        {
                            brainType = layer.BrainType;
                            layer.SetWeights(genome.genome, 0);
                        }
                    }

                    if (!genomes.ContainsKey(brainType))
                    {
                        genomes[brainType] = new List<Genome>();
                    }

                    genomes[brainType].Add(genome);
                }

                inputComponent.inputs = new float[brains.Count][];
                neuralNetComponent.Layers = brains.SelectMany(brain => brain.Layers).ToList();
                neuralNetComponent.Fitness = new float[BrainsAmount];
                neuralNetComponent.FitnessMod = new float[BrainsAmount];

                for (int j = 0; j < neuralNetComponent.FitnessMod.Length; j++)
                {
                    neuralNetComponent.FitnessMod[j] = 1.0f;
                }

                var agent = CreateAgent(agentType);
                _agents[entityID] = agent;

                if (agentType == SimAgentTypes.Scavenger)
                {
                    _scavengers[entityID] = (Scavenger<IVector, ITransform<IVector>>)agent;
                }

                foreach (var brain in agent.brainTypes.Values)
                {
                    if (!_population.ContainsKey(entityID))
                    {
                        _population[entityID] = new Dictionary<BrainType, List<Genome>>();
                    }

                    _population[entityID][brain] = genomes[brain];
                }
            }
        }

        private SimAgentType CreateAgent(SimAgentTypes agentType)
        {
            var randomNode = gridManager.GetRandomPosition();

            SimAgentType agent;

            switch (agentType)
            {
                case SimAgentTypes.Carnivore:
                    agent = new Carnivore<IVector, ITransform<IVector>>();
                    agent.brainTypes = carnBrainTypes;
                    agent.agentType = SimAgentTypes.Carnivore;
                    break;
                case SimAgentTypes.Herbivore:
                    agent = new Herbivore<IVector, ITransform<IVector>>();
                    agent.brainTypes = herbBrainTypes;
                    agent.agentType = SimAgentTypes.Herbivore;
                    break;
                case SimAgentTypes.Scavenger:
                    agent = new Scavenger<IVector, ITransform<IVector>>();
                    agent.brainTypes = scavBrainTypes;
                    agent.agentType = SimAgentTypes.Scavenger;
                    break;
                default:
                    throw new ArgumentException("Invalid agent type");
            }

            agent.SetPosition(randomNode.GetCoordinate());
            agent.Init();

            if (agentType == SimAgentTypes.Scavenger)
            {
                var sca = (Scavenger<IVector, ITransform<IVector>>)agent;
                sca.boid.Init(flockingManager.Alignment, flockingManager.Cohesion, flockingManager.Separation,
                    flockingManager.Direction);
            }

            return agent;
        }


        private List<NeuralNetComponent> CreateBrain(SimAgentTypes agentType)
        {
            var brains = new List<NeuralNetComponent> { CreateSingleBrain(BrainType.Eat, agentType) };


            switch (agentType)
            {
                case SimAgentTypes.Herbivore:
                    brains.Add(CreateSingleBrain(BrainType.Movement, SimAgentTypes.Herbivore));
                    brains.Add(CreateSingleBrain(BrainType.Escape, SimAgentTypes.Herbivore));
                    break;
                case SimAgentTypes.Carnivore:
                    brains.Add(CreateSingleBrain(BrainType.Movement, SimAgentTypes.Carnivore));
                    brains.Add(CreateSingleBrain(BrainType.Attack, SimAgentTypes.Carnivore));
                    break;
                case SimAgentTypes.Scavenger:
                    brains.Add(CreateSingleBrain(BrainType.ScavengerMovement, SimAgentTypes.Scavenger));
                    brains.Add(CreateSingleBrain(BrainType.Flocking, SimAgentTypes.Scavenger));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(agentType), agentType,
                        "Not prepared for this agent type");
            }

            return brains;
        }

        // TODO - Refactor this method
        private NeuralNetComponent CreateSingleBrain(BrainType brainType, SimAgentTypes agentType)
        {
            var neuralNetComponent = new NeuralNetComponent();
            neuralNetComponent.Layers.Add(CreateNeuronLayerList(brainType, agentType));
            return neuralNetComponent;
        }


        private List<NeuronLayer> CreateNeuronLayerList(BrainType brainType, SimAgentTypes agentType)
        {
            if (!InputCountCache.TryGetValue((brainType, agentType), out var inputCount))
            {
                throw new ArgumentException("Invalid brainType or agentType");
            }

            var layers = new List<NeuronLayer>
            {
                new(inputCount.inputCount, inputCount.inputCount, Bias, SigmoidP)
                    { BrainType = brainType, AgentType = agentType }
            };

            foreach (int hiddenLayerInput in inputCount.hiddenLayersInputs)
            {
                layers.Add(new NeuronLayer(layers[^1].OutputsCount, hiddenLayerInput, Bias, SigmoidP)
                    { BrainType = brainType, AgentType = agentType });
            }

            layers.Add(new NeuronLayer(layers[^1].OutputsCount, inputCount.outputCount, Bias, SigmoidP)
                { BrainType = brainType, AgentType = agentType });

            return layers;
        }

        private void DestroyAgents()
        {
            _population.Clear();
        }

        private void Epoch()
        {
            Generation++;

            PurgingSpecials();
            bool remainingPopulation = _agents.Count > 0;
            ECSManager.GetSystem<NeuralNetSystem>().Deinitialize();
            if (Generation % 5 == 0) Save("NeuronData", Generation);

            if (remainingPopulation)
            {
                foreach (var agent in _agents.Values)
                {
                    Debug.Log(agent.agentType + " survived.");
                }
            }

            CleanMap();
            InitializePlants();


            if (!remainingPopulation)
            {
                FillPopulation();

                _population.Clear();
                return;
            }
            
            Dictionary<SimAgentTypes, Dictionary<BrainType, Genome[]>> genomes = new()
            {
                [SimAgentTypes.Scavenger] = new Dictionary<BrainType, Genome[]>(),
                [SimAgentTypes.Herbivore] = new Dictionary<BrainType, Genome[]>(),
                [SimAgentTypes.Carnivore] = new Dictionary<BrainType, Genome[]>()
            };
            Dictionary<SimAgentTypes, Dictionary<BrainType, int>> indexes = new()
            {
                [SimAgentTypes.Scavenger] = new Dictionary<BrainType, int>(),
                [SimAgentTypes.Herbivore] = new Dictionary<BrainType, int>(),
                [SimAgentTypes.Carnivore] = new Dictionary<BrainType, int>()
            };
            

            CreateNewGenomes(genomes);
            FillPopulation();
            BrainsHandler(indexes, genomes);
            
            genomes.Clear();
            indexes.Clear();
        }

        private void BrainsHandler(Dictionary<SimAgentTypes, Dictionary<BrainType, int>> indexes,
            Dictionary<SimAgentTypes, Dictionary<BrainType, Genome[]>> genomes)
        {
            foreach (KeyValuePair<uint, SimAgentType> agent in _agents)
            {
                SimAgentTypes agentType = agent.Value.agentType;
                NeuralNetComponent neuralNetComponent = ECSManager.GetComponent<NeuralNetComponent>(agent.Key);

                foreach (var brain in agent.Value.brainTypes.Values)
                {
                    int brainId = agent.Value.GetBrainTypeKeyByValue(brain);
                    if (!indexes[agentType].ContainsKey(brain))
                    {
                        indexes[agentType][brain] = 0;
                    }

                    int index = indexes[agentType][brain]++;
                    if (!_population.ContainsKey(agent.Key))
                    {
                        _population[agent.Key] = new Dictionary<BrainType, List<Genome>>();
                    }

                    if (!_population[agent.Key].ContainsKey(brain))
                    {
                        _population[agent.Key][brain] = new List<Genome>();
                    }

                    if (index >= genomes[agentType][brain].Length) continue;

                    neuralNetComponent.SetWeights(brainId, genomes[agentType][brain][index].genome);
                    _population[agent.Key][brain].Add(genomes[agentType][brain][index]);
                    agent.Value.Transform = new ITransform<IVector>(new MyVector(
                        gridManager.GetRandomPosition().GetCoordinate().X,
                        gridManager.GetRandomPosition().GetCoordinate().Y));
                    agent.Value.Reset();

                    if (!_agents.ContainsKey(agent.Key))
                    {
                        CreateAgent(agentType);
                    }
                }
            }
        }

        private void FillPopulation()
        {
            CreateAgents(missingHerbivores, SimAgentTypes.Herbivore);
            CreateAgents(missingCarnivores, SimAgentTypes.Carnivore);
            CreateAgents(missingScavengers, SimAgentTypes.Scavenger);
            missingCarnivores = 0;
            missingHerbivores = 0;
            missingScavengers = 0;
        }

        private void CreateNewGenomes(Dictionary<SimAgentTypes, Dictionary<BrainType, Genome[]>> genomes)
        {
            foreach (var brain in scavBrainTypes.Values)
            {
                genomes[SimAgentTypes.Scavenger][brain] =
                    genAlg.Epoch(GetGenomesByBrainAndAgentType(SimAgentTypes.Scavenger, brain).ToArray());
            }

            foreach (var brain in herbBrainTypes.Values)
            {
                genomes[SimAgentTypes.Herbivore][brain] =
                    genAlg.Epoch(GetGenomesByBrainAndAgentType(SimAgentTypes.Herbivore, brain).ToArray());
            }

            foreach (var brain in carnBrainTypes.Values)
            {
                genomes[SimAgentTypes.Carnivore][brain] =
                    genAlg.Epoch(GetGenomesByBrainAndAgentType(SimAgentTypes.Carnivore, brain).ToArray());
            }
        }

        private List<Genome> GetGenomesByBrainAndAgentType(SimAgentTypes agentType, BrainType brainType)
        {
            var genomes = new List<Genome>();

            foreach (var agentEntry in _population)
            {
                var agentId = agentEntry.Key;
                var brainDict = agentEntry.Value;

                if (_agents[agentId].agentType != agentType ||
                    !brainDict.TryGetValue(brainType, out var value)) continue;

                genomes.AddRange(value);
                genomes[^1].fitness = ECSManager.GetComponent<NeuralNetComponent>(agentId).Fitness[_agents[agentId]
                    .GetBrainTypeKeyByValue(brainType)];
            }

            return genomes;
        }

        private void InitializePlants()
        {
            for (int i = 0; i < plantCount; i++)
            {
                var plantPosition = gridManager.GetRandomPosition();
                plantPosition.NodeType = SimNodeType.Bush;
                plantPosition.Food = 5;
            }
        }

        private void CleanMap()
        {
            foreach (var node in graph.NodesType)
            {
                node.Food = 0;
                node.NodeType = SimNodeType.Empty;
            }
        }

        private void Save(string directoryPath, int generation)
        {
            /*
            var agentsData = new List<AgentNeuronData>();

            var entitiesCopy = _agents.ToList();

            Parallel.ForEach(entitiesCopy, parallelOptions, entity =>
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
            });*/
        }

        public void Load(string directoryPath)
        {
            var loadedData = NeuronDataSystem.LoadLatestNeurons(directoryPath);

            Parallel.ForEach(_agents, parallelOptions, entity =>
            {
                var netComponent = ECSManager.GetComponent<NeuralNetComponent>(entity.Key);
                var agent = _agents[entity.Key];

                if (!loadedData.TryGetValue(agent.agentType, out var brainData)) return;

                Parallel.ForEach(agent.brainTypes, parallelOptions, brainType =>
                {
                    if (!brainData.TryGetValue(brainType.Value, out var neuronDataList)) return;

                    for (var i = 0; i < neuronDataList.Count; i++)
                    {
                        var neuronData = neuronDataList[i];
                        foreach (var neuronLayer in netComponent.Layers)
                        {
                            foreach (var layer in neuronLayer)
                            {
                                lock (layer)
                                {
                                    layer.AgentType = neuronData.AgentType;
                                    layer.BrainType = neuronData.BrainType;
                                    layer.Bias = neuronData.Bias;
                                    layer.SetWeights(neuronData.NeuronWeights, 0);
                                }
                            }
                        }

                        lock (netComponent.Fitness)
                        {
                            netComponent.Fitness[i] = neuronData.Fitness;
                        }
                    }
                });
            });
        }

        public static SimAgentType GetNearestEntity(SimAgentTypes entityType, IVector position)
        {
            SimAgentType nearestAgent = null;
            float minDistance = float.MaxValue;

            foreach (var agent in _agents.Values)
            {
                if (agent.agentType != entityType) continue;

                float distance = IVector.Distance(position, agent.CurrentNode.GetCoordinate());

                if (minDistance < distance) continue;

                minDistance = distance;
                nearestAgent = agent;
            }

            return nearestAgent;
        }

        public static SimAgentType GetEntity(SimAgentTypes entityType, INode<IVector> position)
        {
            if (position == null) return null;
            SimAgentType result = null;
            var agentsCopy = _agents.Values.ToArray();

            foreach (var agent in agentsCopy)
            {
                if (agent.agentType != entityType || agent.Transform?.position == null ||
                    !agent.Transform.position.Equals(position.GetCoordinate())) continue;
                result = agent;
                break;
            }

            return result;
        }

        public static SimAgentType GetEntity(SimAgentTypes entityType, ICoordinate<IVector> position)
        {
            SimAgentType target = null;

            foreach (var agent in _agents.Values)
            {
                if (agent.agentType != entityType) continue;

                if (!position.GetCoordinate().Equals(agent.CurrentNode.GetCoordinate())) continue;

                target = agent;
                break;
            }

            return target;
        }

        public static INode<IVector> CoordinateToNode(ICoordinate<IVector> coordinate)
        {
            return graph.NodesType.Cast<INode<IVector>>()
                .FirstOrDefault(node => node.GetCoordinate().Equals(coordinate.GetCoordinate()));
        }

        public static INode<IVector> CoordinateToNode(IVector coordinate)
        {
            if (coordinate.X < 0 || coordinate.Y < 0 || coordinate.X >= graph.MaxX || coordinate.Y >= graph.MaxY)
            {
                return null;
            }

            return graph.NodesType[(int)coordinate.X, (int)coordinate.Y];
        }

        private void StartSimulation()
        {
            _agents = new Dictionary<uint, SimAgentType>();
            _population = new Dictionary<uint, Dictionary<BrainType, List<Genome>>>();
            genAlg = new GeneticAlgorithm(eliteCount, mutationChance, mutationRate);
            GenerateInitialPopulation();
            isRunning = true;
        }

        public void StopSimulation()
        {
            isRunning = false;
            Generation = 0;
            DestroyAgents();
        }

        public void PauseSimulation()
        {
            isRunning = !isRunning;
        }

        public static List<SimBoid> GetBoidsInsideRadius(SimBoid boid)
        {
            List<SimBoid> insideRadiusBoids = new List<SimBoid>();

            foreach (var scavenger in _scavengers.Values)
            {
                if (scavenger?.Transform.position == null)
                {
                    continue;
                }

                if (IVector.Distance(boid.transform.position, scavenger.Transform.position) >
                    boid.detectionRadious) continue;
                if (boid == scavenger.boid) continue;
                insideRadiusBoids.Add(scavenger.boid);
            }

            return insideRadiusBoids;
        }

        public static INode<IVector> GetNearestNode(SimNodeType nodeType, IVector position)
        {
            INode<IVector> nearestNode = null;
            float minDistance = float.MaxValue;

            foreach (var node in graph.NodesType)
            {
                if (node.NodeType != nodeType) continue;

                float distance = IVector.Distance(position, node.GetCoordinate());

                if (minDistance < distance) continue;

                minDistance = distance;

                nearestNode = node;
            }

            return nearestNode;
        }

        private int GetHighestBehaviourCount()
        {
            int highestCount = 0;

            foreach (var entity in _agents.Values)
            {
                int multiThreadCount = entity.Fsm.GetMultiThreadCount();
                int mainThreadCount = entity.Fsm.GetMainThreadCount();

                int maxCount = Math.Max(multiThreadCount, mainThreadCount);
                if (maxCount > highestCount)
                {
                    highestCount = maxCount;
                }
            }

            return highestCount;
        }

        public static int GetBrainTypeKeyByValue(BrainType value, SimAgentTypes agentType)
        {
            var brainTypes = agentType switch
            {
                SimAgentTypes.Carnivore => carnBrainTypes,
                SimAgentTypes.Herbivore => herbBrainTypes,
                SimAgentTypes.Scavenger => scavBrainTypes,
                _ => throw new ArgumentException("Invalid agent type")
            };
            foreach (var kvp in brainTypes)
            {
                if (EqualityComparer<BrainType>.Default.Equals(kvp.Value, value))
                {
                    return kvp.Key;
                }
            }

            throw new KeyNotFoundException("The value is not present in the brainTypes dictionary.");
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;


            foreach (var node in graph.NodesType)
            {
                Gizmos.color = node.NodeType switch
                {
                    SimNodeType.Blocked => Color.black,
                    SimNodeType.Bush => Color.green,
                    SimNodeType.Corpse => Color.red,
                    SimNodeType.Carrion => Color.magenta,
                    SimNodeType.Empty => Color.white,
                    _ => Color.white
                };

                Gizmos.DrawSphere(new Vector3(node.GetCoordinate().X, node.GetCoordinate().Y), (float)CellSize / 5);
                Gizmos.DrawSphere(new Vector3(node.GetCoordinate().X, node.GetCoordinate().Y), (float)CellSize / 5);
            }
        }

        private void PurgingSpecials()
        {
            var agentsToRemove = new List<uint>();

            foreach (var agentEntry in _agents)
            {
                var agent = agentEntry.Value;
                if (agent.agentType == SimAgentTypes.Herbivore)
                {
                    if (agent is Herbivore<IVector, ITransform<IVector>> { Hp: < 0 })
                    {
                        agentsToRemove.Add(agentEntry.Key);
                    }
                }

                if (agent.CanReproduce)
                {
                    agentsToRemove.Add(agentEntry.Key);
                }
            }

            foreach (var agentId in agentsToRemove)
            {
                RemoveEntity(_agents[agentId]);
            }

            agentsToRemove.Clear();
        }


        public static void RemoveEntity(SimAgentType simAgent)
        {
            simAgent.Uninit();
            CountMissing(simAgent.agentType);
            uint agentId = _agents.FirstOrDefault(agent => agent.Value == simAgent).Key;
            _agents.Remove(agentId);
            _population.Remove(agentId);
            _scavengers.Remove(agentId);
            ECSManager.RemoveEntity(agentId);
        }

        private static void CountMissing(SimAgentTypes agentType)
        {
            switch (agentType)
            {
                case SimAgentTypes.Carnivore:
                    missingCarnivores++;
                    break;
                case SimAgentTypes.Herbivore:
                    missingHerbivores++;
                    break;
                case SimAgentTypes.Scavenger:
                    missingScavengers++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}