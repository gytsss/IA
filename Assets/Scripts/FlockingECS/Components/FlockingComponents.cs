using ECS.Patron;

namespace FlockingECS.Components
{
    public class FlockingComponent : ECSComponent
    {
        public float speed;
        public float turnSpeed;
        public float detectionRadious;
        public float alignmentMultiplier;
        public float cohesionMultiplier;
        public float separationMultiplier;
        public float directionMultiplier;

        public FlockingComponent(float speed, float turnSpeed, float detectionRadious, float alignmentMultiplier,
            float cohesionMultiplier, float separationMultiplier, float directionMultiplier)
        {
            this.speed = speed;
            this.turnSpeed = turnSpeed;
            this.detectionRadious = detectionRadious;
            this.alignmentMultiplier = alignmentMultiplier;
            this.cohesionMultiplier = cohesionMultiplier;
            this.separationMultiplier = separationMultiplier;
            this.directionMultiplier = directionMultiplier;
        }
    }
}