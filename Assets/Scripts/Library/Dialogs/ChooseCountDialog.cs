using App.SetupsScript;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.Dialogs
{
    [RequireComponent(typeof(ChooseCountView))]
    public class ChooseCountDialog : Dialog
    {
        public delegate void ApplyHandler(float rate, long count);

        [SerializeField] private Button _acceptButton;

        private bool _active;
        [SerializeField] private Button _cancelButton;

        private ApplyHandler _onApply;

        protected ChooseCountView CountView { get; private set; }

        private void Awake()
        {
            CountView = GetComponent<ChooseCountView>();
            CountView.OnCountChanged += OnCountChanged;
        }

        protected override void Start()
        {
            base.Start();

            _acceptButton.onClick.AddListener(() =>
            {
                if (_active)
                {
                    _active = false;
                    _onApply(CountView.Rate, CountView.Count);
                    Hide();
                }
            });

            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(Hide);
        }

        protected virtual void OnCountChanged(float rate, long count)
        {
        }

        public ChooseCountDialog Setup(float rate, long minCount, long maxCount,
            ChooseCountView.ResultValueCalculator calcResultValue = null)
        {
            CountView.Setup(rate, minCount, maxCount, calcResultValue);

            return this;
        }

        public ChooseCountDialog SetApplyHandler(ApplyHandler onApply)
        {
            _onApply = onApply;
            return this;
        }

        public override void Show(RectTransform parentTransform)
        {
            base.Show(parentTransform);
            _active = true;
        }

        public override void Hide()
        {
            _active = false;
            base.Hide();
        }
    }
}