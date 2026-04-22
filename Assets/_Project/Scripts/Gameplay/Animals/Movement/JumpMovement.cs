using System;
using Tools.Extensions;
using UnityEngine;
using ZooWorld.Gameplay.World;

namespace ZooWorld.Gameplay.Animals.Movement
{
    // Hops every JumpInterval seconds for JumpDistance units.
    // Only jumps when grounded — otherwise back-to-back ticks would stack
    // upward velocity while the animal is still in the air.
    public class JumpMovement : IMovementStrategy
    {
        private const float GroundProbeDistance = 0.2f;
        private const float GroundProbeOriginOffset = 0.02f;

        private readonly IScreenBoundsService _bounds;
        private AnimalView _view;
        private AnimalModel _model;
        private Vector3 _direction;
        private IDisposable _timer;
        private bool _isPaused;

        public JumpMovement(IScreenBoundsService bounds)
        {
            _bounds = bounds;
        }

        public void Initialize(AnimalView view, AnimalModel model)
        {
            _view = view;
            _model = model;
            _direction = RandomHorizontalDir();

            float interval = Mathf.Max(0.1f, _model.JumpInterval);
            _timer = ReactiveExtensions.RepeatableDelayedCall(interval, Jump);
        }

        public void Tick(float deltaTime)
        {
        }

        public void SetPaused(bool paused) => _isPaused = paused;

        private void Jump()
        {
            if (_view == null || _view.Rigidbody == null) return;
            if (_isPaused) return;
            if (!IsGrounded()) return;

            if (_bounds.IsOutside(_view.transform.position))
                _direction = _bounds.DirectionToCenter(_view.transform.position);
            else if (UnityEngine.Random.value < 0.35f)
                _direction = RandomHorizontalDir();

            float distance = Mathf.Max(0.1f, _model.JumpDistance);
            float interval = Mathf.Max(0.1f, _model.JumpInterval);
            float horizontalSpeed = distance / interval;
            float verticalSpeed = Mathf.Clamp(distance * 2.5f, 2f, 8f);

            Vector3 velocity = _direction * horizontalSpeed;
            velocity.y = verticalSpeed;
            _view.Rigidbody.linearVelocity = velocity;

            if (_direction.sqrMagnitude > 0.0001f)
                _view.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
        }

        private bool IsGrounded()
        {
            Collider col = _view.Collider;
            Vector3 origin = col != null
                ? new Vector3(col.bounds.center.x, col.bounds.min.y + GroundProbeOriginOffset, col.bounds.center.z)
                : _view.transform.position;

            return Physics.Raycast(
                origin,
                Vector3.down,
                GroundProbeDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
        }

        private static Vector3 RandomHorizontalDir()
        {
            float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            return new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
            _view = null;
            _model = null;
        }
    }
}
