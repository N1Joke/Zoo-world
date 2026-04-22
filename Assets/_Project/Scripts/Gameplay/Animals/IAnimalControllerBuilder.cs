using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    // Extension point for species that need custom logic (unique AI, group behaviour,
    // bosses). Ordinary species don't touch this — AnimalFactory handles them
    // through GenericAnimalController.
    public interface IAnimalControllerBuilder
    {
        AnimalSubtype Subtype { get; }

        AnimalControllerBase Build(
            AnimalModel model,
            AnimalView view,
            AnimalControllerDependencies dependencies);
    }
}
