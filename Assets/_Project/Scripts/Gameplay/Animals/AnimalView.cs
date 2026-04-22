using Tools.Extensions;
using UnityEngine;

namespace ZooWorld.Gameplay.Animals
{
    // Pure view: holds Unity component refs and proxies collision events.
    // No gameplay logic here.
    [RequireComponent(typeof(Rigidbody))]
    public class AnimalView : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _visualRoot;

        private readonly ReactiveEvent<AnimalView> _onCollidedWith = new();

        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;
        public Transform VisualRoot => _visualRoot != null ? _visualRoot : transform;
        public IReadOnlyReactiveEvent<AnimalView> OnCollidedWith => _onCollidedWith;

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _visualRoot = transform;
        }

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if (_collider == null) _collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            AnimalView other = collision.collider.GetComponentInParent<AnimalView>();
            if (other != null && other != this)
                _onCollidedWith.Notify(other);
        }

        private void OnDestroy()
        {
            _onCollidedWith.Dispose();
        }
    }
}
