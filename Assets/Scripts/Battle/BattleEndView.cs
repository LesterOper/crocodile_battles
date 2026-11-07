using System;
using System.Collections.Generic;
using App.Achievements;
using App.Analytic;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Dialogs;
using App.Dialogs.BattlePass;
using App.Dialogs.EndGameDialog;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using BattleStates.States;
using DG.Tweening;
using I2.Loc;
using Library.Common;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Battle
{
    public class BattleEndView : MonoBehaviour
    {
        [Header("DialogVariants")] 
        [SerializeField] private WinFirstChestView _firstChestWinDialog;
        [SerializeField] private WinFirstChestView tutorVariant;
        [SerializeField] private WinView _winDialog;
        [SerializeField] private ChestNotificationView takeChestNotification;
        [Header("Common")] 
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _descriptionTextTutor;
        [SerializeField] private Button _closeButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject panel;
        [SerializeField] private BattlePassPrizeContainer _prizeContainer;
        [Header("X2Reward")] 
        [SerializeField] private Image x2Icon;
        //[SerializeField] private Sprite activeX2;
        //[SerializeField] private Sprite deActiveX2;
        [SerializeField] private TextMeshProUGUI pointsX2Bonus;
        [SerializeField] private TextMeshProUGUI moneyX2Bonus;
        [Inject] private ChestSetup _chestSetup;
        
        private AdsHelper ads;
        private CounterUtility _utility;
        private IDataService _dataService;
        private IGameInfoService _gameInfoService;
        private IAnalytics _analytics;
        private DialogsContainer container;
        private SignalBus _signalBus;
        private BattlePassUtility _battlePassUtility;
        private float points;
        private float money;

        [Inject]
        public void Construct(SignalBus signalBus, IGameInfoService gameInfoService, CounterUtility utility, 
            IDataService dataService, IAnalytics analytics, BattlePassUtility battlePassUtility, DialogsContainer container)
        {
            _signalBus = signalBus;
            _gameInfoService = gameInfoService;
            _utility = utility;
            _dataService = dataService;
            _analytics = analytics;
            _battlePassUtility = battlePassUtility;
            this.container = container;
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(DisablePanel);
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ShowEndBattleViewSignal>(Setup);   
            _signalBus.TryUnsubscribe<UpdateEndGameDialogSignal>(UpdateDialog);
            _signalBus.TryUnsubscribe<ResetChestProgressSignal>(ResetChestProgress);
            _signalBus.TryUnsubscribe<ChestNotificationSignal>(ShowTakeChestNotification);
            _closeButton.onClick.RemoveListener(DisablePanel);
        }

        private void DisablePanel()
        {
            panel.gameObject.SetActive(false);
        }

        private void ShowTakeChestNotification(ChestNotificationSignal signal)
        {
            Image image = signal.NextChestIcon;
            takeChestNotification.SetChestImage(image.sprite);
            takeChestNotification.gameObject.SetActive(true);
            _signalBus.Fire(new ChestTakenSignal());
        }

        private void Awake()
        {
            ads = FindObjectOfType<AdsHelper>();
        }

        private void Start()
        {
            _signalBus.Subscribe<ShowEndBattleViewSignal>(Setup);
            _signalBus.Subscribe<UpdateEndGameDialogSignal>(UpdateDialog);
            _signalBus.Subscribe<ResetChestProgressSignal>(ResetChestProgress);
            _signalBus.Subscribe<ChestNotificationSignal>(ShowTakeChestNotification);
            SetupX2RewardState(_dataService.GetX2RewardState());
        }

        private void SetupX2RewardState(bool active)
        {
            x2Icon.gameObject.SetActive(active); //sprite = active ? activeX2 : deActiveX2;
            pointsX2Bonus.gameObject.SetActive(active);
            moneyX2Bonus.gameObject.SetActive(active);
        }

        private void Setup(ShowEndBattleViewSignal signal)
        {
            bool isPlayerWin = signal.IsPlayerWin;
            bool isEnemyWin = signal.IsEnemyWin;
            
            if (_gameInfoService.IsBossFight())
            {
                SetupBossBattleEndView(isPlayerWin, isEnemyWin);
            }
            else
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_show);
                SetupRandomBattleEndView(isPlayerWin, isEnemyWin);
            }
            if(_utility.GetMoney(CounterType.BATTLES_WIN) >= 5 )
                Interstitial();
            _signalBus.Fire<ClearSubscribersAfterFightSignal>();
        }

        private void Interstitial()
        {
            long ticks = _dataService.GetLastInterTicks();
            if (ticks <= 0)
            {
                _dataService.SetLastInterTicks();
                return;
            }
            ticks += TimeSpan.TicksPerMinute * 5;
            long newTicks = DateTime.UtcNow.Ticks;
            if (ticks < newTicks)
            {
                _dataService.SetLastInterTicks();
                ads.ShowInterstitial();
            }
        }

        private void SetupBossBattleEndView(bool isPlayerWin, bool isEnemyWin)
        {
            BossBattleEndDialog end = container.ShowBossBattleEndDialog();
            end.Setup(isPlayerWin);
        }
    
        private void SetupRandomBattleEndView(bool isPlayerWin, bool isEnemyWin)
        {
            if (_utility.GetMoney(CounterType.BATTLES_WIN) <= 6
                && _dataService.GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
            {
                if (_utility.GetMoney(CounterType.BATTLES_WIN) == 6 && isEnemyWin)
                {
                    SetupBattleEndView(isPlayerWin, isEnemyWin);
                }
                else
                {
                    SetupBattleEndViewForBeginners(isPlayerWin, isEnemyWin);
                }
            }
            else
            {
                SetupBattleEndView(isPlayerWin, isEnemyWin);
            }
        }
    
        private void SetupBattleEndView(bool isPlayerWin, bool isEnemyWin)
        {
            points = (isPlayerWin ? Random.Range(300, 400) : Random.Range(5, 15));
            BattleViewController.roundIndex = 1;
        
            ChestType nextChest = _utility.GetNextChest();
        
            int nextChestPoints =
                _utility.GetMoney(CounterType.BATTLES_WIN) > 3
                    ? _chestSetup.GetChestCharacteristic(nextChest).pointsToGet
                    : 300;
            panel.SetActive(true);
            _canvasGroup.DOFade(1, 0).SetDelay(1f).OnComplete(()=>
                SetupDialogInformation(points, nextChestPoints, nextChest));
            if (isPlayerWin && !isEnemyWin)
            {
                if (_prizeContainer.GetPrizeItemList()[Mathf.Clamp((int) _battlePassUtility.GetBattlePassCurrentStep() - 1, 0, 100)].BossBathType != _dataService.GetCurrentBoss()) 
                    _battlePassUtility.AppendBattlePassStepProgress((int)_battlePassUtility.GetBattlePassCurrentStep());
                _titleText.text = LocalizationManager.GetTranslation("WIN");
                money = 20;
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.win}});
            }
            else if (!isPlayerWin && isEnemyWin)
            {
                _titleText.text = $"<color=#BAC2C2>{LocalizationManager.GetTranslation("LOOSE")}</color>";
                money = 2;
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.loose}});
            }
            else
            {
                _titleText.text = $"<color=#BAC2C2>{LocalizationManager.GetTranslation("DRAW")}</color>";
                money = 5;
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.draw}});
            }

            LocalizationParamsManager manager = _descriptionText.GetComponent<LocalizationParamsManager>();
            manager.SetParameterValue("VALUE", points.ToString());

        }

        private void SetupBattleEndViewForBeginners(bool isPlayerWin, bool isEnemyWin)
        {
            int winCount = isPlayerWin ? (int)_utility.GetMoney(CounterType.BATTLES_WIN)-1 : (int)_utility.GetMoney(CounterType.BATTLES_WIN);
            float points = isPlayerWin ? 1 : 0;
            ChestType nextChest;
            BattleViewController.roundIndex = 1;
            int nextChestPoints;

            if (winCount == 1)
                nextChest = ChestType.MINI;
            else if (winCount == 2 || winCount == 3)
                nextChest = ChestType.MEDIUM;
            else
                nextChest = ChestType.MINI;

            if (winCount == 1 || winCount == 0)
                nextChestPoints = 1;
            else
                nextChestPoints = 2;
            
            panel.SetActive(true);
            _canvasGroup.DOFade(1, 0).SetDelay(1f).OnComplete(()=>
                SetupDialogInformationForBeginner(points, nextChestPoints, nextChest));
        
            if (isPlayerWin && !isEnemyWin)
            {
                if(_prizeContainer.GetPrizeItemList()[Mathf.Clamp((int) _battlePassUtility.GetBattlePassCurrentStep() - 1, 0, 100)].BossBathType != _dataService.GetCurrentBoss())
                    _battlePassUtility.AppendBattlePassStepProgress((int)_battlePassUtility.GetBattlePassCurrentStep());
                _titleText.text = LocalizationManager.GetTranslation("WIN");
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.win}});
            }
            else if (!isPlayerWin && isEnemyWin)
            {
                _titleText.text = $"<color=#BAC2C2>{LocalizationManager.GetTranslation("LOOSE")}</color>";
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.loose}});
            }
            else
            {
                _titleText.text = $"<color=#BAC2C2>{LocalizationManager.GetTranslation("DRAW")}</color>";
                if(_dataService.GetTutorialCurrentStep() != TutorialStepType.BATTLE_TUTORIAL_END)
                    _analytics.SendEvent(AmplitudeEvent.game_end, 
                    new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), AmplitudePlacementProperty.draw}});
            }
        
        
        }

        private void SetupDialogInformationForBeginner(float points, int nextChestPoints, ChestType nextChest)
        {
            _firstChestWinDialog.gameObject.SetActive(true);
            _firstChestWinDialog.SetupForBeginners(points, nextChestPoints, 
                _dataService.GetLong("WinCountForBeginner"), _chestSetup.GetChestCharacteristic(nextChest).chestIcon, nextChest);
        }
    
        private void SetupDialogInformation(float points, int nextChestPoints, ChestType nextChest)
        {
            bool x2Active = _dataService.GetX2RewardState();
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.BATTLE_TUTORIAL_END)
            {
                _dataService.SetExtensionUnlock(Extension.WEAPON_COLLECTION_BUTTON, true);
                _descriptionText.gameObject.SetActive(false);
                _dataService.GetBathData().Level = 1;
                _dataService.GetBathData().CrocodileBathType = BathType.BATH_TYPE_10;
                _dataService.GetCardCollectionData().Collection.Clear();
                _dataService.GetBattleCardInfoData().CardInfos.Clear();
                _dataService.UpdateBathData();
                //_dataService.UpdateCardCollectionData();
                //_dataService.UpdateBattleCardInfoData();
                _firstChestWinDialog.gameObject.SetActive(false);
                tutorVariant.gameObject.SetActive(true);
                tutorVariant.Setup(points, nextChestPoints, 0);
            }
            else
            {
                _utility.AppendMoney(CounterType.POINTS, points + (x2Active ? points : 0));
                pointsX2Bonus.text = "+" + points;
                _descriptionText.gameObject.SetActive(true);
                _descriptionTextTutor.gameObject.SetActive(false);
                if (nextChest == ChestType.MINI)
                {
                    _firstChestWinDialog.gameObject.SetActive(true);
                    _firstChestWinDialog.Setup(points + (x2Active? points : 0), nextChestPoints, _utility.GetMoney(CounterType.POINTS), money);
                }
                else
                {
                    ChestType prevChest = _utility.GetPrevChest();
                    _firstChestWinDialog.gameObject.SetActive(false);
                    _winDialog.gameObject.SetActive(true);
                    _winDialog.Setup(points + (x2Active? points : 0), nextChestPoints, _utility.GetMoney(CounterType.POINTS),
                        _chestSetup.GetChestCharacteristic(prevChest).chestIcon,
                        _chestSetup.GetChestCharacteristic(nextChest).chestIcon, money);
                }
            }
        }
        
        private void UpdateDialog()
        {
            _utility.SetNextChest();
            ChestType nextChest = _utility.GetNextChest();
            ChestType prevChest = _utility.GetPrevChest();
            int nextChestPoints = _chestSetup.GetChestCharacteristic(nextChest).pointsToGet;
            _firstChestWinDialog.gameObject.SetActive(false);
            _winDialog.gameObject.SetActive(true);
            _winDialog.Setup(_utility.GetMoney(CounterType.POINTS), 
                nextChestPoints, _utility.GetMoney(CounterType.POINTS),
                _chestSetup.GetChestCharacteristic(prevChest).chestIcon,
                _chestSetup.GetChestCharacteristic(nextChest).chestIcon, money);
        }

        private void ResetChestProgress()
        {
            _titleText.text = LocalizationManager.GetTranslation("CHEST_DELIVERED");
            float points = 0;
            float nextChestPoints = _chestSetup.GetChestCharacteristic(ChestType.MINI).pointsToGet;
            _firstChestWinDialog.gameObject.SetActive(true);
            _firstChestWinDialog.Setup(points, nextChestPoints, _utility.GetMoney(CounterType.POINTS), money);
            _winDialog.gameObject.SetActive(false);
        }
    }
}