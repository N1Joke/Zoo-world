using System;

namespace ZooWorld.Gameplay.Animals.Movement
{
    public interface IMovementStrategy : IDisposable
    {
        void Initialize(AnimalView view, AnimalModel model);
        void Tick(float deltaTime);

        // While paused the strategy must not touch the Rigidbody so external
        // forces (e.g. knockback) can resolve without being overwritten.
        void SetPaused(bool paused);
    }
}
