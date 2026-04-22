using UniRx;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    public class AnimalModel
    {
        public AnimalDataRow Data { get; }
        public ReactiveProperty<bool> IsAlive { get; } = new(true);

        public int Id => Data.Id;
        public AnimalRole Role => Data.Role;
        public AnimalSubtype Subtype => Data.Subtype;
        public MovementType MovementType => Data.MovementType;
        public float Speed => Data.Speed;
        public float JumpInterval => Data.JumpInterval;
        public float JumpDistance => Data.JumpDistance;
        public float Hp => Data.Hp;

        public AnimalModel(AnimalDataRow data)
        {
            Data = data;
        }
    }
}
