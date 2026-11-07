using System.Collections.Generic;
using App.Analytic;
using App.CodeBase.Infrastructure.States;
using App.Counters;
using Battle;
using BattleStates.States;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.States;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.EndGameDialog
{
    public class LoseView : Dialog
    {
        [SerializeField] private Image _lostChestImage;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TextMeshProUGUI _restartCost;
        [SerializeField] private RectTransform _parentDialog;
        [Inject] private GameStateMachine _gameStateMachine;
        [Inject] private IAnalytics _analytics;
        private CounterUtility _utility;
        private const string BattleScene = "BattleScene";

        private void OnEnable()
        {
            _okButton.onClick.AddListener(LooseChest);
            _restartButton.onClick.AddListener(StartBattle);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(StartBattle);
            _okButton.onClick.RemoveListener(LooseChest);
        }

        private void Awake()
        {
            _utility = new CounterUtility();
        }

        private void StartBattle()
        {
            _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.battle}});
            _analytics.SendStartBattleEvent(AmplitudeEvent.game_start.ToString(), AmplitudePlacementProperty.result_screen, _dataService.GetBathData());
            _parentDialog.gameObject.SetActive(false);
            BattleViewController.roundIndex = 1;
            _gameStateMachine.Enter<LoadLevelState, string>(BattleScene);
            
        }
        
        private void LooseChest()
        {
            _utility.ClearExactMoney(CounterType.POINTS);
            _utility.ResetChest();
            _parentDialog.gameObject.SetActive(false);
            _gameStateMachine.Enter<LoadLevelState, string>("Main");
        }
    }
}