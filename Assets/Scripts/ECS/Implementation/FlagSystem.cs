using System.Collections.Generic;
using System.Threading.Tasks;
using ECS.Implementation;

public sealed class FlagSystem 
{
    private ParallelOptions parallelOptions;
    
    private IDictionary<uint, RotationComponent> rotationComponents;
    private IDictionary<uint, VelRotationComponent> velocityRotComponents;
    private IEnumerable<uint> queryedEntities;

   
   

   
}