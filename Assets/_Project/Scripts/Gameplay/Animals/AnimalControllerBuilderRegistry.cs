using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    // Registry of custom controllers keyed by subtype.
    // Populated in LevelScopeInstaller. Empty by default — species that only
    // differ in stats/visuals don't need an entry here.
    public class AnimalControllerBuilderRegistry
    {
        private readonly Dictionary<AnimalSubtype, IAnimalControllerBuilder> _map = new();

        public int Count => _map.Count;

        public void Register(IAnimalControllerBuilder builder)
        {
            if (builder == null) return;
            if (_map.ContainsKey(builder.Subtype))
            {
                Debug.LogError($"[AnimalControllerBuilderRegistry] Duplicate controller builder for subtype '{builder.Subtype}'. Ignoring later registration.");
                return;
            }
            _map[builder.Subtype] = builder;
        }

        public bool TryGet(AnimalSubtype subtype, out IAnimalControllerBuilder builder) =>
            _map.TryGetValue(subtype, out builder);
    }
}
