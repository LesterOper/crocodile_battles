using System;
using App.Achievements;
using App.Counters;
using App.DAO;
using App.Signals;
using Library;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TakeDailyReward : MonoBehaviour
{
    [SerializeField] private Button take;
    [SerializeField] private Image takeButtonView;
    [SerializeField] private DailyTaskPrizeSetup dailyTaskPrizeSetup;
    [SerializeField] private Sprite active, inActive;

    [Inject] private SignalBus _signalBus;
    [Inject] private IDataService _dataService;
    [Inject] private CounterUtility _counterUtility;

    private TaskAndAchievementsUtility _utility;

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<DailyTaskReadyToClaim>(Setup);
    }

    private void Start()
    {
        _signalBus.Subscribe<DailyTaskReadyToClaim>(Setup);
        _utility = new TaskAndAchievementsUtility(_dataService);
        long completedDailyTasks = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay);
        if (completedDailyTasks < 3)
        {
            take.interactable = false;
        }
        else
        {
            if (_utility.IsDailyPrizeClaimed()) SetRewardTaken();
            else
            {
                Setup();
            }
        }
        take.onClick.AddListener(Claim);
    }

    private void Setup()
    {
        take.interactable = true;
        takeButtonView.sprite = active;
        _utility.SetDailyTaskReadyToClaim(true);
    }

    private void SetRewardTaken()
    {
        take.interactable = false;
        takeButtonView.sprite = inActive;
    }
    
    private void Claim()
    {
        TaskPrizeInformation prizeType =
            dailyTaskPrizeSetup.GetTaskPrizes()[(int) _dataService.GetLong(Constants.DailyTasksCompletedDays)-1];
        ClaimerTaskReward claimerTaskReward = new ClaimerTaskReward(_dataService, _counterUtility);
        claimerTaskReward.Claim(prizeType.GetPrizeType(), prizeType.GetPrizeAmout());
        _utility.SetDailyPrizeClaimed(true);
        SetRewardTaken();
        _utility.SetDailyTaskReadyToClaim(false);
        _signalBus.Fire(new DailyTasksCompleteSignal()
        {
            TaskCompleted = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay),
            CompletedDays = _dataService.GetLong(Constants.DailyTasksCompletedDays)
        });    
    }
}