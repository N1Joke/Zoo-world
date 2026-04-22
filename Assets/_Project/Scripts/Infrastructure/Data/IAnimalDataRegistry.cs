using System.Collections.Generic;

namespace ZooWorld.Infrastructure.Data
{
    public interface IAnimalDataRegistry
    {
        int Count { get; }
        bool TryGet(int id, out AnimalDataRow row);
        IReadOnlyList<AnimalDataRow> All { get; }
    }
}
