public class ACSComponent : ECSComponent
{
    public float speed;
    public float turnSpeed;
    public float detectionRadious;

    public ACSComponent(float speed, float turnSpeed, float detectionRadious)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.detectionRadious = detectionRadious;
    }
}