using System;
using App.DAO;
using App.Signals;
using Library;
using Library.View;
using UnityEngine;
using Zenject;

public class DailyTaskUpdater : MonoBehaviour
{
    [SerializeField] private CountdownTimer _countdownTimer;
    [Inject] private IDataService _dataService;
    [Inject] private SignalBus _signalBus;

    private TaskAndAchievementsUtility _utility;
    private void Start()
    {
        _utility = new TaskAndAchievementsUtility(_dataService);
        long ticks = DateTime.Parse(_utility.GetLastTime()).Ticks + TimeSpan.TicksPerDay;
        long left = ticks - DateTime.Now.Ticks;
        if (left > 0)
        {
            _countdownTimer.StartCountdown(DateTime.UtcNow.Ticks, left);
            _countdownTimer.OnCountdownFinish += FinishTimer;
        }
    }

    private void UpdateTimer()
    {
        _countdownTimer.StopCountdown();
        _countdownTimer.OnCountdownFinish -= FinishTimer;
        long ticks = DateTime.Parse(_utility.GetLastTime()).Ticks + TimeSpan.TicksPerDay;
        long left = ticks - DateTime.Now.Ticks;
        _countdownTimer.StartCountdown(DateTime.UtcNow.Ticks, left);
        _countdownTimer.OnCountdownFinish += FinishTimer;
        _signalBus.Fire<UpdateDailyTaskListSignal>();
    }

    private void FinishTimer()
    {
        //gameObject.SetActive(false);
        _signalBus.Fire<UpdateDailyTasks>();
        UpdateTimer();
    }
    
    
}