using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Achievements;
using App.AddressablesManager.Addressables;
using App.AddressablesTest;
using App.Analytic;
using App.Cards;
using App.Chests;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using Library;
using Zenject;

class AchievementsObserver : IAchievementObserver
{
    private IDataService _dataService;
     private SignalBus _signalBus;
     private TaskSetup _taskSetup;
     private IAnalytics _analytics;
    private string _lastTime;
    private TaskAndAchievementsUtility _utility;

    public void Setup(IDataService dataService, SignalBus signalBus, TaskSetup taskSetup, IAnalytics analytics)
    {
        _utility = new TaskAndAchievementsUtility(dataService);
        _taskSetup = taskSetup;
        _dataService = dataService;
        _signalBus = signalBus;
        _analytics = analytics;
        if (_dataService.GetTasks().TaskCollection.Count == 0)
        {
            CreateTasks();
            _utility.InitLastTime();
        }
        else if ((DateTime.Now - DateTime.Parse(_utility.GetLastTime())).Days >= 1)
        {
            ResetDailyTasks();
            
        }
        _signalBus.Subscribe<TakeCardSignal>(UpdateTakeCardsAchievement);
        _signalBus.Subscribe<BattleEndSignal>(UpdateBattleEndAchievements);
        _signalBus.Subscribe<ChestOpenedSignal>(UpdateChestOpenedAchievements);
        _signalBus.Subscribe<ChestTakenSignal>(UpdateChestTakenAchievement);
        _signalBus.Subscribe<UpdateDailyTasks>(ResetDailyTasks);
    }

    

    private void CreateTasks()
    {
        CreateDailyTasks(_taskSetup);
        CreateEternalTasks(_taskSetup);
        _dataService.UpdateTaskCollectionData();
    }

    private void CreateEternalTasks(TaskSetup taskSetup)
    {
        foreach (var task in taskSetup.GetEternalTasks())
        {
            _dataService.GetTasks().TaskCollection.Add(new TaskInfo
            {
                currentIndex = 0,
                type = task.GetTaskType, taskGroup = task.GetTaskGroup, taskTimeSpan = task.GetTaskTimeSpan,
                isCompleted = false, progress = 0, prizeType = task.GetPrizeInfo[0].GetPrizeType,
                prizeAmount = task.GetPrizeInfo[0].GetPrizeAmount,
                completeValue = task.GetStartCompleteTaskValue,
                prizeTaken = false, activateTime = DateTime.UtcNow.Ticks,
                CompleteValueCoefficient = task.CompleteValueCoefficient,
                PrizeCoefficient = task.PrizeAmountCoefficient
            });
        }
    }
    
    private void CreateDailyTasks(TaskSetup taskSetup)
    {
        _dataService.SetLong(Constants.DailyTasksCompletedByCurrentDay, 0);
        foreach (var task in taskSetup.GetDailyTasks())
        {
            _dataService.GetTasks().TaskCollection.Add(new TaskInfo
            {
                currentIndex = 0,
                type = task.GetTaskType, taskGroup = task.GetTaskGroup, taskTimeSpan = task.GetTaskTimeSpan,
                isCompleted = false, progress = 0, prizeType = task.GetPrizeInfo[0].GetPrizeType,
                prizeAmount = task.GetPrizeInfo[0].GetPrizeAmount,
                completeValue = task.GetStartCompleteTaskValue,
                prizeTaken = false, activateTime = DateTime.UtcNow.Ticks
            });
        }
    }

    private void ResetDailyTasks()
    { 
        _dataService.SetLong(Constants.DailyTasksCompletedByCurrentDay, 0);
        _dataService.GetTasks().TaskCollection.RemoveAll(task => task.taskTimeSpan == TaskTimeSpan.DAY);
        CreateDailyTasks(_taskSetup);
        _dataService.UpdateTaskCollectionData();
        _utility.SetDailyPrizeClaimed(false);
        _utility.InitLastTime();
    }
    
    private void UpdateTakeCardsAchievement(TakeCardSignal signal)
    {
        if (!_dataService.GetTutorialStepComplete(TutorialStepType.MAIN_TUTORIAL_END))
            return;
        _signalBus.Fire<AchieveSignal>();
        int level = signal.CardLevel;
        if (level == 0 || level == 1)
            return;
        TaskType taskType = $"CARDS_{level}_RANK_TAKEN".ToEnum<TaskType>();
        TaskInfo taskInfo = _dataService.GetTasks().TaskCollection
            .Find(task => task.type == taskType && !task.isCompleted);
        if(taskInfo != null)
            IncreaseProgress(taskInfo);
        CheckNewCardTypeTask(signal.WeaponType, signal.BathType);
        CheckMergeCardTask(signal.FromMegre);
        _dataService.UpdateTaskCollectionData();
    }
    
    private void CheckNewCardTypeTask(WeaponType weaponType, BathType bathType)
    {
        List<CardInfo> cardCollection = _dataService.GetCardCollectionData().Collection;
        List<BattleCardInfo> battlecardCollection = _dataService.GetBattleCardInfoData().CardInfos;
        List<BathInCollectionData> bathCollection = _dataService.GetBathCollection().BathDatas;
        BathData bathData = _dataService.GetBathData();
        
        if (weaponType != WeaponType.NONE && 
            (cardCollection.Exists(card => card.WeaponType == weaponType) ||
             battlecardCollection.Exists(card => card.WeaponType == weaponType)))
            return;

        if (bathType != BathType.NONE &&
            (bathCollection.Exists(card => card.CrocodileBathType == bathType) ||
             bathData.CrocodileBathType == bathType))
            return;

        TaskInfo taskInfo = _dataService.GetTasks().TaskCollection.Find(task => task.type == TaskType.CARDS_OPENED  && !task.isCompleted);
        if(taskInfo != null)
            IncreaseProgress(taskInfo);
    }
    
    private void CheckMergeCardTask(bool fromMerge)
    {
        if (fromMerge)
        {
            List<TaskInfo> taskInfos = _dataService.GetTasks().TaskCollection.FindAll(task => (task.type == TaskType.CARDS_MERGED ||
                task.type == TaskType.MERGE_2_CARDS) && !task.isCompleted);
            foreach (var taskInfo in taskInfos)
            {
                IncreaseProgress(taskInfo);
            }
        }
    }
    
    public void IncreaseProgress(TaskInfo taskInfo)
    {
        taskInfo.progress++;
        if (taskInfo.progress == taskInfo.completeValue)
        {
            taskInfo.isCompleted = true;
            if(taskInfo.taskTimeSpan == TaskTimeSpan.ETERNAL)
                _analytics.SendEvent(AmplitudeEvent.achievement_complete);
            else
                _analytics.SendEvent(AmplitudeEvent.daily_complete);
                
        }
        _dataService.UpdateTaskCollectionData();
    }
    
    public void UpdateBattleEndAchievements(BattleEndSignal signal)
    {
        if (!_dataService.GetTutorialStepComplete(TutorialStepType.MAIN_TUTORIAL_END))
            return;
        _signalBus.Fire<AchieveSignal>();
        List<TaskInfo> tasks = _dataService.GetTasks().TaskCollection.FindAll(task =>
                (task.type == TaskType.PLAY_3_BATTLE || task.type == TaskType.BATTLES_PLAYED) && !task.isCompleted);
        foreach (var task in tasks) 
        { 
            IncreaseProgress(task);
        }
        if (signal.IsWin)
        {
            List<TaskInfo> tasksWin = _dataService.GetTasks().TaskCollection.FindAll(task =>
                (task.type == TaskType.BATTLES_WIN || task.type == TaskType.WIN_1_BATTLE || 
                task.type == TaskType.WIN_3_BATTLE) && !task.isCompleted);
            foreach (var task in tasksWin)
            {
                IncreaseProgress(task);
            }

            if (signal.IsBoss)
            {
                List<TaskInfo> tasksWinBoss = _dataService.GetTasks().TaskCollection.FindAll(task =>
                    task.type == TaskType.BOSS_WIN && !task.isCompleted);
                foreach (var task in tasksWinBoss)
                {
                    IncreaseProgress(task);
                }
            }
        }
        
    }

    public void UpdateChestOpenedAchievements(ChestOpenedSignal signal)
    {
        if (!_dataService.GetTutorialStepComplete(TutorialStepType.MAIN_TUTORIAL_END))
            return;
        _signalBus.Fire<AchieveSignal>();
        switch (signal.ChestType)
        {
            case ChestType.MINI:
                NotifyOpenChestTask(TaskType.SMALL_CHEST_OPENED);
                break;
            case ChestType.MEDIUM:
                NotifyOpenChestTask(TaskType.MEDIUM_SCEST_OPENED);
                break;
            case ChestType.MEGA:
                NotifyOpenChestTask(TaskType.MEGA_CHEST_OPENED);
                break;
            case ChestType.GIGA:
                NotifyOpenChestTask(TaskType.GIGA_CHEST_OPENED);
                break;
        }
        _dataService.UpdateTaskCollectionData();
    }

    private void NotifyOpenChestTask(TaskType taskType)
    {
        if (!_dataService.GetTutorialStepComplete(TutorialStepType.MAIN_TUTORIAL_END))
            return;
        List<TaskInfo> taskInfos = _dataService.GetTasks().TaskCollection
            .FindAll(task => (task.type == taskType || task.type == TaskType.OPEN_1_CHEST) && !task.isCompleted);
        foreach (var chosenTask in taskInfos)
        {
            IncreaseProgress(chosenTask);
        }
        _dataService.UpdateTaskCollectionData();
    }

    public void UpdateChestTakenAchievement(ChestTakenSignal signal)
    {
        if (!_dataService.GetTutorialStepComplete(TutorialStepType.MAIN_TUTORIAL_END))
            return;
        _signalBus.Fire<AchieveSignal>();
        TaskInfo taskInfo = _dataService.GetTasks().TaskCollection
            .Find(task => task.type == TaskType.TAKE_1_CHEST && !task.isCompleted);
        if(taskInfo != null)
            IncreaseProgress(taskInfo);
    }
}

public class TaskAndAchievementsUtility
{
    private IDataService _dataService;

    public TaskAndAchievementsUtility(IDataService dataService)
    {
        _dataService = dataService;
    }
    
    public void InitLastTime()
    {
        _dataService.SetString("FirstTaskInit", DateTime.Now.ToString());
    }

    public void SetDailyPrizeClaimed(bool claimed)
    {
        _dataService.SetLong("DailyPrizeCLaimed", claimed ? 1 : 0);
    }

    public bool IsDailyPrizeClaimed()
    {
        return _dataService.GetLong("DailyPrizeCLaimed") == 1;
    }

    public bool IsDailyTaskReadyToClaim()
    {
        return _dataService.GetLong("DailyTaskReadyToClime") == 1;
    }

    public void SetDailyTaskReadyToClaim(bool ready)
    {
        _dataService.SetLong("DailyTaskReadyToClime", ready? 1:0);
    }
    
    public string GetLastTime() => _dataService.GetString("FirstTaskInit");
}


public static class StringExtension
{
    public static T ToEnum<T>(this string value)
    {
        return (T) Enum.Parse(typeof(T), value, true);
    }
}