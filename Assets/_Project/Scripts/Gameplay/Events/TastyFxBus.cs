using System;
using Tools.Extensions;
using UnityEngine;

namespace ZooWorld.Gameplay.Events
{
    public class TastyFxBus : ITastyFxBus
    {
        private readonly ReactiveEvent<Vector3> _event = new();

        public IDisposable Subscribe(Action<Vector3> action) => _event.Subscribe(action);
        public IDisposable SubscribeWithSkip(Action<Vector3> action) => _event.SubscribeWithSkip(action);
        public IDisposable SubscribeOnceWithSkip(Action<Vector3> action) => _event.SubscribeOnceWithSkip(action);

        public void Notify(Vector3 worldPosition) => _event.Notify(worldPosition);

        public void Dispose() => _event.Dispose();
    }
}
