using App.Analytic;
using App.Counters;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassDialog : Dialog
    {
        [SerializeField] private BattlePassRecycleListView _recycleListView;
        [SerializeField] private BattlePassProgressCounter _progressCounter;
        [SerializeField] private Button _activatePassButton;
        [SerializeField] private TextMeshProUGUI _activatedPassLabel;
        [Inject] private IAnalytics _analytics;
        private BattlePassUtility _battlePassUtility;
        private BattlePassPrizeContainer _prizeContainer;
        private DialogsContainer _dialogsContainer;

        [Inject]
        private void Construct(IAnalytics analytics, CounterUtility counterUtility, BattlePassPrizeContainer prizeContainer,
            BattlePassUtility passUtility, DialogsContainer dialogsContainer)
        {
            _analytics = analytics;
            _prizeContainer = prizeContainer;
            _battlePassUtility = passUtility;
            _dialogsContainer = dialogsContainer;
            _progressCounter.Setup(_battlePassUtility, _prizeContainer);
        }
        
        protected override void Start()
        {
            base.Start();
            _recycleListView.Setup();
            _analytics.SendEvent(AmplitudeEvent.ui_battle_pass_show);
            UpdateView();
            _activatePassButton.onClick.AddListener(OpenPremShop);
        }

        private void OpenPremShop()
        {
            _dialogsContainer.ShowPremShopDialog();
            ForceHide();
        }

        private void UpdateView()
        {
            _activatePassButton.gameObject.SetActive(_battlePassUtility.IsBattlePassBought == false);
            _activatedPassLabel.gameObject.SetActive(_battlePassUtility.IsBattlePassBought);
        }
    }


    public struct BattlePassProgressSignal
    {
        public int index;
        public bool active;
    }
    
    public enum BattlePassPrizeType
    {
        NONE = 0,
        СHEST = 1,
        PREM_MONEY = 2,
        DEFAULT_MONEY = 3,
        BATH_CARDS = 4,
        WEAPON_CARDS = 5,
        BOOSTER = 6
    }
}