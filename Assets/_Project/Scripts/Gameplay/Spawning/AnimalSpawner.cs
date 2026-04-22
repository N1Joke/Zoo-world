using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Tools.Extensions;
using UnityEngine;
using ZooWorld.Gameplay.Animals;
using ZooWorld.Gameplay.World;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Spawning
{
    // Spawns a random animal from the data registry every 1-2 seconds.
    public class AnimalSpawner : BaseDisposable
    {
        private const float MinInterval = 1f;
        private const float MaxInterval = 2f;

        private readonly IAnimalFactory _factory;
        private readonly IAnimalDataRegistry _dataRegistry;
        private readonly IScreenBoundsService _bounds;
        private readonly CancellationTokenSource _cts = new();

        private IDisposable _loop;
        private bool _started;
        private bool _ctsReleased;

        public AnimalSpawner(
            IAnimalFactory factory,
            IAnimalDataRegistry dataRegistry,
            IScreenBoundsService bounds)
        {
            _factory = factory;
            _dataRegistry = dataRegistry;
            _bounds = bounds;
        }

        public void Start()
        {
            if (_started || isDisposed) return;
            _started = true;
            ScheduleNext();
        }

        private void ScheduleNext()
        {
            if (isDisposed) return;
            float delay = UnityEngine.Random.Range(MinInterval, MaxInterval);
            _loop?.Dispose();
            _loop = ReactiveExtensions.DelayedCall(delay, OnTick);
            if (_loop != null) AddDispose(_loop);
        }

        private void OnTick()
        {
            if (isDisposed) return;
            SpawnOneAsync(_cts.Token).Forget();
            ScheduleNext();
        }

        private async UniTaskVoid SpawnOneAsync(CancellationToken ct)
        {
            if (_dataRegistry.Count == 0) return;
            int index = UnityEngine.Random.Range(0, _dataRegistry.Count);
            AnimalDataRow row = _dataRegistry.All[index];
            Vector3 pos = _bounds.GetRandomPointInside();

            try
            {
                AnimalControllerBase controller = await _factory.CreateAsync(row, pos);
                if (controller == null) return;
                if (ct.IsCancellationRequested) controller.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError($"[AnimalSpawner] Failed to spawn id {row.Id}: {e}");
            }
        }

        protected override void OnDispose()
        {
            if (_ctsReleased) return;
            _ctsReleased = true;
            _cts.Cancel();
            _cts.Dispose();
            _started = false;
        }
    }
}
