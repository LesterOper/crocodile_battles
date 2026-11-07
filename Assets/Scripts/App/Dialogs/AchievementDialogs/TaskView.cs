using System;
using System.Collections.Generic;
using App;
using App.Achievements;
using App.Analytic;
using App.Audio;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Dialogs.Merge;
using App.Signals;
using CodeBase.Infrastructure.Services;
using I2.Loc;
using Library;
using Library.Common;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TaskView : MonoBehaviour
{
    [SerializeField] private TaskSetup taskSetup;
    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private Image taskIcon;
    [SerializeField] private Image prizeIcon;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button completeTaskButton;
    [SerializeField] private Image taskComleted;
    [SerializeField] private DailyTaskPrizeSetup dailyTaskPrizeSetup;
    [SerializeField] private TextMeshProUGUI prizeAmount;
    [Inject] private IDataService _dataService;
    private Transform _contentPos;
    private Transform _taskParent;
    private TaskInfo _taskInfo;
    [Inject] private SignalBus _signalBus;
    [Inject] private CounterUtility _counterUtility;
    [Inject] private IAnalytics _analytics;
    [Inject] private SoundPlayer _soundPlayer;
    public GameObject GetObject() => gameObject;
    private void Start()
    {
        _contentPos = gameObject.GetComponentInParent<RecycleListView>(true)?.transform;
        _taskParent = transform.parent;
    }

    private void OnDisable()
    {
        completeTaskButton.onClick.RemoveAllListeners();
        completeTaskButton.gameObject.SetActive(false);
    }

    public void Setup(TaskType taskType)
    {
        taskComleted.gameObject.SetActive(false);
        var taskSetupInfo = taskSetup.GetTaskByType(taskType); 
        _taskInfo = _dataService.GetTasks().TaskCollection.Find(task => task.type == taskType);
        taskIcon.sprite = taskSetupInfo.GetTaskIcon;
        prizeIcon.sprite = taskSetupInfo.GetPrizeInfo[0].GetPrizeIcon;
        prizeAmount.text = _taskInfo.prizeAmount.ToString();
        progressBar.fillAmount = (float)_taskInfo.progress / _taskInfo.completeValue;
        title.text = LocalizationManager.GetTranslation(taskSetupInfo.GetDescriptionKey);
        progressText.text = $"{_taskInfo.progress}/{_taskInfo.completeValue}";
        if (_taskInfo.isCompleted)
        {
            completeTaskButton.gameObject.SetActive(true);
            completeTaskButton.onClick.AddListener(ClaimPrize);
        }
        
        if (_taskInfo.taskTimeSpan == TaskTimeSpan.DAY && _taskInfo.prizeTaken)
        {
            taskComleted.gameObject.SetActive(true);
            completeTaskButton.gameObject.SetActive(false);
            completeTaskButton.onClick.RemoveAllListeners();
            return;
        }
    }

    private void ClaimPrize()
    {
        _soundPlayer.PlaySound("ui_upgrd_01", AudioMixerController.MasterMixerGroup.SFX, 0.5f);
        if(_taskInfo.taskTimeSpan == TaskTimeSpan.ETERNAL)
            _analytics.SendEvent(AmplitudeEvent.achievement_collect);
        else
            _analytics.SendEvent(AmplitudeEvent.daily_collect);
        UpdateDailyTasks();
        ClaimerTaskReward claimerTaskReward = new ClaimerTaskReward(_dataService, _counterUtility);
        claimerTaskReward.Claim(_taskInfo.prizeType, _taskInfo.prizeAmount);
        if(_taskInfo.prizeType == PrizeType.MONEY)
            completeTaskButton.gameObject.SetActive(false);

        if (_taskInfo.taskTimeSpan != TaskTimeSpan.DAY)
            _taskInfo.ClaimPrize();
        else
            _taskInfo.SetDailyTaskCompleted();
        _dataService.UpdateTaskCollectionData();
        _signalBus.Fire<UpdateItemsInListSignal>();
    }

    private void UpdateDailyTasks()
    {
        long completedDailyTasks = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay);
        if (_taskInfo.taskTimeSpan == TaskTimeSpan.DAY)
        {
            _taskInfo.prizeTaken = true;
            _dataService.SetLong(Constants.DailyTasksCompletedByCurrentDay, ++completedDailyTasks);
            if (completedDailyTasks == 3)
            {
                _analytics.SendEvent(AmplitudeEvent.daily_all_collect, new Dictionary<string, object>(){{"day count", _dataService.GetLong(Constants.DailyTasksCompletedDays)+1}});
                TaskPrizeInformation prizeType =
                    dailyTaskPrizeSetup.GetTaskPrizes()[(int) _dataService.GetLong(Constants.DailyTasksCompletedDays)];
                _dataService.SetLong(Constants.DailyTasksCompletedDays,
                    _dataService.GetLong(Constants.DailyTasksCompletedDays) + 1);
                if (prizeType.GetPrizeType() == PrizeType.MONEY)
                    completeTaskButton.gameObject.SetActive(false);
                
                _signalBus.Fire<DailyTaskReadyToClaim>();
            }
            _signalBus.Fire(new DailyTasksCompleteSignal()
            {
                TaskCompleted = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay),
                CompletedDays = _dataService.GetLong(Constants.DailyTasksCompletedDays)
            });
        }
    }
}

public class ClaimerTaskReward
{
    private IDataService _dataService;
    private CounterUtility _counterUtility;

    public ClaimerTaskReward(IDataService dataService, CounterUtility counterUtility)
    {
        _dataService = dataService;
        _counterUtility = counterUtility;
    }

    public void Claim(PrizeType prizeType, long amount)
    {
        switch (prizeType)
        {
            case PrizeType.MONEY : 
                ClaimMoney(amount);
                break;
            case PrizeType.SMALL_CHEST : 
                ClaimChest(ChestType.MINI, (int)amount);
            break;
            case PrizeType.MEDIUM_CHEST :
                ClaimChest(ChestType.MEDIUM, (int) amount);
                break;
            case PrizeType.MEGA_CHEST :
                ClaimChest(ChestType.MEGA, (int) amount);
                break;
            case PrizeType.GIGA_CHEST :
                ClaimChest(ChestType.GIGA, (int) amount);
                break;
            case PrizeType.FREEZE_BOOSTER : 
                ClaimBooster(BoosterType.FREEZE, (int)amount);
                break;
            case PrizeType.DOUBLE_DAMAGE_BOOSTER : 
                ClaimBooster(BoosterType.DOUBLE_DAMAGE, (int)amount);
                break;
            case PrizeType.BLOCK_FASTENER_BOOSTER : 
                ClaimBooster(BoosterType.BLOCK_FASTENER, (int)amount);
                break;
        }
    }

    private void ClaimMoney(long amount)
    {
        _counterUtility.AppendMoney(CounterType.DEFAULT_MONEY, amount);
    }

    private void ClaimChest(ChestType chestType, int amount)
    {
        var chestInfo = _dataService.GetChestCollectionData().ChestCollection.Find(chest => chest.type == chestType);
        chestInfo.chestAmount += amount;
        _dataService.UpdateChestCollectionData();
    }

    private void ClaimBooster(BoosterType boosterType, int amount)
    {
        var boosterInfo = _dataService.GetBoosterCollectionData().BoosterCollection
            .Find(booster => booster.BoosterType == boosterType);
        boosterInfo.BoosterAmount += amount;
        _dataService.UpdateBoosterCollectionData();
    }
}

public struct DailyTaskReadyToClaim{}
