using App.Analytic;
using Library.Common.Purchases;
using Library.Dialogs;
using UnityEngine;
using Zenject;

namespace App.Dialogs.Shop
{
    public class PremPurchaseDialog : Dialog
    {
        private PremPurchaseItem[] _views;
        private IAnalytics _analytics;
        private PurchaseController _purchaseController;
        
        [Inject]
        private void Construct(IAnalytics analytics, PurchaseController purchaseController)
        {
            _analytics = analytics;
            _purchaseController = purchaseController;
        }
        
        protected override void Start()
        {
            base.Start();
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = Vector2.zero;
            Setup();
            _analytics.SendEvent(AmplitudeEvent.ui_shop_show);
        }

        private void Setup()
        {
            _views = GetComponentsInChildren<PremPurchaseItem>();
            foreach (var view in _views)
                view.Setup(_purchaseController);
        }
    }
}