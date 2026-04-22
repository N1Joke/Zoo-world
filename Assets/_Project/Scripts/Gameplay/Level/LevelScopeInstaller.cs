using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using ZooWorld.Gameplay.Animals;
using ZooWorld.Gameplay.Animals.Combat;
using ZooWorld.Gameplay.Animals.Movement;
using ZooWorld.Gameplay.Events;
using ZooWorld.Gameplay.FX;
using ZooWorld.Gameplay.Spawning;
using ZooWorld.Gameplay.World;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.Infrastructure.Data;
using ZooWorld.UI.DeathCounter;

namespace ZooWorld.Gameplay.Level
{
    public class LevelScopeInstaller
    {
        private readonly DeathCounterView _deathCounterView;
        private readonly FloatingTextView _floatingTextPrefab;
        private readonly Transform _worldTextParent;

        public LevelScopeInstaller(
            DeathCounterView deathCounterView,
            FloatingTextView floatingTextPrefab,
            Transform worldTextParent)
        {
            _deathCounterView = deathCounterView;
            _floatingTextPrefab = floatingTextPrefab;
            _worldTextParent = worldTextParent;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.Register<IAnimalRegistry, AnimalRegistry>(Lifetime.Scoped);
            builder.Register<IFoodChainResolver, FoodChainResolver>(Lifetime.Scoped);
            builder.Register<ITastyFxBus, TastyFxBus>(Lifetime.Scoped);

            // Wrap the Addressables provider in a per-id pool — Instantiate/Destroy
            // only happens on the first spawn of each animal; after that we reuse.
            builder.Register<IAnimalViewProvider>(
                resolver => new PooledAnimalViewProvider(
                    new AddressablesAnimalViewProvider(resolver.Resolve<IAnimalViewCatalog>())),
                Lifetime.Scoped);

            builder.Register<AnimalControllerDependencies>(Lifetime.Scoped);

            // Movement strategy registry. New MovementType = one entry in this map.
            builder.Register<IMovementStrategyFactory>(
                resolver =>
                {
                    IScreenBoundsService bounds = resolver.Resolve<IScreenBoundsService>();
                    Dictionary<MovementType, Func<AnimalDataRow, IMovementStrategy>> map = new()
                    {
                        { MovementType.Linear, _ => new LinearMovement(bounds) },
                        { MovementType.Jump, _ => new JumpMovement(bounds) },
                    };
                    return new MovementStrategyFactory(map);
                },
                Lifetime.Scoped);

            // Per-subtype controller overrides. Empty by default — species that only
            // differ in stats/visuals are fully data-driven and don't need an entry.
            // To add a custom controller for a species, Register() it below.
            builder.Register<AnimalControllerBuilderRegistry>(
                _ =>
                {
                    AnimalControllerBuilderRegistry registry = new();
                    // registry.Register(new BossAnimalControllerBuilder());
                    return registry;
                },
                Lifetime.Scoped);

            builder.Register<IAnimalFactory, AnimalFactory>(Lifetime.Scoped);
            builder.Register<AnimalSpawner>(Lifetime.Scoped);
            builder.Register<DeathCounterModel>(Lifetime.Scoped);
            builder.Register<DeathCounterPresenter>(Lifetime.Scoped);

            FloatingTextView floatingTextPrefab = _floatingTextPrefab;
            Transform worldTextParent = _worldTextParent;

            if (_deathCounterView == null)
                Debug.LogError("[LevelScopeInstaller] DeathCounterView reference is missing. Assign it on RootLifetimeScope.");
            else
                builder.RegisterInstance(_deathCounterView);

            builder.Register<FloatingTextPool>(
                _ => new FloatingTextPool(floatingTextPrefab, worldTextParent),
                Lifetime.Scoped);

            builder.Register<DamageTextFactory>(Lifetime.Scoped);

            builder.Register<LevelInstaller>(Lifetime.Scoped);
        }
    }
}
