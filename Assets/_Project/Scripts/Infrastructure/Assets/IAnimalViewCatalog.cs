using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace ZooWorld.Infrastructure.Assets
{
    public interface IAnimalViewCatalog
    {
        IReadOnlyList<int> AllIds { get; }
        bool TryGetReference(int id, out AssetReferenceGameObject reference);
    }
}
