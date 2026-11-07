using System;
using System.Collections.Generic;
using App.Analytic;
using App.Chests;
using App.CodeBase.Infrastructure.States;
using App.Counters;
using App.DAO;
using App.Signals;
using Battle;
using BattleStates.States;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using Library;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.EndGameDialog
{
    public class WinView : Dialog
    {
        [SerializeField] private Slider _pointsBar;
        [SerializeField] private Image _nextChestImage;
        [SerializeField] private Image _prevChestImage;
        [SerializeField] private Image _buttonIcon;
        [SerializeField] private TextMeshProUGUI _counterText;
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private RectTransform _parentDialog;
        [SerializeField] private RectTransform _finishIcon;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Button back;
        [SerializeField] private TextMeshProUGUI moneyBonusX2;
        private float _pointsPercentStart, _pointsPercentEnd;
        [Inject]private CounterUtility _utility;
        [Inject] private IAnalytics _analytics;
        private int _pointsToUpgrade;
        private const string BattleScene = "BattleScene";
        [Inject] private GameStateMachine _gameStateMachine;
        private void OnEnable()
        {
            _takeButton.onClick.AddListener(TakeChest);
            _battleButton.onClick.AddListener(StartBattle);
            back.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.back}});
                _gameStateMachine.Enter<LoadLevelState, string>("Main");
            });
            _okButton.onClick.AddListener(GoToMainScene);
        }

        private void GoToMainScene()
        {
            _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.home}});
            _parentDialog.gameObject.SetActive(false);
            _gameStateMachine.Enter<LoadLevelState, string>("Main");
        }

        private void OnDisable()
        {
            _takeButton.onClick.RemoveListener(TakeChest);
            back.onClick.RemoveListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.back}});
                _gameStateMachine.Enter<LoadLevelState, string>("Main");

            });
            _battleButton.onClick.RemoveListener(StartBattle);
            _okButton.onClick.RemoveListener(GoToMainScene);
        }

        private void Awake()
        {
           
        }
        
        public void Setup(float points, float maxPoints, float currentPoints, Sprite prevSprite, Sprite nextSprite, float money)
        {
            bool x2Active = _dataService.GetX2RewardState();
            _pointsToUpgrade = (int)maxPoints;
            _nextChestImage.sprite = nextSprite;
            _prevChestImage.sprite = prevSprite;
            _buttonIcon.sprite = prevSprite;
            _pointsPercentStart = (currentPoints - points) / maxPoints;
            _pointsPercentEnd = currentPoints / maxPoints;
            _pointsBar.value = _pointsPercentStart;
            _pointsBar.DOValue(_pointsPercentEnd, 1f);
            _utility.AppendMoney(CounterType.DEFAULT_MONEY, money + (x2Active ? money : 0));
            moneyBonusX2.gameObject.SetActive(x2Active);
            moneyBonusX2.text = "+" + money;
            _counterText.text = $"<color=#C7A72C>{(int)(currentPoints-points)}</color>/{_pointsToUpgrade}";
            moneyText.text = "+" + money;
            
            _counterText.gameObject.SetActive(true);
            DOTween.To(Setter, currentPoints - points, currentPoints, 1f).SetDelay(0.5f).OnComplete(ActivateButtons);
        }

        private void ActivateButtons()
        {
            if (_utility.GetNextChest() == ChestType.GIGA ||
                _utility.GetNextChest() == ChestType.MEGA)
            {
                _takeButton.gameObject.SetActive(true);
                _okButton.gameObject.SetActive(false);
                _takeButton.image.DOFade(1, 0.5f);
            }
            else
            {
                _takeButton.gameObject.SetActive(false);
                _okButton.gameObject.SetActive(true);
                _okButton.image.DOFade(1, 0.5f);
            }

            _battleButton.gameObject.SetActive(true);
            _battleButton.image.DOFade(1, 0.5f);
        }

        private void Setter(float i)
        {
            _counterText.text = $"<color=#C7A72C>{(int)i}</color>/{_pointsToUpgrade}";
            if (i >= _pointsToUpgrade)
            {
                if (_utility.GetNextChest() == ChestType.GIGA)
                {
                    var chest = _nextChestImage.gameObject;
                    
                    DOTween.KillAll();
                    TakeChestAnimation(chest);
                    _utility.ClearExactMoney(CounterType.POINTS);
                    _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == _utility.GetNextChest()).chestAmount += 1;
                    _dataService.UpdateChestCollectionData();
                    _utility.ResetChest();
                    _analytics.SendEvent(AmplitudeEvent.ui_result_collect, 
                        new Dictionary<string, object>(){{AmplitudePlacement.type.ToString(), ChestType.MEGA}});
                    _signalBus.Fire<ResetChestProgressSignal>();
                    _signalBus.Fire(new ChestNotificationSignal(){NextChestIcon = _nextChestImage});
                    return;
                }
                int remain = (int) (_pointsPercentEnd*_pointsToUpgrade - _pointsToUpgrade);
                _utility.ClearExactMoney(CounterType.POINTS);
                _utility.AppendMoney(CounterType.POINTS, remain);
                DOTween.KillAll();
                _signalBus.Fire<UpdateEndGameDialogSignal>();
            }
        }

        private void TakeChest()
        {
            var chest = _prevChestImage.gameObject;
            TakeChestAnimation(chest);
            _utility.ClearExactMoney(CounterType.POINTS);
            _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == _utility.GetPrevChest()).chestAmount += 1;
            _analytics.SendEvent(AmplitudeEvent.ui_result_collect, 
                new Dictionary<string, object>(){{AmplitudePlacement.type.ToString(), _utility.GetPrevChest()}});
            _dataService.UpdateChestCollectionData();
            _utility.ResetChest();
            _signalBus.Fire<ResetChestProgressSignal>();
            _signalBus.Fire(new ChestNotificationSignal(){NextChestIcon = _prevChestImage});
        }

        private void TakeChestAnimation(GameObject chest)
        {
            chest.transform.SetParent(GetComponentInParent<BattleEndView>().gameObject.transform);
            Sequence seq = DOTween.Sequence();
            seq.Append(chest.transform.DOScale(chest.transform.localScale*3f, 0.5f))
                .Append(chest.transform.DOMove(_finishIcon.transform.position, 1f))
                .Join(chest.transform.DOScale(Vector3.zero, 1f)).OnComplete(()=>
                {
                    Destroy(chest);
                });
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
    }
}