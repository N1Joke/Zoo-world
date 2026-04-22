using System.Collections.Generic;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly IGameStateChanger _stateMachine;
        private readonly CsvAnimalDataLoader _csvLoader;
        private readonly AnimalDataRegistry _dataRegistry;
        private readonly IAnimalViewCatalog _viewCatalog;

        public BootstrapState(
            IGameStateChanger stateMachine,
            CsvAnimalDataLoader csvLoader,
            AnimalDataRegistry dataRegistry,
            IAnimalViewCatalog viewCatalog)
        {
            _stateMachine = stateMachine;
            _csvLoader = csvLoader;
            _dataRegistry = dataRegistry;
            _viewCatalog = viewCatalog;
        }

        public void Enter() => RunAsync().Forget();

        public void Exit() { }

        private async UniTaskVoid RunAsync()
        {
            IReadOnlyList<AnimalDataRow> rows = await _csvLoader.LoadAsync();
            _dataRegistry.Populate(rows);
            ValidateCatalog(rows);

            Debug.Log($"[Bootstrap] Loaded {rows.Count} animals from CSV.");
            _stateMachine.Enter<Game_LevelState>();
        }

        private void ValidateCatalog(IReadOnlyList<AnimalDataRow> rows)
        {
            if (_viewCatalog == null) return;
            for (int i = 0; i < rows.Count; i++)
            {
                int id = rows[i].Id;
                if (!_viewCatalog.TryGetReference(id, out _))
                    Debug.LogWarning($"[Bootstrap] Animal id {id} has no view reference in AnimalViewCatalogSO.");
            }
        }
    }
}
