﻿using GeneticAlgGame.Agents;
using GeneticAlgorithmDirectory.NeuralNet;

namespace GeneticAlgorithmDirectory.DataManagement
{
    public class AgentNeuronData
    {
        public uint AgentId { get; set; }
        public float[] NeuronWeights { get; set; }
        public float Fitness { get; set; }
        public BrainType BrainType { get; set; }
        public SimAgentTypes AgentType { get; set; }
        public float Bias { get; set; } = 1;
        public float P { get; set; } = 0.5f;
        public int TotalWeights { get; set; }
    }
    
}