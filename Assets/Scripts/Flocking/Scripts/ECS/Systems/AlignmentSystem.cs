using System.Collections.Generic;
using System.Threading.Tasks;
using ECS.Implementation;
using ECS.Patron;

public sealed class AlignmentSystem : ECSSystem
{
    private ParallelOptions parallelOptions;
    
    private IDictionary<uint, FlockingComponent> flockingComponents;
    private IEnumerable<uint> queryedEntities;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        flockingComponents??= ECSManager.GetComponents<FlockingComponent>();
        queryedEntities??= ECSManager.GetEntitiesWhitComponentTypes(typeof(FlockingComponent));
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
             
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
}