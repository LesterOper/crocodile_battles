using App.Analytic;
using App.Dialogs.EndGameDialog;
using App.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassPrizeView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image closeIcon;
        [SerializeField] private Image readyToClaimIcon;
        [SerializeField] private Image claimedIcon;
        [SerializeField] private Image claimedBackground;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Characteristic amount;
        [SerializeField] private Image barFilling;
        [SerializeField] private CanvasGroup _canvasGroup;
        [Inject] private BattlePassUtility _battlePassUtility;
        [Inject] private SignalBus _signalBus;
        [Inject] private IGameInfoService _infoService;
        [Inject] private IAnalytics _analytics;
        private PrizeItem _prizeItem;
        private bool _isOpen;
        private int _prizeIndex;
        private bool _prem;
        private BattlePassUtility _passUtility;

        public void Setup(PrizeItem prizeItem, int prizeIndex, bool prem, BattlePassUtility passUtility)
        {
            _prem = prem;
            _prizeItem = prizeItem;
            _prizeIndex = prizeIndex;
            _passUtility = passUtility;
            UpdateView();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_canvasGroup.interactable == false) return;
            bool isClaimed =
                _prem ? _passUtility.IsPremPrizeClaimed(_prizeIndex) : _passUtility.IsPrizeClaimed(_prizeIndex);
            bool isCompleted = _passUtility.GetBattlePassStepComplete(_prizeIndex);
            if (isClaimed || isCompleted == false) return;
            if(_passUtility.IsBattlePassBought == false && _prem) return;
            _battlePassUtility.ApplyPrize(_prizeItem, _prizeIndex, _prem, _infoService, _signalBus, _analytics);
            UpdateView();
            _signalBus.Fire<BattlePassNotificationSignal>();
        }

        public void SetVisibility(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1 : 0;
            _canvasGroup.interactable = isVisible;
        }
        
        private void UpdateView()
        {
            icon.sprite = _prizeItem.PrizeIconProperty;
            title.text = _prizeItem.PrizeTypeProperty.ToString();
            amount.SetupCharacteristic(_prizeItem.GetAmount().ToString());
            bool isClaimed =
                _prem ? _passUtility.IsPremPrizeClaimed(_prizeIndex) : _passUtility.IsPrizeClaimed(_prizeIndex);
            bool isCompleted = _passUtility.GetBattlePassStepComplete(_prizeIndex);
            bool isPassBought = !_prem || _passUtility.IsBattlePassBought;
            
            closeIcon.gameObject.SetActive(isPassBought == false && _prem);
            readyToClaimIcon.gameObject.SetActive(isCompleted && isClaimed == false && isPassBought);
            claimedIcon.gameObject.SetActive(isClaimed);
            claimedBackground.gameObject.SetActive(isClaimed);
            barFilling.gameObject.SetActive(isCompleted && isPassBought);
            icon.DOFade(isClaimed ? 0.5f : 1f, 0f);
            amount.SetColor(isClaimed == false);
        }
    }
}