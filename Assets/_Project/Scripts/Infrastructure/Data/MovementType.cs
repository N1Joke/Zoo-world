namespace ZooWorld.Infrastructure.Data
{
    // Movement type for an animal. Rename entries carefully — CSV parses by name.
    public enum MovementType
    {
        Unknown = 0,
        Linear = 1,
        Jump = 2,
    }
}
