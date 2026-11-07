using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class DelayedCallback : MonoBehaviour, IDelayedCallbackManager
    {
        private readonly PriorityQueue<InvokableFunction> _callbacksQueue = new PriorityQueue<InvokableFunction>();
        private DateTime _nextCallbackTime = DateTime.MaxValue;

        public void Invoke(float delayInSeconds, Action action)
        {
            InvokableFunction func = new InvokableFunction(action, delayInSeconds);
            if (func.InvokeTime < _nextCallbackTime) _nextCallbackTime = func.InvokeTime;
            _callbacksQueue.Enqueue(func);
        }
        
        public bool Contains(Action method) 
            => _callbacksQueue.FirstOrDefault(c => c.Equals(method)) != null;

        public bool Remove(Action method)
        {
            InvokableFunction removableObject = _callbacksQueue.FirstOrDefault(c => c.Equals(method));
            if (removableObject == null) return false;
            return _callbacksQueue.Remove(removableObject);
        }

        private void FixedUpdate()
        {
            if(_nextCallbackTime > DateTime.UtcNow) return;
            if(_callbacksQueue.Count == 0) return;
            long now = DateTime.UtcNow.Ticks;
            while (now > _callbacksQueue.Peek().InvokeTime.Ticks)
            {
                InvokableFunction method =_callbacksQueue.Dequeue();
                method?.Action?.Invoke();
                if (_callbacksQueue.Count == 0) break;
            }
            _nextCallbackTime = _callbacksQueue.Count > 0 ? _callbacksQueue.Peek().InvokeTime : DateTime.MaxValue;
        }

        private class InvokableFunction : IComparable<InvokableFunction>, IEquatable<Action>
        {
            public readonly Action Action;
            public readonly DateTime InvokeTime;

            public InvokableFunction(Action action, float delayInSeconds)
            {
                Action = action;
                InvokeTime = DateTime.UtcNow + TimeSpan.FromSeconds(delayInSeconds);
            }

            public int CompareTo(InvokableFunction obj) 
                => InvokeTime > obj.InvokeTime ? 1 : InvokeTime < obj.InvokeTime ? -1 : 0;

            public bool Equals(Action other) 
                => other == Action;
        }
    }
    
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        public int Count => _count;

        private readonly List<T> _objects = new List<T>();
        private int _count;
        
        public void Enqueue(T obj)
        {
            int index = _objects.FindIndex(p => p.CompareTo(obj) >= 0);
            if (index < 0) _objects.Add(obj);
            else _objects.Insert(index, obj);
            _count++;
        }
        
        public T Dequeue()
        {
            T buf = Peek();
            _objects.RemoveAt(0);
            _count--;
            return buf;
        }

        public T Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return _objects[0];
        }

        public bool Remove(T removableObject)
        {
            T obj = _objects.Find(p => p.Equals(removableObject));
            if (obj == null) return false;
            _objects.Remove(obj);
            _count--;
            return true;
        }

        public IEnumerator<T> GetEnumerator() => _objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _objects.GetEnumerator();
    }

    public interface IDelayedCallbackManager
    {
        void Invoke(float delayInSeconds, Action action);
        bool Contains(Action method);
        bool Remove(Action method);
    }
}