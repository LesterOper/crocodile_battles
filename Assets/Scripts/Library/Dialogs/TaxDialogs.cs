using App.SetupsScript;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.Dialogs
{
    public class TaxDialogs : Dialog
    {
        public delegate void ApplyHandler(float rate, long count);

        [SerializeField] private Button _acceptButton;

        private bool _active;
        [SerializeField] private Button _cancelButton;

        private ApplyHandler _onApply;


        protected override void Start()
        {
            base.Start();
            _acceptButton.onClick.AddListener(() =>
            {
                if (_active)
                {
                    _active = false;
                    Hide();
                }
            });

            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(Hide);
        }

        protected virtual void OnCountChanged(float rate, long count)
        {
        }

        public TaxDialogs Setup(float rate, long minCount, long maxCount,
            ChooseCountView.ResultValueCalculator calcResultValue = null)
        {
            return this;
        }

        public TaxDialogs SetApplyHandler(ApplyHandler onApply)
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