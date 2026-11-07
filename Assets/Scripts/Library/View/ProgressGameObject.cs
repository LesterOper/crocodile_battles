using Library.Collections;
using UnityEngine;

namespace Library.View
{
    public abstract class ProgressGameObject : MonoBehaviour, IPoolable
    {
        public delegate void CompleteHandler(GameObject gameObject);

        private bool _active;
        [SerializeField] private float _duration = 0.6f;

        private float _progress;
        private float _timer;

        public float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = Mathf.Clamp01(value);
                UpdateProgress();
            }
        }

        public IInterpolator Interpolator { get; set; }

        public virtual void Reset()
        {
            _progress = 0;
        }

        public event CompleteHandler _onComplete;

        protected abstract void OnProgressChange(float progress);

        public virtual void Run(CompleteHandler completeHandler)
        {
            Reset();
            _onComplete = completeHandler;

            if (Duration > 0)
            {
                _progress = 0;
                _active = true;
            }
            else
            {
                _progress = 1;
                _active = false;
            }

            UpdateProgress();
        }

        private void Update()
        {
            if (_active)
            {
                _progress = Mathf.Clamp01(_progress + Time.deltaTime / Duration);
                UpdateProgress();

                if (_progress >= 1f)
                {
                    _active = false;
                    _onComplete?.Invoke(gameObject);
                }
            }
        }

        public virtual void Stop()
        {
            _active = false;
        }

        private void UpdateProgress()
        {
            var stateProgress = Interpolator == null ? _progress : Interpolator.Apply(_progress);
            OnProgressChange(stateProgress);
        }
    }
}