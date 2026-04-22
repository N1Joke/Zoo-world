using System;
using Tools.Extensions;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Events
{
    public class AnimalDeathEventBus : IAnimalDeathEventBus
    {
        private readonly ReactiveEvent<AnimalRole> _event = new();

        public IDisposable Subscribe(Action<AnimalRole> action) => _event.Subscribe(action);
        public IDisposable SubscribeWithSkip(Action<AnimalRole> action) => _event.SubscribeWithSkip(action);
        public IDisposable SubscribeOnceWithSkip(Action<AnimalRole> action) => _event.SubscribeOnceWithSkip(action);

        public void Notify(AnimalRole role) => _event.Notify(role);

        public void Dispose() => _event.Dispose();
    }
}
