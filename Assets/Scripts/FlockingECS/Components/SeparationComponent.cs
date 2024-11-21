using ECS.Patron;

namespace FlockingECS.Components
{
    public class SeparationComponent : ECSComponent
    {
        public float speed;
        public float turnSpeed;
        public float detectionRadious;
        public float separationMultiplier;


        public SeparationComponent(float speed, float turnSpeed, float detectionRadious,
            float separationMultiplier)
        {
            this.speed = speed;
            this.turnSpeed = turnSpeed;
            this.detectionRadious = detectionRadious;
            this.separationMultiplier = separationMultiplier;
        }
    }
}