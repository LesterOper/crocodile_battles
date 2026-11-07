using System;
using System.Collections.Generic;
using App.Cards;
using App.Controllers;
using App.Crocodile.Interfaces;
using App.DAO;
using Battle;
using BattleStates.States;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using Zenject;

namespace BattleStates
{
    public class BattleStateMachine : StateMachine<IBattleState>
    {
        private ICoroutineRunner _coroutineRunner;
        private IBattleController _battleController;
        private BattleStateInfoHandler _infoHandler;
        private FightingController _controller;
        private DiContainer _diContainer;
        public event Action FinishState;
        public BattleStateMachine(IBattleController battleController,  ICoroutineRunner coroutineRunner,
            ICrocodileController playerController, ICrocodileController enemyController,
            IBattleViewController battleViewController, ICardsHandler playerCardsHandler, ICardsHandler enemyCardHandler, BattleStateInfoHandler infoHandler, FightingController controller, DiContainer diContainer)
        {
            _battleController = battleController;
            _coroutineRunner = coroutineRunner;
            _infoHandler = infoHandler;
            _controller = controller;
            _diContainer = diContainer;
            states = new Dictionary<Type, IBattleState>
            {
                [typeof(PreBattleState)] = new PreBattleState(this, battleViewController, enemyController, playerController, _infoHandler),
                [typeof(PlayerPreparingState)] = new PlayerPreparingState(this, battleViewController,  enemyController, _battleController, playerController, playerCardsHandler, enemyCardHandler, _infoHandler, coroutineRunner),
                [typeof(FightingState)] = new FightingState(this, battleViewController,_coroutineRunner, playerController, enemyController, _infoHandler, _controller),
            };
        }

        public void RemoveState(Type state)
        {
            states.Remove(typeof(Type));
        }
        
        public void InjectDependencies()
        {
            foreach (var state in states)
            {
                _diContainer.Inject(state.Value);
            }
        }
        

        protected override TState ChangeState<TState>()
        {
            var changeState = base.ChangeState<TState>();
            return changeState;
        }

        protected override IState ChangeState(Type type)
        {
            var changeState = base.ChangeState(type);
            activeState.Complete += OnFinishState;
            return changeState;
        }

        private void OnFinishState()
        {
            activeState.Complete -= OnFinishState;
            
            FinishState?.Invoke();
        }
    }
}