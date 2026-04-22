using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    public interface IAnimalFactory
    {
        UniTask<AnimalControllerBase> CreateAsync(AnimalDataRow row, Vector3 position);
    }
}
