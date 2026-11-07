using System;
using System.Threading.Tasks;
using App.Crocodile;
using App.DAO;
using App.SetupsScript;
using BattleStates;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace Battle
{
    public class BossBattleController : BattleController
    {
        private const int INDEX_BOSS = 0;

        protected override Task InitializePlayers()
        {
            return base.InitializePlayers();
        }

        public override async Task SetupBattle(BattleStateInfoHandler infoHandler, SignalBus signalBus)
        {
            battleRule = new RandomBattleRule();
            SetEnemyBathData();
            await InitializePlayers();
            await base.SetupBattle(infoHandler, signalBus);
            battleStateMachine.FinishState += OnFinishState;
            NextState();
        }

        protected override void SetEnemyBathData()
        {
            if (_dataService.GetCurrentBoss() == 0)
            {
                _dataService.SetCurrentBoss(BathType.BOSS_1);
            }
            _dataService.SetLong("EnemyBath", (int)_dataService.GetCurrentBoss());
            EnemyGenerator.SetBathLevel(_dataService,0,0);
            base.SetEnemyBathData();
        }

        private void OnFinishState()
        {
            NextState(); 
        }

        private void NextState()
        {
            Type state = battleRule.GetNextState();
            Debug.Log($"[BattleController] Set state {state.Name}");
            battleStateMachine.Enter(state);
        }
    }
}