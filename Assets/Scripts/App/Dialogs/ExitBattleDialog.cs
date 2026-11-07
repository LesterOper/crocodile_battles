using App.Counters;
using App.Signals;
using BattleStates.States;
using Library;
using Library.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs
{
    public class ExitBattleDialog : Dialog
    {
        [SerializeField] private Button exit;
        [SerializeField] private Button stay;
        private CounterUtility _utility;

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<GameOverSignal>(Hide);
        }

        protected override void Start()
        {
            base.Start();
            _signalBus.Subscribe<GameOverSignal>(Hide);
            _utility = new CounterUtility(_dataService, _signalBus);
            if (_hideDialogButtonArea != null)
                _hideDialogButtonArea.onClick.AddListener(Hide);
            exit.onClick.AddListener(() =>
            {
            
                BattleViewController.roundIndex = 1;
                _signalBus.Fire<ClearSubscribersAfterFightSignal>();
                _signalBus.Fire(new GameOverSignal(){EnemyHealth = 100, PlayerHealth = 50});
                _signalBus.Fire(new ShowEndBattleViewSignal(){IsPlayerWin = false, IsEnemyWin = true});
                _utility.AppendMoney(CounterType.BATTLES_LOOSE, 1);
                Hide();
            });
        
            stay.onClick.AddListener(Hide);
        }
        
    }
}
