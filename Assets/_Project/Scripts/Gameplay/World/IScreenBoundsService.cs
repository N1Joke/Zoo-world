using UnityEngine;

namespace ZooWorld.Gameplay.World
{
    public interface IScreenBoundsService
    {
        // Play area center on the ground plane (same y as spawn).
        Vector3 Center { get; }

        // True if the world position (projected onto the ground) is outside the play area.
        bool IsOutside(Vector3 worldPosition);

        // A random point inside the play area.
        Vector3 GetRandomPointInside();

        // Horizontal direction from the given position back toward the center.
        Vector3 DirectionToCenter(Vector3 worldPosition);
    }
}
