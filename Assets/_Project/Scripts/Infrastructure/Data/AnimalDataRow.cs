namespace ZooWorld.Infrastructure.Data
{
    public readonly struct AnimalDataRow
    {
        public readonly int Id;
        public readonly float Speed;
        public readonly float Damage;
        public readonly float Hp;
        public readonly AnimalRole Role;
        public readonly AnimalSubtype Subtype;
        public readonly float JumpInterval;
        public readonly float JumpDistance;
        public readonly MovementType MovementType;

        public AnimalDataRow(
            int id,
            float speed,
            float damage,
            float hp,
            AnimalRole role,
            AnimalSubtype subtype,
            float jumpInterval,
            float jumpDistance,
            MovementType movementType)
        {
            Id = id;
            Speed = speed;
            Damage = damage;
            Hp = hp;
            Role = role;
            Subtype = subtype;
            JumpInterval = jumpInterval;
            JumpDistance = jumpDistance;
            MovementType = movementType;
        }
    }
}
