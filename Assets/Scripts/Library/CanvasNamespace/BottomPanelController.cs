using System;
using App;
using App.Analytic;
using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using I2.Loc;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.CanvasNamespace
{
    public class BottomPanelController : AbstractPanelController, ICoroutineRunner
    {
        private const string BattleScene = "BattleScene";
    
        [SerializeField] private Button BossFight;
        [SerializeField] private Button RandomFight;
        [SerializeField] private Transform toast;
        [SerializeField] private TextMeshProUGUI toastText;
        private DialogsContainer _dialogHelper;
        private SignalBus _signalBus;
        private GameStateMachine _gameStateMachine;
        private IGameInfoService _gameInfoService;
        private IDataService _dataService;
        private IAnalytics _analytics;
        [Inject]
        public void Construct(DialogsContainer  dialogHelper, SignalBus signalBus, 
            GameStateMachine gameStateMachine, IGameInfoService gameInfoService, 
            IDataService dataService, IAnalytics analytics)
        {
            _dialogHelper = dialogHelper;
            _signalBus = signalBus;
            _gameStateMachine = gameStateMachine;
            _gameInfoService = gameInfoService;
            _dataService = dataService;
            _analytics = analytics;
        }
        private void Awake()
        {
            
            /*SetupButton(RandomFight, () =>
            {
                bool isGarage = AllServices.Container.Single<IGameInfoService>().IsGarage();
                AllServices.Container.Single<IAnalytics>().SendStartBattleEvent(AmplitudeEvent.game_start,AmplitudePlacementProperty.main, _service.GetBathData());
                AllServices.Container.Single<IGameInfoService>().SetIsBossFight(false);
                AllServices.Container.Single<IGameInfoService>().SetDoubleCardsInRound(false);
                if (_service.GetTutorialCurrentStep() == TutorialStepType.RANDOM_BATTLE_POINTER_HELP)
                {
                    _signalBus.Fire<CompleteTutorialSignal>();
                }
                LoadBattleScene();
            });
            SetupButton(BossFight, () =>
            {
                _dialogHelper.ShowBattlePassDialog();*/
                /*bool isGarage = AllServices.Container.Single<IGameInfoService>().IsGarage();
                AllServices.Container.Single<IAnalytics>().SendStartBossBattleEvent(AmplitudeEvent.game_boss_start, _service.GetBathData());


                if (_service.GetTutorialCurrentStep() == TutorialStepType.BOSS_POINTER)
                {
                    AllServices.Container.Single<IAnalytics>().SendEvent(AmplitudeTutorialEvent.tutor_boss_button_complete);
                    AllServices.Container.Single<IAnalytics>().SendEvent(AmplitudeTutorialEvent.tutor_boss_complete);
                }
                    
                if ((int)_service.GetCurrentBoss() <= 16)
                {
                    AllServices.Container.Single<IGameInfoService>().SetIsBossFight(true);
                    LoadBattleScene();
                }*/
            //});
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<LoadBattleSceneSignal>(LoadBattleScene);
        }

        private void Start()
        {
            _signalBus.Subscribe<LoadBattleSceneSignal>(LoadBattleScene);
            SetupButton(RandomFight, () =>
            {
                var boosterDialog = _dialogHelper.ShowBoostersDialog();
                boosterDialog.SetFightButton(() =>
                {
                    bool isGarage = _gameInfoService.IsGarage();
                    _analytics.SendStartBattleEvent(AmplitudeEvent.game_start,AmplitudePlacementProperty.main, _dataService.GetBathData());
                    _gameInfoService.SetIsBossFight(false);
                    _gameInfoService.SetDoubleCardsInRound(false);
                    if (_dataService.GetTutorialCurrentStep() == TutorialStepType.RANDOM_BATTLE_POINTER_HELP)
                    {
                        _signalBus.Fire<CompleteTutorialSignal>();
                    }
                    LoadBattleScene();
                });
            });
            SetupButton(BossFight, () => { _dialogHelper.ShowBattlePassDialog(); });
        }

        private void LoadBattleScene()
        {
            if (ToastShow())
            {
                toast.DOKill();
                toastText.DOKill();
                toast.gameObject.SetActive(true);
                toast.DOScale(1.2f, 2f);
                toastText.DOFade(0, 1.2f).SetDelay(3f).OnComplete(() =>
                {
                    toast.gameObject.SetActive(false);
                    toast.localScale = Vector3.zero;
                    toastText.DOFade(1, 0);
                });
                return;
            }
            _gameStateMachine.Enter<LoadLevelState, string>(BattleScene);
        }

        private bool ToastShow()
        {
            var cards = _dataService.GetBattleCardInfoData().CardInfos;

            if (cards.Count < 5)
            {
                toastText.text = LocalizationManager.GetTranslation("TOAST_WITHOUT_WEAPONS");
                return true;
            }

            return false;
        }
    }
}