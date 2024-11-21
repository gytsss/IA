using ECS.Patron;

namespace GeneticAlgorithmDirectory.ECS
{
    public class NeuronComponent : ECSComponent
    {
        public NeuronComponent(float[] weights, float bias, float p)
        {
            Weights = weights;
            Bias = bias;
            P = p;
        }

        public float[] Weights { get; set; }
        public float Bias { get; set; }
        public float P { get; set; }
    }
}