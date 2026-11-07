using System.Collections.Generic;
using System.Linq;
using App;
using App.Achievements;
using App.Analytic;
using App.DAO;
using App.Dialogs.Merge;
using App.Signals;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Dialogs;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AchievementDialog : Dialog, RecycleListView.IListAdapter
{
    [SerializeField] private ScrollRect _scrollRectCollection;
    [SerializeField] private TaskFilterButtonHandler dailyTasksButton;
    [SerializeField] private TaskFilterButtonHandler eternalTasksButton;
    [SerializeField] private RecycleListView listViewTasks;
    [SerializeField] private TaskSetup taskSetup;
    [SerializeField] private GameObject dailyTasksProgressBar;
    [SerializeField] private Image dailyTasksBar;
    [SerializeField] private TextMeshProUGUI dailyTasksProgressText;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private Image dailyTaskPrizeIcon;
    [SerializeField] private DailyTaskPrizeSetup dailyTaskPrizeSetup;
    [SerializeField] private DailyTaskProgress dailyTaskProgress;
    [SerializeField] private TextMeshProUGUI allDailyTasksPrize;
    [Inject] private IAnalytics _analytics;
    private TaskFilterType _taskFilterType = TaskFilterType.NONE;
    private List<TaskInfo> _currentFilteredTasks;
    public GameObject FirstElem;
    private RectTransform contentRect;
    private TaskAndAchievementsUtility _utility;
    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<UpdateItemsInListSignal>(UpdateItems);
    }

    private void Awake()
    {
        
    }

    protected override void Start()
    {
        base.Start();
        _utility = new TaskAndAchievementsUtility(_dataService);
        int completedDays = (int) _dataService.GetLong(Constants.DailyTasksCompletedDays);
        if (_utility.IsDailyPrizeClaimed() || _utility.IsDailyTaskReadyToClaim())
        {
            SetMainDailyPrize(completedDays-1);
        }
        else
        {
            SetMainDailyPrize(completedDays);
        }
        contentRect = listViewTasks.GetComponent<RectTransform>();
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        InitFilter();
        ChangeSprite(_taskFilterType);
        Filter();
        dailyTasksButton.AddListener(()=>SetButtonListener(TaskFilterType.DAILY));
        eternalTasksButton.AddListener(()=>SetButtonListener(TaskFilterType.ETERNAL));
        listViewTasks.SetAdapter(this);
        listViewTasks.OnItemsChanged();
        gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
        _signalBus.Subscribe<UpdateItemsInListSignal>(UpdateItems);
        _signalBus.Subscribe<UpdateDailyTaskListSignal>(UpdateDailyTasksList);
        _analytics.SendEvent(AmplitudeEvent.ui_daily_show);
    }

    private void SetMainDailyPrize(int day)
    {
        dailyTaskPrizeIcon.sprite =
            dailyTaskPrizeSetup.GetTaskPrizes()[day]
                .GetPrizeIcon();
        allDailyTasksPrize.text =
            dailyTaskPrizeSetup.GetTaskPrizes()[day]
                .GetPrizeAmout().ToString();
    }

    private void UpdateDailyTasksList()
    {
        SetupButton(TaskFilterType.DAILY);
    }
    
    private void SetButtonListener(TaskFilterType filterType)
    {
        SetupButton(filterType);
    }

    private void SetupButton(TaskFilterType taskFilterType)
    {
        SetFilter(taskFilterType);
        ChangeSprite(_taskFilterType);
        Filter();
        listViewTasks.OnItemsChanged();
    }
    
    
    private void Filter() => _scrollRectCollection.verticalNormalizedPosition = 1f;
    
    private void ChangeSprite(TaskFilterType taskFilterType)
    {
        dailyTasksButton.ChangeSprite(TaskFilterType.DAILY == taskFilterType);
        eternalTasksButton.ChangeSprite(TaskFilterType.ETERNAL == taskFilterType);
    }
    
    private void InitFilter() => SetFilter(TaskFilterType.DAILY);
    private void SetFilter(TaskFilterType type)
    {
        _taskFilterType = type;
        GroupTasks();
        
    }

    private void SetupDailyTasksProgress()
    {
        long completedTasks = _dataService.GetLong(Constants.DailyTasksCompletedByCurrentDay);
        dailyTasksBar.fillAmount = completedTasks / 3f;
        dailyTasksProgressText.text = $"{completedTasks}/{3}";
    }
    
    private void GroupTasks()
    {
        if (_taskFilterType == TaskFilterType.DAILY)
        {
            contentParent.localPosition = new Vector3(-115.423f, -118.323f, 0);
            contentParent.sizeDelta = new Vector2(727.816f, 386.331f);
            dailyTaskProgress.gameObject.SetActive(true);
            dailyTasksProgressBar.SetActive(true);
            SetupDailyTasksProgress();
            _currentFilteredTasks = _dataService.GetTasks().TaskCollection.FindAll(task =>
                task.taskTimeSpan == TaskTimeSpan.DAY);
            SortTasks();
        }

        if (_taskFilterType == TaskFilterType.ETERNAL)
        {
            contentParent.localPosition = new Vector3(2.153f, -38.811f, 0);
            contentParent.sizeDelta = new Vector2(963.01f, 545.356f);
            dailyTaskProgress.gameObject.SetActive(false);
            dailyTasksProgressBar.SetActive(false);
            _currentFilteredTasks = _dataService.GetTasks().TaskCollection.FindAll(task =>
                task.taskTimeSpan == TaskTimeSpan.ETERNAL);
           SortTasks();
        }
    }

    private void SortTasks()
    {
        _currentFilteredTasks = _currentFilteredTasks.OrderBy(x => x.isCompleted).ThenBy(x => x.progress).Reverse().ToList();
    }
    
    private void UpdateItems()
    {
        listViewTasks.OnItemsChanged();
        switch (_taskFilterType)
        {
            case TaskFilterType.DAILY:
                SetupButton(TaskFilterType.DAILY);
                break;
            case TaskFilterType.ETERNAL:
                SetupButton(TaskFilterType.ETERNAL);
                break;
        }
    }

    public override void Hide()
    {
        _dataService.UpdateTaskCollectionData();
        _signalBus.Fire<AchieveSignal>();
        base.Hide();
    }
    
    public int GetItemsCount()
    {
        if (_taskFilterType == TaskFilterType.NONE)
            return 0;
        return _currentFilteredTasks.Count;
    }

    public int GetViewTypeNumber(int position)
    {
        switch (_taskFilterType)
        {
            case TaskFilterType.DAILY:
                return 0;
            case TaskFilterType.ETERNAL:
                return 1;
            case TaskFilterType.NONE:
                break;
        }
        return 0;
    }

    public void SetupView(int position, int type, GameObject itemObject)
    {
        if (position == 0) FirstElem = itemObject;
        TaskInfo taskInfo = _currentFilteredTasks[position];
        TaskView taskView = itemObject.GetComponent<TaskView>();
        taskView.Setup(taskInfo.type);
        itemObject.transform.localScale = new Vector3(1f,1f, 1);
        itemObject.GetComponent<CanvasGroup>().alpha = 1;
    }
}