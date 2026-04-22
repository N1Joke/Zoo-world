using System;
using System.Collections.Generic;

namespace ZooWorld.Gameplay.Animals
{
    public interface IAnimalRegistry : IDisposable
    {
        IReadOnlyCollection<AnimalControllerBase> All { get; }
        void Register(AnimalControllerBase controller);
        void Unregister(AnimalControllerBase controller);
        bool TryGet(AnimalView view, out AnimalControllerBase controller);
    }
}
