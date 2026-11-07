using System;
using App.Analytic;
using App.Chests;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using App.Tutorial;
using App.Utils;
using DG.Tweening;
using I2.Loc;
using Library.Common;
using Library.Dialogs;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs
{
    public class ChestsDialog : Dialog
    {
        [Header("Common")]
        private ChestSetup _chestSetup;
        [SerializeField] private RectTransform _chestObjectParent;
        [SerializeField] private TextMeshProUGUI _chestName;
        [SerializeField] private TextMeshProUGUI _trophyCount;
        [SerializeField] private TextMeshProUGUI _chestCost;
        [SerializeField] private CountdownTimer _timer;
        [SerializeField] private TextMeshProUGUI _openTimer;
        [Header("To open view")] 
        [SerializeField] private GameObject openView;
        [SerializeField] private Button wait;
        [SerializeField] private Button forPremMoney;
        [SerializeField] private Button open;
        private ChestHandler _chestHandler;
        private CounterUtility _utility;
        //private IDataService _dataService;
        private IAnalytics _analytics;
        private bool canOpen;
        private bool isTutorial;

        [Inject]
        private void Construct(ChestSetup chestSetup, CounterUtility counterUtility, IAnalytics analytics)
        {
            _chestSetup = chestSetup;
            _utility = counterUtility;
            _analytics = analytics;
        }
        
        protected override void Start()
        {
            base.Start();
            
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //gameObject.SetActive(false);
            SetupButton(wait, () => FindObjectOfType<AdsHelper>().PlayVideo(WaitUntilOpen, AmplitudePlacementProperty.chest_timer));
            SetupButton(forPremMoney, OpenByPremMoney);
            SetupButton(open, Open);
            if (_utility.GetMoney(CounterType.PREMIUM_MONEY) - 5 < 0)
            {
                forPremMoney.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                forPremMoney.interactable = false;
            }
            
            _hideDialogButtonArea.onClick.RemoveAllListeners();
            _hideDialogButtonArea.onClick.AddListener(AreaClick);
            if(_chestHandler.ChestType == ChestType.DAILY)
                _closeButton.gameObject.SetActive(false);
        }
        
        
        public void Setup(bool canOpen, ChestHandler chestHandler, bool isTutorial = false)
        {
            this.isTutorial = isTutorial;
            _chestName.text = LocalizationManager.GetTranslation(_chestSetup.GetChestCharacteristic(chestHandler.ChestType).chestName);
            _trophyCount.text =  SetTrophiesCountText(_chestSetup.GetChestCharacteristic(chestHandler.ChestType).trophyCount);
       
            _chestCost.text = "-" + _chestSetup.GetChestCharacteristic(chestHandler.ChestType).trophyCost.ToString();
            Instantiate(_chestSetup.GetChestObject(chestHandler.ChestType), _chestObjectParent);
            this.canOpen = canOpen;
            if (canOpen)
            {
                open.gameObject.SetActive(true);
                wait.gameObject.SetActive(false);
                forPremMoney.gameObject.SetActive(false);
            }

            _chestHandler = chestHandler;
            if (chestHandler.ChestType != ChestType.DAILY)
            {
                if (this.isTutorial)
                {
                    _timer.gameObject.SetActive(false);
                    _openTimer.gameObject.SetActive(true);
                    _closeButton.gameObject.SetActive(false);
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_chest_open_start);
                }
                else
                    StartTimer();
            }
            else
                _timer.gameObject.SetActive(false);
            gameObject.SetActive(true);
            gameObject.transform.DOPunchScale(Vector3.one*0.1f, 0.2f, 1, 0);
        }

        private void AreaClick()
        {
            if(canOpen) OpenChest();
            Hide();
        }

        private string SetTrophiesCountText(int counts)
        {
            string translate = counts > 4
                ? LocalizationManager.GetTranslation("TROPHIES_MORE__THAN_4")
                : LocalizationManager.GetTranslation("TROPHIES");
            return counts + " " + translate;
        }

        private void SetupButton(Button target, Action action)
        {
            if(target != null) target.onClick.AddListener(action.Invoke);   
        }

        private void StartTimer()
        {
            long time;
            string ticksString = _dataService.GetString(AppConstants.TIMER + _chestHandler.ChestType + _chestHandler.ChestSlot);
            if (Int64.TryParse(ticksString, out time))
            {
                long restTime = time - DateTime.UtcNow.Ticks;
                if (restTime > 0)
                {
                    _timer.StartCountdown(DateTime.UtcNow.Ticks, restTime);
                    _timer.OnCountdownFinish += () => ChangeState(true);
                    return;
                }
                ChangeState(true);
            }
        }
    
        private void ChangeState(bool canOpen)
        {
            _openTimer.gameObject.SetActive(!canOpen);
            _timer.gameObject.SetActive(!canOpen);
            open.gameObject.SetActive(canOpen);
            wait.gameObject.SetActive(!canOpen);
            forPremMoney.gameObject.SetActive(!canOpen);
        }

        private void Open()
        {
            OpenChest();
            if (_chestHandler.ChestType == ChestType.DAILY)
            {
                DailyPrizeUtils.OnPrizeCollected(_dataService);
            }
        }

        private void WaitUntilOpen()
        {
            long time;
            string ticksString = _dataService.GetString(AppConstants.TIMER + _chestHandler.ChestType + _chestHandler.ChestSlot);
            Int64.TryParse(ticksString, out time);
            time = time - TimeSpan.TicksPerMinute * 30;
            _dataService.SetString(AppConstants.TIMER + _chestHandler.ChestType + _chestHandler.ChestSlot, time.ToString());
            _chestHandler.SetupTimer();
            _dataService.SetLastInterTicks();
            Hide();
        }
        
        private void OpenByPremMoney()
        {
            OpenChest();
        }

        private void OpenChest()
        {
            if (_dataService.GetTutorialCurrentStep() != TutorialStepType.OPEN_CHEST)
            {
                _chestHandler.SetOpened();
                if (_chestHandler.ChestType != ChestType.DAILY)
                {
                    _chestHandler.ResetTimer();
                    _dataService.GetChestSlotsData().Slots.Find(slot => slot.ChestSlot == _chestHandler.ChestSlot)
                            .Empty =
                        true;
                    _dataService.UpdateChestSlotsData();
                }
                _signalBus.Fire(new ChestCollectedSignal(){ChestSlot = _chestHandler.ChestSlot});
            }
            openView.SetActive(false);
            TakeCardsDialog takeCardsDialog = FindObjectOfType<DialogsContainer>().ShowTakeCardsDialog();
            takeCardsDialog.Setup(_chestHandler.ChestType, isTutorial);
            Hide();
        }
    }
}