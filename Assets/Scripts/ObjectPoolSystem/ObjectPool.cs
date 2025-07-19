using System;
using System.Collections.Generic;

namespace ObjectPoolSystem
{
    public class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly HashSet<T> _activeObjects = new HashSet<T>();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;
        private readonly Action<T> _onDestroy;
        private readonly int _maxSize;

        public int ActiveCount => _activeObjects.Count;
        public int InactiveCount => _pool.Count;

        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onReturn = null,
            Action<T> onDestroy = null,
            int maxSize = 1000)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onReturn = onReturn;
            _onDestroy = onDestroy;
            _maxSize = maxSize;
        }

        public T Get()
        {
            T item;

            if (_pool.Count > 0)
            {
                item = _pool.Dequeue();
            }
            else
            {
                item = _createFunc();
            }

            _activeObjects.Add(item);
            _onGet?.Invoke(item);

            return item;
        }

        public void Return(T item)
        {
            if (item == null || !_activeObjects.Contains(item))
                return;

            _activeObjects.Remove(item);

            if (_pool.Count < _maxSize)
            {
                _onReturn?.Invoke(item);
                _pool.Enqueue(item);
            }
            else
            {
                _onDestroy?.Invoke(item);
            }
        }

        public void PreWarm(int count)
        {
            var items = new List<T>();

            for (int i = 0; i < count; i++)
            {
                items.Add(Get());
            }

            foreach (var item in items)
            {
                Return(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _pool)
            {
                _onDestroy?.Invoke(item);
            }

            foreach (var item in _activeObjects)
            {
                _onDestroy?.Invoke(item);
            }

            _pool.Clear();
            _activeObjects.Clear();
        }
    }
}