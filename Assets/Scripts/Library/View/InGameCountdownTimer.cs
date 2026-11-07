using System;
using Library.Text;
using UnityEngine;

namespace Library.View
{
    [RequireComponent(typeof(TextAdapter))]
    public class InGameCountdownTimer : MonoBehaviour
    {
        private const long UPDATE_INTERVAL = 1;
        public const long BONUS_DURATION = 5 * 60;
        private long _durationTicks;
        private bool _inProgress;
        private long _rewindSpeed;
        private long _startTimeTicks;
        private bool _stopped;
        private bool _textChecked;
        [Tooltip("d:h:m:s")] [SerializeField] private string _timeFormatDays;
        [Tooltip("h:m:s")] [SerializeField] private string _timeFormatHours;
        [Tooltip("m:s")] [SerializeField] private string _timeFormatMinutes;
        private double _timeLeft;
        private double _timeLeftPause;
        private double _updatedTimeLeft = double.MinValue;
        public long DisplayTimeScale { get; set; } = 1;
        public int TimeLeft => (int) _timeLeft;
        public int TimeLeftPause => (int) _timeLeftPause;
        public TextAdapter Text { get; private set; }

        public event Action OnCountdownFinish;

        private void Awake()
        {
            Text = GetComponent<TextAdapter>();

            if (string.IsNullOrEmpty(_timeFormatDays))
                _timeFormatDays = "{0:00}:{1:00}:{2:00}:{3:00}";
            if (string.IsNullOrEmpty(_timeFormatHours))
                _timeFormatHours = "{0:00}:{1:00}:{2:00}";
            if (string.IsNullOrEmpty(_timeFormatMinutes))
                _timeFormatMinutes = "{0:00}:{1:00}";
            _textChecked = true;
        }

        public void StartCountdown(double duration)
        {
            if (duration > 600)
            {
                /*if (PremPurchaseUtils.IsPurchased(PremPurchaseType.VIP))
                    duration = BONUS_DURATION * 2;
                else
                    duration = BONUS_DURATION;*/
            }

            _timeLeft = duration;
            _inProgress = true;
            _stopped = false;
        }

        public void AppendTicks(long ticks)
        {
            _startTimeTicks = _startTimeTicks + ticks;
        }

        public void StopCountdown()
        {
            _timeLeftPause = _timeLeft;
            _timeLeft = 0;
            _stopped = true;
        }

        public bool IsActive()
        {
            return _inProgress && !_stopped;
        }

        private void Update()
        {
            if (_inProgress && !_stopped && _textChecked)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft > 0)
                {
                    if (Math.Abs(_timeLeft - _updatedTimeLeft) >= UPDATE_INTERVAL)
                    {
                        var displayTimeLeft = _timeLeft * DisplayTimeScale;
                        var minutes = Mathf.FloorToInt((float) displayTimeLeft / 60F);
                        var seconds = Mathf.FloorToInt((float) displayTimeLeft - minutes * 60);
                        var niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
                        Text.SetText(niceTime);
                        _updatedTimeLeft = _timeLeft;
                    }
                }
                else
                {
                    Text.SetText(_timeFormatMinutes, 0, 0);
                    _timeLeftPause = 0;
                    _inProgress = false;
                    OnCountdownFinish?.Invoke();
                }
            }
        }
    }
}