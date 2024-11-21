using ECS.Patron;

namespace FlockingECS.Components
{
    public class CohesionComponent : ECSComponent
    {
        public float speed;
        public float turnSpeed;
        public float detectionRadious;
        public float cohesionMultiplier;

        public CohesionComponent(float speed, float turnSpeed, float detectionRadious,
            float cohesionMultiplier)
        {
            this.speed = speed;
            this.turnSpeed = turnSpeed;
            this.detectionRadious = detectionRadious;
            this.cohesionMultiplier = cohesionMultiplier;
        }
    }
}