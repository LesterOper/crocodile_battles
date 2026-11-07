using System;
using System.Collections.Generic;

namespace Library.Collections
{
    public abstract class Pool<T>
    {
        private readonly Queue<T> _freeObjects;

        public Pool() : this(16, int.MaxValue)
        {
        }

        public Pool(int initialCapacity) : this(initialCapacity, int.MaxValue)
        {
        }

        public Pool(int initialCapacity, int max)
        {
            _freeObjects = new Queue<T>(initialCapacity);
            Max = max;
        }

        public int Max { get; }
        public int Peak { get; private set; }

        protected abstract T NewObject();

        public T Obtain()
        {
            return _freeObjects.Count == 0 ? NewObject() : _freeObjects.Dequeue();
        }

        public void Free(T poolObject)
        {
            if (poolObject == null)
                throw new SystemException("object cannot be null.");

            if (_freeObjects.Count < Max)
            {
                _freeObjects.Enqueue(poolObject);
                Peak = Math.Max(Peak, _freeObjects.Count);
            }

            Reset(poolObject);
        }

        protected void Reset(T poolObject)
        {
            if (poolObject is IPoolable)
                ((IPoolable) poolObject).Reset();
        }

        public void Clear()
        {
            foreach (var poolObject in _freeObjects) Destroy(poolObject);
            _freeObjects.Clear();
        }

        protected virtual void Destroy(T poolObject)
        {
        }

        public int GetFree()
        {
            return _freeObjects.Count;
        }
    }

    public interface IPoolable
    {
        void Reset();
    }
}