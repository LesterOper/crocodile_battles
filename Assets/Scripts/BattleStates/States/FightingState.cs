using System.Collections;
using App;
using App.Achievements;
using App.Analytic;
using App.Controllers;
using App.Counters;
using App.Crocodile.Interfaces;
using App.DAO;
using App.Signals;
using App.Utils;
using Battle;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using Zenject;

namespace BattleStates.States
{
    public class FightingState : BattleState
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ICrocodileController _playerController;
        private ICrocodileController _enemyController;
        private FightingController _fightingController;
        private BattleStateInfoHandler _handler;
        private float _enemyHealth, _playerHealth;
        [Inject]private SignalBus _signalBus;
        [Inject] private IGameInfoService _gameInfoService;
        [Inject] private CounterUtility _utility;
        [Inject] private IDataService dataService;
        [Inject] private IAnalytics _analytics;
        public FightingState(BattleStateMachine battleStateMachine, IBattleViewController battleViewController,
            ICoroutineRunner coroutineRunner, ICrocodileController playerController,
            ICrocodileController enemyController,  BattleStateInfoHandler handler, FightingController controller)
            : base(battleStateMachine, battleViewController)
        {
            _coroutineRunner = coroutineRunner;
            _playerController = playerController;
            _enemyController = enemyController;
            _handler = handler;
            _fightingController = controller;
        }
        
        
        public void ReplaceEnemy(ICrocodileController enemyController)
        {
            _enemyController = enemyController;
            
        }
        public override void Enter()
        {
            _signalBus.Subscribe<GameOverSignal>(SetBattleEnded);
            CustomEventManager.CompleteFightState.AddListener(CompleteStateEvent);
            _coroutineRunner.StartCoroutine(Fight());
            _enemyController.HideOpenedCards();
        }

        public override void Exit()
        {
            RestoreMana();
            _signalBus.TryUnsubscribe<GameOverSignal>(SetBattleEnded);
            CustomEventManager.CompleteFightState.RemoveListener(CompleteStateEvent);
        }

        private void RestoreMana()
        {
            int round = BattleViewController.roundIndex-1;
            int addManaPoints = round > 2 ? 5 : 3;
            _playerController.GetCrocodileInfo().StatsHandler.CurrentManaPoint +=
                addManaPoints; 
            _enemyController.GetCrocodileInfo().StatsHandler.CurrentManaPoint +=
                addManaPoints; 
        }

        private IEnumerator Fight()
        {
            CustomEventManager.InvokeBeforePreparingState();
            yield return new WaitForSeconds(0.25f);
            battleViewController.IncreaseRoundCounter();

            StartFight();
        }

        private void CompleteStateEvent()
        {
            CustomEventManager.CompleteFightState.RemoveAllListeners();
            if (_handler.IsBattleFinishedProperty)
            {
                _signalBus.Fire(new MoveEffectSignal(){ShowEffect = true});
                BattleViewController.roundIndex ++;

                if (_handler.IsPlayerWin && !_handler.IsEnemyWin)
                {
                    if(_enemyHealth <= 0)
                        _enemyController.Disappearing(BroadcastShowEndBattleView);
                    else
                        BroadcastShowEndBattleView();
                    _playerController.VictoryAnim();
                }
                else if(_handler.IsEnemyWin && !_handler.IsPlayerWin)
                {
                    if(_playerHealth <= 0)
                        _playerController.Disappearing(BroadcastShowEndBattleView);
                    else
                        BroadcastShowEndBattleView();
                    _enemyController.VictoryAnim();
                }
                else
                {
                    _playerController.Disappearing(BroadcastShowEndBattleView);
                    _enemyController.Disappearing(null);
                }

                if (_handler.IsPlayerWin)
                {
                    FightUtils.OnCompleteBattle(dataService);
                }
            }
            else
            {
                _fightingController.AttackStopped();
                CustomEventManager.InvokeResettingWeaponEvent();
                OnComplete();
            }
        }
        
        private void BroadcastShowEndBattleView() => _signalBus.Fire(new ShowEndBattleViewSignal()
            {IsPlayerWin = _handler.IsPlayerWin, IsEnemyWin = _handler.IsEnemyWin});


        private void StartFight()
        {
            _signalBus.Fire<DebufDamageSignal>();
            _handler.CheckIsCrocodilesOnBattlePoints();
            _fightingController.AttackStarted();
        }

        private void SetBattleEnded(GameOverSignal signal)
        {
            _playerHealth = signal.PlayerHealth; 
            _enemyHealth = signal.EnemyHealth; 
            _handler.IsPlayerWin = _playerHealth > 0 && _playerHealth > _enemyHealth;
            _handler.IsEnemyWin = _enemyHealth > 0 && _enemyHealth > _playerHealth;
            _handler.IsBattleFinishedProperty = true;
            _signalBus.Fire(new BattleEndSignal(){IsWin = _handler.IsPlayerWin, IsBoss = _gameInfoService.IsBossFight()});
            Exit();
            CompleteStateEvent();
            
            if (_gameInfoService.IsBossFight())
            {
                _utility.AppendMoney(CounterType.BOSS_BATTLES_WIN, 1);
            }
            else
            {
                if(_handler.IsPlayerWin)
                    _utility.AppendMoney(CounterType.BATTLES_WIN, 1);
            }
            if(_handler.IsEnemyWin)
                _utility.AppendMoney(CounterType.BATTLES_LOOSE, 1);
        }
    }
}