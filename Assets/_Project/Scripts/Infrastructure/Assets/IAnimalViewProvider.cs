using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Gameplay.Animals;

namespace ZooWorld.Infrastructure.Assets
{
    public interface IAnimalViewProvider : IDisposable
    {
        UniTask<AnimalView> LoadViewAsync(int id, Vector3 position, Quaternion rotation);
        void Release(AnimalView view);
    }
}
