using System;
using UniRx;

namespace Tools.Extensions
{
    public class ReactiveEvent : IReadOnlyReactiveEvent, IDisposable
    {
        private readonly UniRx.ReactiveProperty<int> _property;

        public ReactiveEvent()
        {
            _property = new UniRx.ReactiveProperty<int>(0);
        }

        public void Dispose()
        {
            _property?.Dispose();
        }

        public IDisposable Subscribe(Action action)
        {
            return _property.Subscribe(_ => action?.Invoke());
        }

        public IDisposable SubscribeWithSkip(Action action)
        {
            return _property.SkipLatestValueOnSubscribe().Subscribe(_ => action?.Invoke());
        }

        public IDisposable SubscribeOnceWithSkip(Action action)
        {
            return _property.SkipLatestValueOnSubscribe().Take(1).Subscribe(_ => action?.Invoke());
        }

        public void Notify()
        {
            _property.SetValueAndForceNotify(0);
        }
    }

    public class ReactiveEvent<T> : IReadOnlyReactiveEvent<T>, IDisposable
    {
        private readonly UniRx.ReactiveProperty<T> _property;

        public ReactiveEvent()
        {
            _property = new UniRx.ReactiveProperty<T>();
        }

        public ReactiveEvent(T init)
        {
            _property = new UniRx.ReactiveProperty<T>(init);
        }

        public void Dispose()
        {
            _property?.Dispose();
        }

        public IDisposable Subscribe(Action<T> action)
        {
            return _property.Subscribe(action);
        }
        
        public IDisposable SubscribeOnce(Action<T> action)
        {
            return _property.Take(1).Subscribe(action);
        }
        public IDisposable SubscribeWithSkip(Action<T> action)
        {
            return _property.SkipLatestValueOnSubscribe().Subscribe(action);
        }

        public IDisposable SubscribeOnceWithSkip(Action<T> action)
        {
            return _property.SkipLatestValueOnSubscribe().Take(1).Subscribe(action);
        }
        
        public IDisposable SubscribeWithTwiceSkip(Action<T> action)
        {
            return _property.SkipLatestValueOnSubscribe().Skip(2).Take(1).Subscribe(action);
        }

        public void Notify(T obj)
        {
            _property.SetValueAndForceNotify(obj);
        }
    }

    public class ReactiveEvent<T1, T2> : IReadOnlyReactiveEvent<T1, T2>, IDisposable
    {
        private struct Entry
        {
            public T1 arg1;
            public T2 arg2;
        }

        private readonly UniRx.ReactiveProperty<Entry> _property;

        public ReactiveEvent()
        {
            _property = new UniRx.ReactiveProperty<Entry>();
        }

        public ReactiveEvent(T1 arg1, T2 arg2)
        {
            _property = new UniRx.ReactiveProperty<Entry>(new Entry { arg1 = arg1, arg2 = arg2 });
        }

        public void Dispose()
        {
            _property?.Dispose();
        }

        public IDisposable Subscribe(Action<T1, T2> action)
        {
            return _property.Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2));
        }

        public IDisposable SubscribeWithSkip(Action<T1, T2> action)
        {
            return _property.SkipLatestValueOnSubscribe()
                .Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2));
        }

        public IDisposable SubscribeOnceWithSkip(Action<T1, T2> action)
        {
            return _property.SkipLatestValueOnSubscribe().Take(1)
                .Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2));
        }

        public void Notify(T1 arg1, T2 arg2)
        {
            _property.SetValueAndForceNotify(new Entry { arg1 = arg1, arg2 = arg2 });
        }

        private IDisposable Subscripe(Action<Entry> entry)
        {
            return _property.Subscribe(entry);
        }
    }

    public class ReactiveEvent<T1, T2, T3> : IReadOnlyReactiveEvent<T1, T2, T3>, IDisposable
    {
        private struct Entry
        {
            public T1 arg1;
            public T2 arg2;
            public T3 arg3;
        }

        private readonly UniRx.ReactiveProperty<Entry> _property;

        public ReactiveEvent()
        {
            _property = new UniRx.ReactiveProperty<Entry>();
        }

        public ReactiveEvent(T1 arg1, T2 arg2, T3 arg3)
        {
            _property = new UniRx.ReactiveProperty<Entry>(new Entry { arg1 = arg1, arg2 = arg2, arg3 = arg3 });
        }

        public void Dispose()
        {
            _property?.Dispose();
        }

        public IDisposable Subscribe(Action<T1, T2, T3> action)
        {
            return _property.Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2, entry.arg3));
        }

        public IDisposable SubscribeWithSkip(Action<T1, T2, T3> action)
        {
            return _property.SkipLatestValueOnSubscribe()
                .Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2, entry.arg3));
        }

        public IDisposable SubscribeOnceWithSkip(Action<T1, T2, T3> action)
        {
            return _property.SkipLatestValueOnSubscribe().Take(1)
                .Subscribe(entry => action?.Invoke(entry.arg1, entry.arg2, entry.arg3));
        }

        public void Notify(T1 arg1, T2 arg2, T3 arg3)
        {
            _property.SetValueAndForceNotify(new Entry { arg1 = arg1, arg2 = arg2, arg3 = arg3 });
        }

        private IDisposable Subscripe(Action<Entry> entry)
        {
            return _property.Subscribe(entry);
        }
    }
}