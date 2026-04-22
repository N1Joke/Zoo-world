using System;
using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals.Movement
{
    // Maps MovementType to a factory delegate. The delegate closes over whatever
    // dependencies each strategy needs (bounds, etc.), so adding a new movement
    // type is a single registration in LevelScopeInstaller.
    public class MovementStrategyFactory : IMovementStrategyFactory
    {
        private readonly IReadOnlyDictionary<MovementType, Func<AnimalDataRow, IMovementStrategy>> _map;
        private readonly Func<AnimalDataRow, IMovementStrategy> _fallback;

        public MovementStrategyFactory(
            IReadOnlyDictionary<MovementType, Func<AnimalDataRow, IMovementStrategy>> map,
            MovementType fallback = MovementType.Linear)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            if (!_map.TryGetValue(fallback, out _fallback))
                Debug.LogError($"[MovementStrategyFactory] Fallback MovementType '{fallback}' is not registered. Animals with unknown movement will fail to spawn.");
        }

        public IMovementStrategy Create(AnimalDataRow row)
        {
            if (_map.TryGetValue(row.MovementType, out var factory))
                return factory(row);

            Debug.LogError($"[MovementStrategyFactory] No strategy registered for '{row.MovementType}' (animal id {row.Id}). Using fallback.");
            return _fallback?.Invoke(row);
        }
    }
}
