using Library.Text;
using Library.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Library.Dialogs
{
    public class ChooseCountView : MonoBehaviour
    {
        public delegate void CountChangeHandler(float rate, long count);

        public delegate long ResultValueCalculator(float rate, long count);

        [SerializeField] private TextAdapter _countText;
        [SerializeField] private string _countTextKey;

        [SerializeField] private Button _lessButton;

        private long _maxCount;
        private long _minCount;
        [SerializeField] private Button _moreButton;

        [Tooltip("Can be null")] [SerializeField]
        private TextAdapter _rateText;

        [SerializeField] private string _rateTextKey;
        [SerializeField] private string _resultTextKey;

        private ResultValueCalculator _resultValueCalculator;

        [Tooltip("Can be null")] [SerializeField]
        private TextAdapter _resultValueText;

        public long ChangeStep { get; set; } = 0;

        public long Count { get; private set; }
        public float Rate { get; private set; }

        public string RateTextKey
        {
            get => _rateTextKey;
            set
            {
                _rateTextKey = value;
                UpdateView();
            }
        }

        public string CountTextKey
        {
            get => _countTextKey;
            set
            {
                _countTextKey = value;
                UpdateView();
            }
        }

        public string ResultTextKey
        {
            get => _resultTextKey;
            set
            {
                _resultTextKey = value;
                UpdateView();
            }
        }

        public TextAdapter CountText => _countText;

        public event CountChangeHandler OnCountChanged;

        protected virtual void Start()
        {
            _lessButton.onClick.AddListener(Less);
            _moreButton.onClick.AddListener(More);
        }

        public void Setup(float rate, long minCount, long maxCount, ResultValueCalculator calcResultValue = null)
        {
            Rate = rate;

            if (_rateText != null)
                _rateText.SetText(_rateTextKey, rate);

            _minCount = minCount;
            _maxCount = maxCount;

            _resultValueCalculator = calcResultValue;

            Count = _maxCount;

            UpdateView();

            OnCountChanged?.Invoke(Rate, Count);
        }

        protected virtual void UpdateView()
        {
            if (!string.IsNullOrEmpty(_countTextKey))
                _countText.SetText(_countTextKey, Count);
            else
                _countText.SetText("{0:0}", Count.ToString());

            

            if (_resultValueText != null)
            {
                var resultValue = _resultValueCalculator == null
                    ? MathA.Ceil(Count * Rate)
                    : _resultValueCalculator(Rate, Count);
                _resultValueText.SetText(_resultTextKey, resultValue);
            }
        }

        private void More()
        {
            //todo rewrite this shame
            if (ChangeStep != 0)
                SetCount(Count + ChangeStep);
            else if (Count < 10)
                SetCount(Count + 1);
            else if (Count < 100)
                SetCount(Count + 10);
            else if (Count < 1000)
                SetCount(Count + 100);
            else if (Count < 10000)
                SetCount(Count + 1000);
            else if (Count < 100000)
                SetCount(Count + 10000);
            else if (Count < 1000000)
                SetCount(Count + 100000);
            else if (Count < 10000000)
                SetCount(Count + 1000000);
            else if (Count < 100000000)
                SetCount(Count + 10000000);
            else
                SetCount(Count + 100000000);
        }

        private void Less()
        {
            //todo rewrite this shame
            if (ChangeStep != 0)
                SetCount(Count - ChangeStep);
            else if (Count <= 10)
                SetCount(Count - 1);
            else if (Count <= 100)
                SetCount(Count - 10);
            else if (Count <= 1000)
                SetCount(Count - 100);
            else if (Count <= 10000)
                SetCount(Count - 1000);
            else if (Count <= 100000)
                SetCount(Count - 10000);
            else if (Count <= 1000000)
                SetCount(Count - 100000);
            else if (Count <= 10000000)
                SetCount(Count - 1000000);
            else if (Count <= 100000000)
                SetCount(Count - 10000000);
            else
                SetCount(Count - 100000000);
        }

        public void SetCount(long count)
        {
            Count = MathA.Clamp(count, _minCount, _maxCount);

            UpdateView();
            OnCountChanged?.Invoke(Rate, Count);
        }
    }
}