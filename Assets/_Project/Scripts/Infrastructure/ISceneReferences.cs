using UnityEngine;
using ZooWorld.Gameplay.FX;
using ZooWorld.UI.DeathCounter;

namespace ZooWorld.Infrastructure
{
    // Exposes scene-serialized references (prefabs, UI views, transforms) to
    // the level scope. Implemented on RootLifetimeScope.
    public interface ISceneReferences
    {
        DeathCounterView DeathCounterView { get; }
        FloatingTextView FloatingTextPrefab { get; }
        Transform WorldTextParent { get; }
    }
}
