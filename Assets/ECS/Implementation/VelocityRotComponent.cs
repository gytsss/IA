namespace ECS.Implementation
{
    public class VelocityRotComponent : ECSComponent
    {
        public float velocity;

        public float directionX;
        public float directionY;
        public float directionZ;

        public VelocityRotComponent(float velocity, float directionX, float directionY, float directionZ)
        {
            this.velocity = velocity;
            this.directionX = directionX;
            this.directionY = directionY;
            this.directionZ = directionZ;
        }
    }
}