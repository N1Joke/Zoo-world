using System.Collections.Generic;

namespace ZooWorld.Infrastructure.Data
{
    public class AnimalDataRegistry : IAnimalDataRegistry
    {
        private readonly Dictionary<int, AnimalDataRow> _rowsById = new();
        private readonly List<AnimalDataRow> _all = new();

        public int Count => _all.Count;
        public IReadOnlyList<AnimalDataRow> All => _all;

        public void Populate(IEnumerable<AnimalDataRow> rows)
        {
            _rowsById.Clear();
            _all.Clear();
            foreach (AnimalDataRow row in rows)
            {
                _rowsById[row.Id] = row;
                _all.Add(row);
            }
        }

        public bool TryGet(int id, out AnimalDataRow row) => _rowsById.TryGetValue(id, out row);
    }
}
