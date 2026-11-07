using Alexplay.GDPR.Logic.Service;
using App.Achievements;
using App.Analytic;
using App.Cards;
using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Tutorial;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using Firebase.Analytics;
using Library.Common;
using Library.Common.Purchases;
using Library.Data;
using Library.L10n;
using Library.Utils;
using MRGS.Base;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly DataHelper _dataHelper;
        private AdsHelper _adsHelper;
        private IDataService _dataService;
        private IMRGSService _mrgsService; 
        private SignalBus _signalBus;
        private TaskSetup _taskSetup;
        private IAchievementObserver _achievementObserver;
        private IAnalytics _analytics;
        private PurchaseTransactionController _purchaseTransactionController;

        private RectTransform _gdprParent;

        private TutorialController _tutorialController;
        //private GDPRHandler _gdprHandler;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine, IDataService dataService,
            SignalBus signalBus, IAchievementObserver achievementObserver, TaskSetup taskSetup, IAnalytics analytics, PurchaseTransactionController purchaseTransactionController)
        {
            _taskSetup = taskSetup;
            _stateMachine = gameStateMachine;
            _dataService = dataService;
            _signalBus = signalBus;
            _analytics = analytics;
            _achievementObserver = achievementObserver;
            _purchaseTransactionController = purchaseTransactionController;
        }
        
        public BootstrapState( SceneLoader sceneLoader, DataHelper dataHelper,  AdsHelper adsHelper, RectTransform gdprParent, TutorialController tutorialController)
        {
            _sceneLoader = sceneLoader;
            _dataHelper = dataHelper;
            _adsHelper = adsHelper;
            _gdprParent = gdprParent;
            _tutorialController = tutorialController;
            //_gdprHandler = gdprHandler;
        }

        private void SetTutorialSetOfEquipment()
        {
            _dataService.SetTutorialSetOfEquipment();
        }
        
        public void Enter()
        {
            _dataService.Initialize(_dataHelper, _signalBus);
            if (_dataService.GetLong("GDPR_ACCEPTED") == 1)
            {
#if UNITY_EDITOR
                Init();     
#else
                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    Init();
                });
#endif
                
            }
            else
            {
                GdprService.Instance.Accepted += () =>
                {
#if UNITY_EDITOR
                    Init(); 
                    _dataService.SetLong("GDPR_ACCEPTED", 1);
#else
                    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                    {
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                        Init();
                    });
                    _dataService.SetLong("GDPR_ACCEPTED", 1);
#endif
                    
                };
                GdprService.Instance.ShowGdpr(_gdprParent);
            }

        }

        private void Init()
        {
            _achievementObserver.Setup(_dataService, _signalBus, _taskSetup, _analytics);
            SetTutorialSetOfEquipment();
            _adsHelper.InitAds(true);
            _purchaseTransactionController.InitPurchaseService();
            LoadScene();
            _tutorialController.Setup();
        }

        private void LoadScene()
        {
            string sceneName = default;
            if (_dataService.GetTutorialCurrentStep().GetAttribute<TutorialAttribute>().IsBattle)
            {
                sceneName = "BattleScene";
            }
            else sceneName = "Main";

            _stateMachine.Enter<LoadLevelState, string>(sceneName);
        }

        public void Exit()
        {
        }
        
    }
}