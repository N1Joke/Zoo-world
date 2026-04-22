using UnityEngine;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals.Combat
{
    // Deterministically resolves a collision pair from either side.
    // AnimalControllerBase already guarantees Resolve is called only once per pair
    // per frame. The method itself is symmetric — argument order doesn't affect the outcome.
    //
    // Rules:
    //   prey + prey          -> None (handled via knockback in the controller)
    //   predator + prey      -> prey dies, Tasty at the predator's position
    //   predator + predator  -> higher Hp+Damage wins; ties broken by InstanceId.
    //                          Tasty at the winner's position.
    public class FoodChainResolver : IFoodChainResolver
    {
        public FoodChainOutcome Resolve(AnimalControllerBase self, AnimalControllerBase other)
        {
            if (self == null || other == null) return FoodChainOutcome.None;
            if (self.IsDisposed || other.IsDisposed) return FoodChainOutcome.None;
            if (self.View == null || other.View == null) return FoodChainOutcome.None;

            AnimalRole selfRole = self.Model.Role;
            AnimalRole otherRole = other.Model.Role;

            if (selfRole == AnimalRole.Prey && otherRole == AnimalRole.Prey)
                return FoodChainOutcome.None;

            if (selfRole == AnimalRole.Predator && otherRole == AnimalRole.Prey)
                return new FoodChainOutcome(
                    selfDies: false,
                    otherDies: true,
                    tasty: true,
                    tastyPosition: self.View.transform.position);

            if (selfRole == AnimalRole.Prey && otherRole == AnimalRole.Predator)
                return new FoodChainOutcome(
                    selfDies: true,
                    otherDies: false,
                    tasty: true,
                    tastyPosition: other.View.transform.position);

            float selfScore = Score(self);
            float otherScore = Score(other);

            bool selfSurvives = Mathf.Approximately(selfScore, otherScore)
                ? self.InstanceId > other.InstanceId
                : selfScore > otherScore;

            Vector3 tastyPos = (selfSurvives ? self.View : other.View).transform.position;
            return new FoodChainOutcome(
                selfDies: !selfSurvives,
                otherDies: selfSurvives,
                tasty: true,
                tastyPosition: tastyPos);
        }

        private static float Score(AnimalControllerBase a) => a.Model.Data.Hp + a.Model.Data.Damage;
    }
}
