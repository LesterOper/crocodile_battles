using System.Collections.Generic;
using System.Linq;
using App;
using App.Crocodile.Interfaces;
using App.DAO;
using App.Signals;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace BattleStates.States
{
    public class PreBattleState : BattleState
    {
        private ICrocodileController _enemyController;
        private ICrocodileController _playerController;
        private BattleStateInfoHandler _infoHandler;
        private bool _boostersIsDone;
        private float _delay;
        [Inject] private SignalBus _signalBus;
        [Inject] private IDataService _dataService;
        public PreBattleState(BattleStateMachine battleStateMachine, IBattleViewController battleViewController,
            ICrocodileController enemyController, ICrocodileController playerController,
            BattleStateInfoHandler handler) : base(battleStateMachine, battleViewController)
        {
            _enemyController = enemyController;
            _playerController = playerController;
            _infoHandler = handler;
        }

        public override void Exit()
        {
            
        }

        public override void Enter()
        {
            battleViewController.ChangeReadyButtonState(false);
            CustomEventManager.InvokeActivateCardsEvent(false);
            if (!_boostersIsDone)
                _delay = 1.5f;
            else
                _delay = 0f;
            DOVirtual.DelayedCall(_delay, CheckBoosters);
        }

        private void CheckBoosters()
        {
            List<BoosterInfo> boosters = _dataService.GetBoosterCollectionData().BoosterCollection;//.Where(booster => booster.isChosen).ToList();
            foreach (var booster in boosters)
            {
                _signalBus.Fire(new ActivateBoosterSignal(){BoosterType = booster.BoosterType, IsUsed = !booster.isChosen});
                booster.isChosen = false;
            }
            _boostersIsDone = true;
            DOVirtual.DelayedCall(_delay, OnComplete);
        }
    }
}