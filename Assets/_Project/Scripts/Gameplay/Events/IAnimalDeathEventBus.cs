using System;
using Tools.Extensions;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Events
{
    public interface IAnimalDeathEventBus : IReadOnlyReactiveEvent<AnimalRole>, IDisposable
    {
        void Notify(AnimalRole role);
    }
}
