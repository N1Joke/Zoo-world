using System.Collections.Generic;

namespace ZooWorld.Gameplay.Animals
{
    public class AnimalRegistry : IAnimalRegistry
    {
        private readonly Dictionary<AnimalView, AnimalControllerBase> _byView = new();
        private bool _disposed;

        public IReadOnlyCollection<AnimalControllerBase> All => _byView.Values;

        public void Register(AnimalControllerBase controller)
        {
            if (_disposed || controller == null || controller.View == null) return;
            _byView[controller.View] = controller;
        }

        public void Unregister(AnimalControllerBase controller)
        {
            if (controller == null || controller.View == null) return;
            _byView.Remove(controller.View);
        }

        public bool TryGet(AnimalView view, out AnimalControllerBase controller)
        {
            if (view == null)
            {
                controller = null;
                return false;
            }
            return _byView.TryGetValue(view, out controller);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            List<AnimalControllerBase> snapshot = new(_byView.Values);
            _byView.Clear();
            for (int i = snapshot.Count - 1; i >= 0; i--)
                snapshot[i]?.Dispose();
        }
    }
}
