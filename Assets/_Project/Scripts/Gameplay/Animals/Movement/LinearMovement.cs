using UnityEngine;
using ZooWorld.Gameplay.World;

namespace ZooWorld.Gameplay.Animals.Movement
{
    // Moves at constant speed in the current direction.
    // Redirects toward the center when the animal steps outside the play area.
    public class LinearMovement : IMovementStrategy
    {
        private readonly IScreenBoundsService _bounds;
        private AnimalView _view;
        private AnimalModel _model;
        private Vector3 _direction;
        private bool _isPaused;

        public LinearMovement(IScreenBoundsService bounds)
        {
            _bounds = bounds;
        }

        public void Initialize(AnimalView view, AnimalModel model)
        {
            _view = view;
            _model = model;
            _direction = RandomHorizontalDir();
            ApplyVelocity();
        }

        public void Tick(float deltaTime)
        {
            if (_view == null || _isPaused) return;

            if (_bounds.IsOutside(_view.transform.position))
                _direction = _bounds.DirectionToCenter(_view.transform.position);

            ApplyVelocity();
            RotateToDirection();
        }

        public void SetPaused(bool paused) => _isPaused = paused;

        private void ApplyVelocity()
        {
            if (_view.Rigidbody == null) return;
            Vector3 vel = _direction * _model.Speed;
            Vector3 cur = _view.Rigidbody.linearVelocity;
            _view.Rigidbody.linearVelocity = new Vector3(vel.x, cur.y, vel.z);
        }

        private void RotateToDirection()
        {
            if (_direction.sqrMagnitude < 0.0001f) return;
            Quaternion target = Quaternion.LookRotation(_direction, Vector3.up);
            _view.transform.rotation = Quaternion.Slerp(_view.transform.rotation, target, 0.2f);
        }

        private static Vector3 RandomHorizontalDir()
        {
            float a = Random.Range(0f, Mathf.PI * 2f);
            return new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
        }

        public void Dispose()
        {
            _view = null;
            _model = null;
        }
    }
}
