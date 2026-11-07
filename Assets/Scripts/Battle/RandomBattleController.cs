using System;
using System.Threading.Tasks;
using App.Counters;
using App.Crocodile;
using App.DAO;
using App.SetupsScript;
using BattleStates;
using CodeBase.Infrastructure.Factory;
using Library.CanvasNamespace;
using UnityEngine;
using Zenject;

namespace Battle
{
    public class RandomBattleController : BattleController
    {
        public override void Init(IGameFactory gameFactory, SignalBus signalBus, CounterUtility counterUtility, IDataService dataService, DiContainer diContainer)
        {
            base.Init(gameFactory, signalBus, counterUtility, dataService, diContainer);
            battleViewController.BattleController = this;
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
            WeightsRandomBathSettings settings = Resources.Load<WeightsRandomBathSettings>("WeightsSettings");
            WeightsRandomBathSettings.Modificator set = settings.GetModificator(_counterUtility);
            base.SetEnemyBathData();
            
            int type = (int)_dataService.GetBathData().CrocodileBathType + set.BathTypeMod;
            if (type <= 0) type = 1;
            if (type > 10) type = 10;
            
            
            EnemyGenerator.SetBathType(_dataService,(BathType)type);
            EnemyGenerator.SetBathLevel(_dataService, _dataService.GetBathData().Level, set.LevelMod);
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