using System;
using System.Collections.Generic;
using App.Achievements;
using App.Analytic;
using App.Audio;
using App.DAO;
using App.Signals;
using App.Tutorial;
using DG.Tweening;
using Library.Dialogs;
using Library.View;
using ModestTree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class BoostersDialog : Dialog, RecycleListView.IListAdapter
{
    [SerializeField] private ScrollRect _scrollRectCollection;
    [SerializeField] private RecycleListView listViewBoosters;
    [SerializeField] private Button fightButton;
    private List<BoosterInfo> _currentBoosters;
    [Inject] private IAnalytics _analytics;
    [Inject] private IGameInfoService _gameInfoService;
    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<UpdateItemsInListSignal>(UpdateItems);
    }
    
    protected override void Start()
    {
        base.Start();
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        Filter();
        listViewBoosters.SetAdapter(this);
        listViewBoosters.OnItemsChanged();
        gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
        _signalBus.Subscribe<UpdateItemsInListSignal>(UpdateItems);
        /*SetupButton(fightButton, () =>
        {
            RemoveChosenBoosters();
            _analytics.SendStartBattleEvent(AmplitudeEvent.game_start,AmplitudePlacementProperty.main, _dataService.GetBathData());
            _gameInfoService.SetIsBossFight(false);
            _gameInfoService.SetDoubleCardsInRound(false);
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.RANDOM_BATTLE_POINTER_HELP)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
            }
            _signalBus.Fire<LoadBattleSceneSignal>();
        });*/
    }

    public void SetFightButton(UnityAction action)
    {
        SetupButton(fightButton, ()=>
        {
            _soundPlayer.PlaySound("button_start_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            RemoveChosenBoosters();
            action.Invoke();
        });
    }

    private void RemoveChosenBoosters()
    {
        int index = 0;
        Dictionary<string, object> boosters = new Dictionary<string, object>();
        foreach (var booster in _dataService.GetBoosterCollectionData().BoosterCollection)
        {
            if (booster.isChosen)
            {
                boosters.Add($"booster_type_{index}", booster.BoosterType.ToString());
                booster.BoosterAmount--;
            }

            index++;
        }

        if (!boosters.IsEmpty())
        {
            _analytics.SendEvent(AmplitudeEvent.booster_use, boosters);
        }
        _dataService.UpdateBoosterCollectionData();
    }


    public void SetupButton(Button TargetButton, UnityAction Action)
    {
        if (TargetButton != null) TargetButton.onClick.AddListener(Action);
    }
    private void Filter()
    {
        _scrollRectCollection.verticalNormalizedPosition = 1f;
        GroupBoosters();
    }

    private void GroupBoosters()
    {
        _currentBoosters = _dataService.GetBoosterCollectionData().BoosterCollection;
    }
    
    private void UpdateItems()
    {
        listViewBoosters.OnItemsChanged();
    }
    public override void Hide()
    {
        _dataService.UpdateBoosterCollectionData();
        base.Hide();
    }
    public int GetItemsCount()
    {
        return _currentBoosters.Count;
    }

    public int GetViewTypeNumber(int position) => 0;

    public void SetupView(int position, int type, GameObject itemObject)
    {
        BoosterInfo boosterInfo = _currentBoosters[position];
        BoosterView boosterView = itemObject.GetComponent<BoosterView>();
        boosterView.Setup(boosterInfo.BoosterType);
        itemObject.transform.localScale = new Vector3(1.3f,1.3f, 0);
        itemObject.GetComponent<CanvasGroup>().alpha = 1;
    }
}