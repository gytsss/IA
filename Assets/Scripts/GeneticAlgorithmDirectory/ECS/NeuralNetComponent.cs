using System.Collections.Generic;

namespace GeneticAlgorithmDirectory.ECS
{
    public class NeuralNetComponent : ECSComponent
    {
        public List<List<NeuronLayer>> Layers { get; set; } = new();
    }
}