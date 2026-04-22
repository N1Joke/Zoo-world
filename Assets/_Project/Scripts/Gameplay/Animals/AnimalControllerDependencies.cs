using ZooWorld.Gameplay.Animals.Combat;
using ZooWorld.Gameplay.Events;
using ZooWorld.Gameplay.World;
using ZooWorld.Infrastructure.Assets;

namespace ZooWorld.Gameplay.Animals
{
    // Bundles the level-scoped services every AnimalControllerBase needs.
    // Kept as a class (not a struct) so the same reference is shared across all
    // controllers; VContainer wires it via constructor injection.
    public class AnimalControllerDependencies
    {
        public IScreenBoundsService Bounds { get; }
        public IAnimalRegistry Registry { get; }
        public IFoodChainResolver FoodChain { get; }
        public IAnimalDeathEventBus DeathBus { get; }
        public ITastyFxBus TastyBus { get; }
        public IAnimalViewProvider ViewProvider { get; }

        public AnimalControllerDependencies(
            IScreenBoundsService bounds,
            IAnimalRegistry registry,
            IFoodChainResolver foodChain,
            IAnimalDeathEventBus deathBus,
            ITastyFxBus tastyBus,
            IAnimalViewProvider viewProvider)
        {
            Bounds = bounds;
            Registry = registry;
            FoodChain = foodChain;
            DeathBus = deathBus;
            TastyBus = tastyBus;
            ViewProvider = viewProvider;
        }
    }
}
