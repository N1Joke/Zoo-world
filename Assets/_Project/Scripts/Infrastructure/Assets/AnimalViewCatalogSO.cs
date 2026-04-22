using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZooWorld.Infrastructure.Assets
{
    [CreateAssetMenu(menuName = "ZooWorld/Animal View Catalog", fileName = "AnimalViewCatalogSO")]
    public class AnimalViewCatalogSO : ScriptableObject, IAnimalViewCatalog
    {
        [Serializable]
        public class Entry
        {
            public int Id;
            public AssetReferenceGameObject Prefab;
        }

        [SerializeField] private List<Entry> _entries = new();

        public IReadOnlyList<int> AllIds
        {
            get
            {
                List<int> ids = new(_entries.Count);
                for (int i = 0; i < _entries.Count; i++) ids.Add(_entries[i].Id);
                return ids;
            }
        }

        public bool TryGetReference(int id, out AssetReferenceGameObject reference)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                Entry e = _entries[i];
                if (e.Id == id)
                {
                    reference = e.Prefab;
                    return reference != null;
                }
            }

            reference = null;
            return false;
        }
    }
}
