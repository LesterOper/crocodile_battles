using App.CodeBase.Infrastructure.States;
using App.Counters;
using App.DAO;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.States;
using DG.Tweening;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs
{
    public class ChestNotificationView : Dialog
    {
        [SerializeField] private Button okButton;
        [SerializeField] private Image chestImage;
        private CounterUtility _utility;
        [Inject]private GameStateMachine _gameStateMachine;
        //[Inject] private IDataService _dataService;
        //[Inject] private SignalBus _signalBus;
        private void OnEnable()
        {
            ShowAnimation();
            okButton.onClick.AddListener(HideNotification);
            _hideDialogButtonArea.onClick.AddListener(HideNotification);
            if(_utility.GetMoney(CounterType.BATTLES_WIN) == 2)
                _closeButton.gameObject.SetActive(false);
        }

        
        
        public void SetChestImage(Sprite sprite)
        {
            _utility = new CounterUtility(_dataService, _signalBus);
            chestImage.sprite = sprite;
           
        }
        private void ShowAnimation()
        {
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 0, 0);
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveListener(HideNotification);
        }

        private void HideNotification()
        {
            _utility = new CounterUtility(_dataService, _signalBus);
            transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 0, 0).OnComplete(
                () =>
                {
                    gameObject.SetActive(false);
                    if (_utility.GetMoney(CounterType.BATTLES_WIN) == 2)
                    {
                        _gameStateMachine.Enter<LoadLevelState, string>("Main");
                    }
                });
        }
    }
}
