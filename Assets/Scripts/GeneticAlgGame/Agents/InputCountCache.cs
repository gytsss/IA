using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithmDirectory.ECS;
using GeneticAlgorithmDirectory.NeuralNet;
using NeuralNetworkDirectory.NeuralNet;
using StateMachine.Agents.Simulation;

namespace GeneticAlgGame.Agents
{
    public static class InputCountCache
    {
        private static readonly Dictionary<(SimAgentTypes, BrainType), int> cache = new();

        public static int GetInputCount(SimAgentTypes agentType, BrainType brainType)
        {
            var key = (agentType, brainType);
            if (cache.TryGetValue(key, out int inputCount)) return inputCount;
            
            inputCount = EcsPopulationManager.inputCounts
                .FirstOrDefault(input => input.agentType == agentType && input.brainType == brainType).inputCount;
            cache[key] = inputCount;

            return inputCount;
        }
    }
}