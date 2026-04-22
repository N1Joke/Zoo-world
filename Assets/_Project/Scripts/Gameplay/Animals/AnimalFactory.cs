using Cysharp.Threading.Tasks;
using UnityEngine;
using ZooWorld.Gameplay.Animals.Movement;
using ZooWorld.Infrastructure.Assets;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    // Two creation paths: if a subtype has a registered builder, delegate to it
    // (bosses, unique logic, etc.); otherwise build a GenericAnimalController
    // straight from the CSV row. Adding a normal species = CSV row + Addressables
    // entry, no code changes needed.
    public class AnimalFactory : IAnimalFactory
    {
        private readonly IAnimalViewProvider _viewProvider;
        private readonly IAnimalRegistry _registry;
        private readonly AnimalControllerDependencies _dependencies;
        private readonly IMovementStrategyFactory _movementFactory;
        private readonly AnimalControllerBuilderRegistry _builderRegistry;

        public AnimalFactory(
            AnimalControllerDependencies dependencies,
            IMovementStrategyFactory movementFactory,
            AnimalControllerBuilderRegistry builderRegistry)
        {
            _dependencies = dependencies;
            _viewProvider = dependencies.ViewProvider;
            _registry = dependencies.Registry;
            _movementFactory = movementFactory;
            _builderRegistry = builderRegistry;
        }

        public async UniTask<AnimalControllerBase> CreateAsync(AnimalDataRow row, Vector3 position)
        {
            AnimalView view = await _viewProvider.LoadViewAsync(row.Id, position, Quaternion.identity);
            if (view == null) return null;

            AnimalModel model = new(row);
            AnimalControllerBase controller = BuildController(row, model, view);
            if (controller == null)
            {
                _viewProvider.Release(view);
                return null;
            }

            _registry.Register(controller);
            controller.Initialize();
            return controller;
        }

        private AnimalControllerBase BuildController(AnimalDataRow row, AnimalModel model, AnimalView view)
        {
            if (_builderRegistry != null
                && _builderRegistry.TryGet(row.Subtype, out IAnimalControllerBuilder custom))
                return custom.Build(model, view, _dependencies);

            IMovementStrategy movement = _movementFactory.Create(row);
            if (movement == null)
            {
                Debug.LogError($"[AnimalFactory] No movement strategy for animal id {row.Id} (subtype '{row.Subtype}', movement '{row.MovementType}').");
                return null;
            }

            return new GenericAnimalController(model, view, movement, _dependencies);
        }
    }
}
