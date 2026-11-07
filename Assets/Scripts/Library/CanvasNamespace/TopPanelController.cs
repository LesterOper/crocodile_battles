using System;
using System.Linq;
using App.Analytic;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Library.CanvasNamespace
{
    public class TopPanelController : AbstractPanelController
    {
        [SerializeField] private Button Settings;
        [SerializeField] private Button AchievementButton;
        [SerializeField] private Button PremShopButton;
        [SerializeField] private GameObject taskNotify;
        
        private DialogsContainer _dialogHelper;
        private IAnalytics _analytics;
        private SignalBus _signalBus;
        private IDataService _dataService;
    
        [Inject]
        public void Construct(DialogsContainer  dialogHelper, IAnalytics analytics, SignalBus signalBus, IDataService dataService)
        {
            _dialogHelper = dialogHelper;
            _analytics = analytics;
            _signalBus = signalBus;
            _dataService = dataService;
        }
        private void Awake()
        {
            SetupButton(Settings,  ShowSettings);
            SetupButton(AchievementButton, ShowAchievements);
            SetupButton(PremShopButton, ShowPremShop);
            
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<AchieveSignal>(CheckNotify);
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<AchieveSignal>(CheckNotify);
        }

        private void Start()
        {
            CheckNotify();
        }

        private void CheckNotify()
        {
            if(_dataService.GetTasks().TaskCollection?.FirstOrDefault(task => task.isCompleted ) != null)
                taskNotify.SetActive(true);
            else
                taskNotify.SetActive(false);
        }

        private void ShowPremShop()
        {
            _dialogHelper.ShowPremShopDialog();
        }
        private void ShowAchievements()
        {
            _dialogHelper.ShowAchievementsDialog();
        }
        private  void ShowSettings()
        {
            _analytics
                .SendEvent(AmplitudeEvent.ui_settings_show);
            _dialogHelper.ShowSettingsDialog();
        }

        
    }
}