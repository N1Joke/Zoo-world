using System.Collections.Generic;
using Core;
using UnityEngine;

namespace ZooWorld.Gameplay.FX
{
    // Simple stack-based pool. Instances are tracked via AddObject so they
    // get destroyed together with the level scope.
    public class FloatingTextPool : BaseDisposable
    {
        private readonly FloatingTextView _prefab;
        private readonly Transform _parent;
        private readonly Stack<FloatingTextView> _idle = new();

        public FloatingTextPool(FloatingTextView prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;

            if (_prefab == null)
                Debug.LogWarning("[FloatingTextPool] Floating text prefab is not assigned.");
        }

        public FloatingTextView Get(Vector3 worldPosition)
        {
            if (isDisposed || _prefab == null) return null;

            FloatingTextView instance = null;
            while (_idle.Count > 0 && instance == null)
                instance = _idle.Pop();

            if (instance == null)
            {
                instance = AddObject(Object.Instantiate(_prefab, worldPosition, _prefab.transform.rotation, _parent));
            }
            else
            {
                instance.transform.position = worldPosition;
                instance.gameObject.SetActive(true);
            }

            return instance;
        }

        public void Release(FloatingTextView instance)
        {
            if (instance == null) return;

            if (isDisposed)
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            instance.gameObject.SetActive(false);
            _idle.Push(instance);
        }
    }
}
