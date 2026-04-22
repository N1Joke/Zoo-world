using System.Collections.Generic;
using Core;
using ZooWorld.Gameplay.Animals;
using ZooWorld.Gameplay.FX;
using ZooWorld.Gameplay.Spawning;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.UI.DeathCounter;

namespace ZooWorld.Gameplay.Level
{
    // Main gameplay entry point for the level. Holds strong references to all
    // level controllers/services and disposes them when the level ends.
    public class LevelInstaller : BaseDisposable
    {
        private readonly AnimalSpawner _spawner;
        private readonly List<System.IDisposable> _controllers = new();

        public LevelInstaller(
            AnimalSpawner spawner,
            DeathCounterPresenter deathPresenter,
            DamageTextFactory damageTextFactory,
            IAnimalRegistry animalRegistry,
            IAnimalViewProvider viewProvider)
        {
            _spawner = spawner;

            _controllers.Add(AddDispose(damageTextFactory));
            _controllers.Add(AddDispose(deathPresenter));
            _controllers.Add(AddDispose(_spawner));
            _controllers.Add(AddDispose(animalRegistry));
            _controllers.Add(AddDispose(viewProvider));
        }

        public void StartGameplay()
        {
            if (isDisposed) return;
            _spawner.Start();
        }

        public void Attach(System.IDisposable controller)
        {
            if (controller == null || isDisposed) return;
            _controllers.Add(AddDispose(controller));
        }
    }
}
