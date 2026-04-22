using System;
using UniRx;
using UnityEngine;

namespace Tools.Extensions
{
    public static class ReactiveExtensions
    {
        public static IDisposable DelayedCall(float delaySec, Action action)
        {
            if (delaySec <= 0)
            {
                action?.Invoke();
                return null;
            }
            return Observable.Timer(TimeSpan.FromSeconds(delaySec)).Take(1).Subscribe(_ => action?.Invoke());
        }

        public static IDisposable RepeatableDelayedCall(float delaySec, Action action)
        {
            if (delaySec <= 0)
            {
                action?.Invoke();
                return null;
            }

            return Observable.Interval(TimeSpan.FromSeconds(delaySec)).Subscribe(_ => action?.Invoke());
        }

        //public static int lastFrame = -1;

        public static IDisposable StartUpdate(Action onIteration)
        {
            int localLastFrame = -1;

            return Observable.EveryUpdate()
                .Where(_ =>
                {
                    if (localLastFrame == Time.frameCount) return false;
                    localLastFrame = Time.frameCount;
                    return true;
                })
                .Subscribe(_ => onIteration?.Invoke());
        }

        public static IDisposable StartLateUpdate(Action onIteration)
        {
            return Observable.EveryLateUpdate().Subscribe(_ => { onIteration?.Invoke(); });
        }

        public static IDisposable StartFixedUpdate(Action onIteration)
        {
            return Observable.EveryFixedUpdate().Subscribe(_ => { onIteration?.Invoke(); });
        }
    }
}