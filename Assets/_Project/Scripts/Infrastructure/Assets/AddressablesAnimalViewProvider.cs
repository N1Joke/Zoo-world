using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZooWorld.Gameplay.Animals;

namespace ZooWorld.Infrastructure.Assets
{
    public class AddressablesAnimalViewProvider : IAnimalViewProvider
    {
        private readonly IAnimalViewCatalog _catalog;
        private readonly Dictionary<AnimalView, AsyncOperationHandle<GameObject>> _instanceHandles = new();
        private bool _disposed;

        public AddressablesAnimalViewProvider(IAnimalViewCatalog catalog)
        {
            _catalog = catalog;
        }

        public async UniTask<AnimalView> LoadViewAsync(int id, Vector3 position, Quaternion rotation)
        {
            if (_disposed) return null;

            if (!_catalog.TryGetReference(id, out AssetReferenceGameObject reference) || reference == null)
            {
                Debug.LogError($"[AddressablesAnimalViewProvider] No AssetReference registered for id {id}.");
                return null;
            }

            AsyncOperationHandle<GameObject> handle = reference.InstantiateAsync(position, rotation);
            GameObject instance;
            try
            {
                instance = await handle.ToUniTask();
            }
            catch (Exception e)
            {
                Debug.LogError($"[AddressablesAnimalViewProvider] Failed to instantiate id {id}: {e}");
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }

            if (instance == null)
            {
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }

            AnimalView view = instance.GetComponent<AnimalView>();
            if (view == null)
            {
                Debug.LogError($"[AddressablesAnimalViewProvider] Prefab for id {id} has no AnimalView component.");
                Addressables.ReleaseInstance(instance);
                return null;
            }

            _instanceHandles[view] = handle;
            return view;
        }

        public void Release(AnimalView view)
        {
            if (view == null) return;

            bool found = _instanceHandles.TryGetValue(view, out AsyncOperationHandle<GameObject> handle);
            if (found)
                _instanceHandles.Remove(view);
            else
                Debug.LogWarning($"[AddressablesAnimalViewProvider] Release: view '{view.name}' was not tracked; destroying anyway.");

            DestroyAndRelease(view != null ? view.gameObject : null, handle, found);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (KeyValuePair<AnimalView, AsyncOperationHandle<GameObject>> kvp in _instanceHandles)
            {
                GameObject go = kvp.Key != null ? kvp.Key.gameObject : null;
                DestroyAndRelease(go, kvp.Value, true);
            }
            _instanceHandles.Clear();
        }

        private static void DestroyAndRelease(GameObject go, AsyncOperationHandle<GameObject> handle, bool hasHandle)
        {
            bool released = false;
            if (hasHandle && handle.IsValid())
            {
                if (go != null)
                    released = Addressables.ReleaseInstance(go);
                else
                    Addressables.Release(handle);
            }

            if (go == null) return;
            if (!released)
                UnityEngine.Object.Destroy(go);
        }
    }
}
