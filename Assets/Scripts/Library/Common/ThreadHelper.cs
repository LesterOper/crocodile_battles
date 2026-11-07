using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class ThreadHelper : MonoBehaviour
    {
        private volatile Queue<Action> _executionQueue = new Queue<Action>();

        private void Update()
        {
            if (_executionQueue.Count > 0)
                lock (_executionQueue)
                {
                    while (_executionQueue.Count > 0) _executionQueue.Dequeue().Invoke();
                }
        }

        public void Enqueue(IEnumerator action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(() => StartCoroutine(action));
            }
        }

        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        private IEnumerator ActionWrapper(Action action)
        {
            action();
            yield return null;
        }
    }
}