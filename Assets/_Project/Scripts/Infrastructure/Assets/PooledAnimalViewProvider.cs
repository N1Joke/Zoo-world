using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Gameplay.Animals;

namespace ZooWorld.Infrastructure.Assets
{
    // Decorator over IAnimalViewProvider that recycles released AnimalViews
    // into a per-id pool instead of destroying them. Eliminates GC spikes from
    // Addressables Instantiate/Destroy during heavy spawning.
    //
    // Scoped to the level. On Dispose the inner provider is disposed, which
    // releases all Addressables handles and destroys every instance.
    public class PooledAnimalViewProvider : IAnimalViewProvider
    {
        private readonly IAnimalViewProvider _inner;
        private readonly Dictionary<int, Stack<AnimalView>> _idle = new();
        private readonly Dictionary<AnimalView, int> _viewToId = new();
        private bool _disposed;

        public PooledAnimalViewProvider(IAnimalViewProvider inner)
        {
            _inner = inner;
        }

        public async UniTask<AnimalView> LoadViewAsync(int id, Vector3 position, Quaternion rotation)
        {
            if (_disposed) return null;

            if (_idle.TryGetValue(id, out Stack<AnimalView> stack))
            {
                while (stack.Count > 0)
                {
                    AnimalView reused = stack.Pop();
                    if (reused == null) continue;

                    Activate(reused, position, rotation);
                    return reused;
                }
            }

            AnimalView view = await _inner.LoadViewAsync(id, position, rotation);
            if (view == null) return null;

            _viewToId[view] = id;
            return view;
        }

        public void Release(AnimalView view)
        {
            if (view == null) return;

            // inner provider already torn down — forward to it to avoid leaks
            if (_disposed)
            {
                _inner.Release(view);
                return;
            }

            if (!_viewToId.TryGetValue(view, out int id))
            {
                // view not tracked by pool (shouldn't normally happen) — let inner handle it
                _inner.Release(view);
                return;
            }

            if (view.gameObject == null)
            {
                _viewToId.Remove(view);
                return;
            }

            Deactivate(view);

            if (!_idle.TryGetValue(id, out Stack<AnimalView> stack))
            {
                stack = new Stack<AnimalView>();
                _idle[id] = stack;
            }
            stack.Push(view);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _idle.Clear();
            _viewToId.Clear();
            _inner.Dispose();
        }

        private static void Activate(AnimalView view, Vector3 position, Quaternion rotation)
        {
            Transform t = view.transform;
            t.SetPositionAndRotation(position, rotation);
            ResetRigidbody(view.Rigidbody);
            if (!view.gameObject.activeSelf)
                view.gameObject.SetActive(true);
        }

        private static void Deactivate(AnimalView view)
        {
            ResetRigidbody(view.Rigidbody);
            if (view.gameObject.activeSelf)
                view.gameObject.SetActive(false);
        }

        private static void ResetRigidbody(Rigidbody rb)
        {
            if (rb == null) return;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
