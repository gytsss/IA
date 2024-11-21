using ECS.Patron;

namespace ECS.Implementation
{
    public class PositionComponent<T> : ECSComponent
    {
        public T Position;

        public PositionComponent(T position)
        {
            Position = position;
        }
    }
}