using Core;
using UniRx;
using ZooWorld.Gameplay.Events;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.UI.DeathCounter
{
    public class DeathCounterModel : BaseDisposable
    {
        public ReactiveProperty<int> PreyDead { get; } = new(0);
        public ReactiveProperty<int> PredatorDead { get; } = new(0);

        public DeathCounterModel(IAnimalDeathEventBus deathBus)
        {
            AddDispose(deathBus.SubscribeWithSkip(OnDeath));
            AddDispose(PreyDead);
            AddDispose(PredatorDead);
        }

        private void OnDeath(AnimalRole role)
        {
            if (role == AnimalRole.Prey) PreyDead.Value++;
            else PredatorDead.Value++;
        }
    }
}
