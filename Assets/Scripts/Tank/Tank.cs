using UnityEngine;

namespace Tank
{
    public class Tank : TankBase
    {
        float fitness = 0;

        protected override void OnReset()
        {
            fitness = 1;
        }
        
        protected override void OnThink(float dt)
        {
            Vector3 dirToGoodMine = GetDirToMine(goodMine);
            Vector3 dirToBadMine = GetDirToMine(badMine);
            

            inputs[0] = dirToGoodMine.x;
            inputs[1] = dirToGoodMine.z;
            inputs[2] = transform.forward.x;
            inputs[3] = transform.forward.z;
            inputs[4] = dirToBadMine.x;
            inputs[5] = dirToBadMine.z;
            
            float[] output = movementBrain.Synapsis(inputs);

            SetForces(output[0], output[1], dt);
        }

        protected override void OnTakeMine(GameObject mine)
        {
            if (mine == goodMine)
                fitness += 10;
            else if (mine == badMine)
                fitness *= 0.8f;
            
            movementGenome.fitness = fitness;
        }
    }
}