using System.Collections.Generic;
using System.Threading.Tasks;
using ECS.Implementation;
using ECS.Patron;

public sealed class RotationSystem : ECSSystem
{
    private ParallelOptions parallelOptions;
    
    private IDictionary<uint, RotationComponent> rotationComponents;
    private IDictionary<uint, VelocityRotComponent> velocityRotComponents;
    private IEnumerable<uint> queryedEntities;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        rotationComponents??= ECSManager.GetComponents<RotationComponent>();
        velocityRotComponents??= ECSManager.GetComponents<VelocityRotComponent>();
        queryedEntities??= ECSManager.GetEntitiesWithComponentTypes(typeof(RotationComponent), typeof(VelocityRotComponent));
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            rotationComponents[i].X += velocityRotComponents[i].directionX * velocityRotComponents[i].velocity * deltaTime;
            rotationComponents[i].Y += velocityRotComponents[i].directionY * velocityRotComponents[i].velocity * deltaTime;
            rotationComponents[i].Z += velocityRotComponents[i].directionZ * velocityRotComponents[i].velocity * deltaTime;
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
}