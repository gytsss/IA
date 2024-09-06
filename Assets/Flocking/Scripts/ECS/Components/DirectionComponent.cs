public class DirectionComponent : ECSComponent
{
    public float speed;
    public float turnSpeed;
    public float detectionRadious;
    public float directionMultiplier;

    public DirectionComponent(float speed, float turnSpeed, float detectionRadious, float directionMultiplier)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.detectionRadious = detectionRadious;
        this.directionMultiplier = directionMultiplier;
    }
}