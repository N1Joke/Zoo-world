using System;
using System.Threading;
using Core;
using Tools.Extensions;
using UnityEngine;
using ZooWorld.Gameplay.Animals.Combat;
using ZooWorld.Gameplay.Animals.Movement;
using ZooWorld.Infrastructure.Data;

namespace ZooWorld.Gameplay.Animals
{
    public abstract class AnimalControllerBase : BaseDisposable
    {
        private const float PreyKnockbackImpulse = 3.5f;
        private const float PreyKnockbackUp = 1.5f;
        private const float PreyBumpPauseSeconds = 0.5f;

        private static int _instanceCounter;

        public int InstanceId { get; }
        public AnimalModel Model { get; }
        public AnimalView View { get; }

        protected readonly IMovementStrategy Movement;
        protected readonly AnimalControllerDependencies Deps;

        private IDisposable _bumpTimer;

        protected AnimalControllerBase(
            AnimalModel model,
            AnimalView view,
            IMovementStrategy movement,
            AnimalControllerDependencies dependencies)
        {
            InstanceId = Interlocked.Increment(ref _instanceCounter);
            Model = model;
            View = view;
            Movement = movement;
            Deps = dependencies;
        }

        public void Initialize()
        {
            Movement.Initialize(View, Model);
            AddDispose(Movement);

            AddDispose(View.OnCollidedWith.SubscribeWithSkip(OnCollidedWith));
            AddDispose(ReactiveExtensions.StartUpdate(Tick));
        }

        private void Tick()
        {
            if (isDisposed || View == null) return;

            Movement.Tick(Time.deltaTime);
        }

        private void OnCollidedWith(AnimalView otherView)
        {
            if (isDisposed || otherView == null) return;
            if (!Deps.Registry.TryGet(otherView, out AnimalControllerBase other)) return;
            if (other.IsDisposed) return;

            // Unity fires OnCollisionEnter on both sides, so without this guard
            // both would call Resolve() and the loser could die before the winner
            // emits Tasty. Lower InstanceId owns the pair for this frame.
            if (InstanceId > other.InstanceId) return;

            if (Model.Role == AnimalRole.Prey && other.Model.Role == AnimalRole.Prey)
            {
                ApplyPreyKnockback(other);
                return;
            }

            FoodChainOutcome outcome = Deps.FoodChain.Resolve(this, other);
            if (!outcome.SelfDies && !outcome.OtherDies) return;

            if (outcome.Tasty)
                Deps.TastyBus?.Notify(outcome.TastyPosition);

            if (outcome.OtherDies) other.Kill();
            if (outcome.SelfDies) Kill();
        }

        private void ApplyPreyKnockback(AnimalControllerBase other)
        {
            if (View == null || other.View == null) return;

            Vector3 delta = View.transform.position - other.View.transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude < 0.0001f)
            {
                float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                delta = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
            }
            Vector3 dir = delta.normalized;

            PushAndPause(dir);
            other.PushAndPause(-dir);
        }

        // Applies a knockback impulse along dir and briefly pauses movement.
        // Called on self by the arbitrating side and on the peer so both animals
        // bounce from a single resolver pass.
        private void PushAndPause(Vector3 dir)
        {
            if (isDisposed || View == null) return;
            Rigidbody rb = View.Rigidbody;
            if (rb == null) return;

            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, v.y, 0f);
            rb.AddForce(dir * PreyKnockbackImpulse + Vector3.up * PreyKnockbackUp, ForceMode.Impulse);

            Movement.SetPaused(true);
            _bumpTimer?.Dispose();
            _bumpTimer = ReactiveExtensions.DelayedCall(PreyBumpPauseSeconds, ResumeFromBump);
        }

        private void ResumeFromBump()
        {
            if (isDisposed) return;
            Movement.SetPaused(false);
        }

        public void Kill()
        {
            if (isDisposed) return;
            Model.IsAlive.Value = false;
            Deps.DeathBus?.Notify(Model.Role);
            Dispose();
        }

        protected override void OnDispose()
        {
            _bumpTimer?.Dispose();
            _bumpTimer = null;
            Deps.Registry?.Unregister(this);
            if (View != null)
                Deps.ViewProvider?.Release(View);
        }
    }
}
