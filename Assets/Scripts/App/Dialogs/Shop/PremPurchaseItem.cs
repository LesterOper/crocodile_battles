using System;
using App.Counters;
using App.Extensions;
using App.PremPurchases;
using I2.Loc;
using Library.Common.Purchases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.Shop
{
    public class PremPurchaseItem : MonoBehaviour
    {
        [SerializeField] private PremPurchaseType _type;

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _countText;

        [Space]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _giftView;

        protected Purchase _purchase;
        private CounterUtility _counterUtility;
        private PurchaseController _purchaseController;
        private Coroutine _timerCoroutine;
        
        public void Setup(PurchaseController purchaseController)
        {
            _purchaseController = purchaseController;
            _purchase = _purchaseController.GetPurchase(_type);
            _buyButton.onClick.AddListener(OnBuyClicked);
            UpdateView();
        }

        protected virtual void UpdateView()
        {
            if (_nameText != null)
                _nameText.text = LocalizationManager.GetTranslation(_purchase.NameKey);
            if (_descriptionText != null)
            {
                _descriptionText.text = LocalizationManager.GetTranslation(_purchase.Descriptions);
                var param = _descriptionText.GetComponent<LocalizationParamsManager>();
                if(param != null)
                    param.SetParameterValue("VALUE", _purchase.Amount.ToString());
            }
                
            if (_countText != null)
                _countText.text = _purchase.Amount.ToString();
            
            if (_priceText != null)
                _priceText.text = _purchase.GetPriceText();

            if (_iconImage != null)
                _iconImage.sprite = _purchase.SmallIcon;

            if(_giftView != null)
                _giftView.SetActive(_purchase.IsX2Amount);

            Purchase.PurchaseStatus purchaseStatus = _purchase.GetStatus();
            bool isAvailableToBuy = purchaseStatus == Purchase.PurchaseStatus.Available;
            _buyButton.interactable = isAvailableToBuy;
            if (purchaseStatus == Purchase.PurchaseStatus.PurchaseLimitReached)
            {
                _priceText.text = LocalizationManager.GetTranslation("PURCHASED");
                _buyButton.interactable = false;
            }

            if(_timerCoroutine != null) StopCoroutine(_timerCoroutine);

            if (purchaseStatus == Purchase.PurchaseStatus.OnCooldown)
            {
                var remainingCooldownTime = _purchase.GetRemainingCooldownTime();
                _timerCoroutine = StartCoroutine(_priceText.DOCustomCounter(remainingCooldownTime.TotalSeconds + 2, 
                    0, (float) remainingCooldownTime.TotalSeconds + 2,
                    t => TimeSpan.FromSeconds(t).ToString("hh\\:mm\\:ss"), UpdateView));
            }
        }
        
        private void OnBuyClicked() => _purchase.Buy(UpdateView);
    }
}