using System;
using Tools.Extensions;
using UnityEngine;

namespace ZooWorld.Gameplay.Events
{
    public interface ITastyFxBus : IReadOnlyReactiveEvent<Vector3>, IDisposable
    {
        void Notify(Vector3 worldPosition);
    }
}
