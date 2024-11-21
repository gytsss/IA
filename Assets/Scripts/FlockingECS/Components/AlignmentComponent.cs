using ECS.Patron;

namespace FlockingECS.Components
{
    public class AlignmentComponent : ECSComponent
    {
        public float speed;
        public float turnSpeed;
        public float detectionRadious;
        public float alignmentMultiplier;

        public AlignmentComponent(float speed, float turnSpeed, float detectionRadious, float alignmentMultiplier)
        {
            this.speed = speed;
            this.turnSpeed = turnSpeed;
            this.detectionRadious = detectionRadious;
            this.alignmentMultiplier = alignmentMultiplier;
        }
    }
}