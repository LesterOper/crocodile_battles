using System;
using System.Collections;
using System.Collections.Generic;
using App;
using App.Achievements;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DailyTaskProgress : MonoBehaviour
{
    [SerializeField] private List<DailyTaskCheckPoint> dailyTaskCheckpoints;
    [SerializeField] private DailyTaskPrizeSetup dailyTaskPrizeSetup;
    //[SerializeField] private Image dailyTasksProgressBar;
    private IDataService _dataService;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus, IDataService dataService)
    {
        _signalBus = signalBus;
        _dataService = dataService;
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<DailyTasksCompleteSignal>(UpdateDailyTaskProgress);
    }

    private void Start()
    {
        _signalBus.Subscribe<DailyTasksCompleteSignal>(UpdateDailyTaskProgress);
        _signalBus.Fire(new DailyTasksCompleteSignal()
        {
            TaskCompleted = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay),
            CompletedDays = _dataService.GetLong(Constants.DailyTasksCompletedDays)
        });
    }


    private void UpdateDailyTaskProgress(DailyTasksCompleteSignal signal)
    {
        long tasksCompleted = signal.TaskCompleted;
        if (tasksCompleted == 3)
            tasksCompleted = 0;
        long dailyTasksCompletedDays = signal.CompletedDays;

        for (int i = 0; i < dailyTasksCompletedDays; i++)
        {
            int completedTasks;
            if (i == dailyTasksCompletedDays - 1)
                completedTasks = (int) tasksCompleted;
            else
                completedTasks = 3;
            dailyTaskCheckpoints[i].Setup(true, completedTasks, _dataService, dailyTaskPrizeSetup.GetTaskPrizes()[i], dailyTasksCompletedDays);
        }
        /*if(dailyTasksCompletedDays == 5)
            _dataService.SetLong(Constants.DailyTasksCompletedDays, 0);*/
    }
}

[System.Serializable]
public class DailyTaskPrize
{
    public PrizeType PrizeType;
    public int Amount;
}
