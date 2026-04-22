namespace ZooWorld.Gameplay.Animals.Combat
{
    public interface IFoodChainResolver
    {
        FoodChainOutcome Resolve(AnimalControllerBase self, AnimalControllerBase other);
    }
}
