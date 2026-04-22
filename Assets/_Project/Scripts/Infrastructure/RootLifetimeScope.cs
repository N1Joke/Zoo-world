using UnityEngine;
using VContainer;
using VContainer.Unity;
using ZooWorld.Gameplay.Events;
using ZooWorld.Gameplay.FX;
using ZooWorld.Gameplay.World;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.Infrastructure.Data;
using ZooWorld.Infrastructure.States;
using ZooWorld.UI.DeathCounter;

namespace ZooWorld.Infrastructure
{
    public class RootLifetimeScope : LifetimeScope, ISceneReferences
    {
        [Header("Data")]
        [SerializeField] private TextAsset _animalsCsv;
        [SerializeField] private AnimalViewCatalogSO _animalViewCatalog;

        [Header("World")]
        [SerializeField] private Camera _gameplayCamera;
        [SerializeField] private float _groundY = 0f;
        [SerializeField] private float _boundsPadding = 0.5f;

        [Header("UI")]
        [SerializeField] private DeathCounterView _deathCounterView;

        [Header("FX")]
        [SerializeField] private FloatingTextView _floatingTextPrefab;
        [SerializeField] private Transform _worldTextParent;

        private GameStateMachine _stateMachine;
        private bool _cleanupRequested;

        public DeathCounterView DeathCounterView => _deathCounterView;
        public FloatingTextView FloatingTextPrefab => _floatingTextPrefab;
        public Transform WorldTextParent => _worldTextParent;

        protected override void Configure(IContainerBuilder builder)
        {
            Camera cam = _gameplayCamera != null ? _gameplayCamera : Camera.main;
            TextAsset csv = _animalsCsv;
            AnimalViewCatalogSO catalog = _animalViewCatalog;
            float groundY = _groundY;
            float padding = _boundsPadding;

            if (csv == null)
                Debug.LogError("[RootLifetimeScope] Animals CSV TextAsset is not assigned.");
            if (catalog == null)
                Debug.LogError("[RootLifetimeScope] AnimalViewCatalogSO is not assigned.");
            else
                builder.RegisterInstance<IAnimalViewCatalog>(catalog);

            builder.Register(_ => new CsvAnimalDataLoader(csv), Lifetime.Singleton);

            builder.Register<AnimalDataRegistry>(Lifetime.Singleton)
                .As<IAnimalDataRegistry>()
                .AsSelf();

            builder.Register<IAnimalDeathEventBus, AnimalDeathEventBus>(Lifetime.Singleton);

            builder.Register<IScreenBoundsService>(
                _ => new CameraScreenBoundsService(cam, groundY, padding),
                Lifetime.Singleton);

            builder.Register<ISceneReferences>(_ => this, Lifetime.Singleton);

            builder.Register<GameStateMachine>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppBootstrapper>();

            // Cache the state machine so OnApplicationQuit can route shutdown through
            // Game_CleanUpState → Game_LevelState.Exit → _levelScope.Dispose.
            builder.RegisterBuildCallback(resolver =>
            {
                _stateMachine = resolver.Resolve<GameStateMachine>();
            });
        }

        // Route application shutdown through the state machine so every level-scope
        // service (view providers, animal registry, pools, buses, etc.) disposes
        // cleanly before Unity tears down the scene.
        private void OnApplicationQuit()
        {
            RequestCleanup();
        }

        private void RequestCleanup()
        {
            if (_cleanupRequested) return;
            _cleanupRequested = true;
            _stateMachine?.Enter<Game_CleanUpState>();
        }
    }
}
