using UnityEngine;

namespace ZooWorld.Gameplay.Animals.Combat
{
    public readonly struct FoodChainOutcome
    {
        public readonly bool SelfDies;
        public readonly bool OtherDies;
        public readonly bool Tasty;

        // Where to spawn "Tasty!" — the eater's position. Only meaningful when Tasty is true.
        public readonly Vector3 TastyPosition;

        public static readonly FoodChainOutcome None = default;

        public FoodChainOutcome(bool selfDies, bool otherDies, bool tasty, Vector3 tastyPosition)
        {
            SelfDies = selfDies;
            OtherDies = otherDies;
            Tasty = tasty;
            TastyPosition = tastyPosition;
        }
    }
}
