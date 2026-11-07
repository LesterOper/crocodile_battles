using System;
using System.Collections.Generic;
using App.Analytic;
using App.Chests;
using App.CodeBase.Infrastructure.States;
using App.Counters;
using App.DAO;
using App.Tutorial;
using App.Utils;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.EndGameDialog
{
    public class BossBattleEndDialog : Dialog
    {
        private const string PAYLOAD = "Main";
        [SerializeField] private GameObject winView;
        [SerializeField] private GameObject looseView;
        [Header("WinView")]
        [SerializeField] private GameObject textWin;
        [SerializeField] private Button okWin;
        [SerializeField] private Transform chest;
        [SerializeField] private TextMeshProUGUI moneyWin;
        [Header("LooseView")]
        [SerializeField] private Button okLoose;
        [SerializeField] private GameObject textLose;
        [SerializeField] private TextMeshProUGUI moneyLoose;
        [Inject] private GameStateMachine _gameStateMachine;

        [Inject]private CounterUtility _utility;
        [Inject] private IAnalytics _analytics;
        protected override void Start()
        {
            base.Start();
            okWin.onClick.AddListener(()=> _gameStateMachine.Enter<LoadLevelState, string>(PAYLOAD));
            okLoose.onClick.AddListener(()=> _gameStateMachine.Enter<LoadLevelState, string>(PAYLOAD));
            _hideDialogButtonArea.onClick.AddListener(() => _gameStateMachine.Enter<LoadLevelState, string>(PAYLOAD));
        }

        public void Setup(bool isWin)
        {
            _analytics.SendEvent(AmplitudeEvent.ui_result_show);
            winView.SetActive(isWin);
            looseView.SetActive(!isWin);
            //long moneyValue = isWin ? Random.Range(100, 200) : Random.Range(10, 30);
            if (isWin)
            {
                //moneyWin.text = "+" + moneyValue.ToString();
                _dataService.SetExtensionUnlock(Extension.X2_REWARD, true);
                _dataService.SetCurrentBoss(_dataService.GetCurrentBoss() + 1);
                FightUtils.OnCompleteBossBattle(_dataService);
                chest.transform.DOPunchScale(Vector3.one * 0.07f, 0.3f, 1, 0).SetDelay(0.2f).SetLoops(-1, LoopType.Restart);
                _dataService.GetChestCollectionData().ChestCollection.Find(chest => chest.type == ChestType.MEGA)
                    .chestAmount += 1;
                _dataService.UpdateChestCollectionData();
                _analytics.SendEvent(AmplitudeEvent.ui_result_collect, 
                    new Dictionary<string, object>(){{AmplitudePlacement.type.ToString(), ChestType.MEGA}});
                _analytics.SendEvent(AmplitudeEvent.battle_pass_step_collect, new Dictionary<string, object>(){{"type", "boss"},{"Bosslvl", _dataService.GetCurrentBoss()-1}});
            }
            //else moneyLoose.text = "+" + moneyValue.ToString();

            //_utility.AppendMoney(CounterType.DEFAULT_MONEY, moneyValue);
            _analytics.SendEvent(AmplitudeEvent.game_boss_end, 
                new Dictionary<string, object>(){{AmplitudePlacement.result.ToString(), isWin ? AmplitudePlacementProperty.win : AmplitudePlacementProperty.loose}});
        }

    }
}