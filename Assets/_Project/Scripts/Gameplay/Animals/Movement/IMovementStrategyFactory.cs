using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals.Movement
{
    public interface IMovementStrategyFactory
    {
        IMovementStrategy Create(AnimalDataRow row);
    }
}
