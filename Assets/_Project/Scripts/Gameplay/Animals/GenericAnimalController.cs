using ZooWorld.Gameplay.Animals.Movement;

namespace ZooWorld.Gameplay.Animals
{
    // Default controller for species that need no custom logic.
    // Collision/combat/dispose all live in the base; movement comes from the strategy.
    public sealed class GenericAnimalController : AnimalControllerBase
    {
        public GenericAnimalController(
            AnimalModel model,
            AnimalView view,
            IMovementStrategy movement,
            AnimalControllerDependencies dependencies)
            : base(model, view, movement, dependencies)
        {
        }
    }
}
