using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Gameplay.Events;

namespace ZooWorld.Gameplay.FX
{
    // Listens to TastyFxBus and pops a "Tasty!" label at the given world position.
    // Instances come from the pool to avoid GC spikes on every spawn.
    public class DamageTextFactory : BaseDisposable
    {
        private const string TastyText = "Tasty!";

        private readonly ITastyFxBus _tastyBus;
        private readonly FloatingTextPool _pool;
        private readonly CancellationTokenSource _cts = new();
        private bool _ctsReleased;

        public DamageTextFactory(ITastyFxBus tastyBus, FloatingTextPool pool)
        {
            _tastyBus = tastyBus;
            _pool = pool;

            AddDispose(_tastyBus.SubscribeWithSkip(OnTasty));
        }

        private void OnTasty(Vector3 worldPosition)
        {
            if (isDisposed) return;
            SpawnAsync(worldPosition, _cts.Token).Forget();
        }

        private async UniTaskVoid SpawnAsync(Vector3 worldPosition, CancellationToken ct)
        {
            FloatingTextView instance = _pool.Get(worldPosition);
            if (instance == null) return;

            instance.SetText(TastyText);

            TMPro.TMP_Text tmp = instance.Text;
            Color baseColor = tmp != null ? tmp.color : Color.white;
            // alpha may be zero from a previous fade-out, reset it
            baseColor.a = 1f;
            if (tmp != null) tmp.color = baseColor;

            float lifetime = instance.Lifetime;
            float rise = instance.RiseSpeed;
            Vector3 basePos = worldPosition;

            float time = 0f;
            try
            {
                while (time < lifetime && instance != null)
                {
                    if (ct.IsCancellationRequested) break;
                    time += Time.deltaTime;
                    float t = time / lifetime;
                    instance.transform.position = basePos + Vector3.up * (rise * time);
                    if (tmp != null)
                        tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(1f, 0f, t));

                    await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                }
            }
            finally
            {
                _pool.Release(instance);
            }
        }

        protected override void OnDispose()
        {
            if (_ctsReleased) return;
            _ctsReleased = true;
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
