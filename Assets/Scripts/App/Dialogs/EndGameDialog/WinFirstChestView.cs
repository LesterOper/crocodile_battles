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
using Zenject.Asteroids;

namespace App.Dialogs.EndGameDialog
{
    public class WinFirstChestView : Dialog
    {
        [SerializeField] private Slider _pointsBar;
        [SerializeField] private Image _nextChestImage;
        [SerializeField] private TextMeshProUGUI _counterText;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _battle;
        [SerializeField] private RectTransform _parentDialog;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Button back;
        [SerializeField] private RectTransform _finishIcon;
        [SerializeField] private RectTransform _description;
        [SerializeField] private TextMeshProUGUI x2BonusMoney;

        private float _pointsPercentStart, _pointsPercentEnd;
        private CounterUtility _utility;
        private int _pointsToUpgrade;
        private ChestType _nextChest;
        private IAnalytics _analytics;
        private GameStateMachine _gameStateMachine;

        [Inject]
        public void Construct( GameStateMachine gameStateMachine, CounterUtility counterUtility, IAnalytics analytics)
        {
            _gameStateMachine = gameStateMachine;
            _utility = counterUtility;
            _analytics = analytics;
        }

        public void Setup(float points, float maxPoints, float currentPoints)
        {
            if(back != null)
                back.onClick.AddListener(() =>
                {
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_complete);
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_result_complete);
                    TransferToMainScene();
                });
            _okButton.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_complete);
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_result_complete);
                TransferToMainScene();
            });
            _analytics.SendEvent(AmplitudeTutorialEvent.tutor_game_result_start);
            _pointsPercentStart = (currentPoints - points) / maxPoints;
            _pointsPercentEnd = currentPoints / maxPoints;
            _pointsBar.value = _pointsPercentStart;
            _pointsBar.DOValue(_pointsPercentEnd, 1f);
            _utility.ClearExactMoney(CounterType.POINTS);
        }

        private void TransferToMainScene(string sceneName = "Main")
        {
            _parentDialog.gameObject.SetActive(false);
            _gameStateMachine.Enter<LoadLevelState, string>(sceneName);
        }

        public void Setup(float points, float maxPoints, float currentPoints, float money)
        {
            _pointsToUpgrade = (int)maxPoints;
            if(back != null)
                back.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.back}});
                TransferToMainScene();
            });
            _okButton.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.home}});
                TransferToMainScene();
            });
            _battle.onClick.RemoveAllListeners();
            _battle.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.battle}});
                BattleViewController.roundIndex = 1;
                TransferToMainScene("BattleScene");
            });
            _pointsPercentStart = (currentPoints - points) / maxPoints;
            _pointsPercentEnd = currentPoints / maxPoints;
            _pointsBar.value = _pointsPercentStart;
            _pointsBar.DOValue(_pointsPercentEnd, 1f);
            _counterText.text = $"<color=#C7A72C>{(int)(currentPoints-points)}</color>/{_pointsToUpgrade}";
            _counterText.gameObject.SetActive(true);

            bool x2Active = _dataService.GetX2RewardState();
            moneyText.text = "+" + money;
            _utility.AppendMoney(CounterType.DEFAULT_MONEY, money + (x2Active ? money : 0));
            x2BonusMoney.gameObject.SetActive(x2Active);
            x2BonusMoney.text += money;
            
            DOTween.To(Setter, currentPoints - points, currentPoints, 1f)
                .SetDelay(0.3f)
                .OnComplete(ActivateButtons);
        }

        private void ActivateButtons()
        {
            _okButton.gameObject.SetActive(true);
            _battle.gameObject.SetActive(true);
            _okButton.image.DOFade(1, 0.5f);
            _battle.image.DOFade(1, 0.5f);
        }

        public void SetupForBeginners(float points, float maxPoints, float currentPoints, Sprite nextSprite, ChestType nextChest)
        {
            moneyText.text = "+" + 20;
            _utility.AppendMoney(CounterType.DEFAULT_MONEY, 20);
            _description.gameObject.SetActive(false);
            _nextChest = nextChest;
            _nextChestImage.sprite = nextSprite;
            _pointsToUpgrade = (int)maxPoints;
            if(back != null)
                back.onClick.AddListener(() =>
                {
                    _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                        new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.back}});
                    TransferToMainScene();
                });
            _okButton.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.home}});
                TransferToMainScene();
            });
            _battle.onClick.RemoveAllListeners();
            _battle.onClick.AddListener(() =>
            {
                _analytics.SendEvent(AmplitudeEvent.ui_result_close, 
                    new Dictionary<string, object>(){{AmplitudePlacement.source.ToString(), AmplitudePlacementProperty.battle}});
                _analytics.SendStartBattleEvent(AmplitudeEvent.game_start.ToString(), AmplitudePlacementProperty.result_screen, _dataService.GetBathData());
                BattleViewController.roundIndex = 1;
                TransferToMainScene("BattleScene");
            });     
            int currentWinCount = (int)_dataService.GetLong("WinCountForBeginner");
            _pointsPercentStart = (float) currentWinCount / maxPoints;  
            _pointsPercentEnd = (float)(currentWinCount+points) / maxPoints;
            _pointsBar.value = _pointsPercentStart;
            if(points != 0)
                _dataService.SetLong("WinCountForBeginner",_dataService.GetLong("WinCountForBeginner")+1);
            _counterText.text = $"<color=#C7A72C>{(int)currentWinCount}</color>/{_pointsToUpgrade}";
            _counterText.gameObject.SetActive(true);
            _pointsBar.DOValue(_pointsPercentEnd, 1f);
            DOTween.To(SetterForBeginners, (float)currentWinCount, (float)currentWinCount+points, 1f)
                .SetDelay(0.3f)
                .OnComplete(ActivateButtons);
        }

        private void SetterForBeginners(float i)
        {
            _counterText.text = $"<color=#C7A72C>{(int)i}</color>/{_pointsToUpgrade}";
            if (i >= _pointsToUpgrade)
            {
                _dataService.SetLong("WinCountForBeginner", 0);
                DOTween.KillAll();
                var chest = _nextChestImage.gameObject;
                TakeChestAnimation(chest);
                ActivateButtons();
                _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == _nextChest)
                    .chestAmount += 1;
                _dataService.UpdateChestCollectionData();
                _signalBus.Fire(new ChestNotificationSignal(){NextChestIcon = _nextChestImage});
            }
        }
        
        private void Setter(float i)
        {
            _counterText.text = $"<color=#C7A72C>{(int)i}</color>/{_pointsToUpgrade}";
            if (i >= _pointsToUpgrade)
            {
                int remain = (int) (_pointsPercentEnd*_pointsToUpgrade - _pointsToUpgrade);
                _utility.ClearExactMoney(CounterType.POINTS);
                _utility.AppendMoney(CounterType.POINTS, remain);
                DOTween.KillAll();
                if (_utility.GetNextChest() == ChestType.MINI)
                {
                    var chest = _nextChestImage.gameObject;
                    TakeChestAnimation(chest);
                    _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type ==ChestType.MINI).chestAmount += 1;
                    _dataService.UpdateChestCollectionData();
                    _signalBus.Fire(new ChestNotificationSignal(){NextChestIcon = _nextChestImage});
                }
                _signalBus.Fire<UpdateEndGameDialogSignal>();
            }
        }
        
        private void TakeChestAnimation(GameObject chest)
        {
            chest.transform.SetParent(GetComponentInParent<BattleEndView>().gameObject.transform);
            chest.transform.position = Vector3.zero;
            Sequence seq = DOTween.Sequence();
            seq.Append(chest.transform.DOScale(chest.transform.localScale*3f, 0.5f))
                .Append(chest.transform.DOMove(_finishIcon.transform.position, 1f))
                .Join(chest.transform.DOScale(Vector3.zero, 1f)).OnComplete(()=>
                {
                    Destroy(chest);
                });
        }
    }
}