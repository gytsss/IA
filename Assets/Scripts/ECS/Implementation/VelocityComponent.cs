using ECS.Patron;

namespace ECS.Implementation
{
    public class VelocityComponent<TVector> : ECSComponent
    {
        public TVector direction;
        public float velocity;

        public VelocityComponent(float velocity, TVector direction)
        {
            this.velocity = velocity;
            this.direction = direction;
        }
    }
}