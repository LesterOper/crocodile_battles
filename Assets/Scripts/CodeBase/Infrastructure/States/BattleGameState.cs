using App.CodeBase.Infrastructure.States;
using App.Counters;
using App.DAO;
using App.Signals;
using Battle;
using BattleStates;
using BattleStates.States;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Logic;
using Effects;
using Zenject;

namespace CodeBase.Infrastructure.States
{
    public class BattleGameState : IState
    {
        private readonly LoadingCurtain _curtain;
        private IBattleController _battleController;
        private BattleStateInfoHandler _infoHandler;
        [Inject] private IGameInfoService _gameInfoService;
        [Inject] private IGameFactory _gameFactory;
        [Inject] private SignalBus _signalBus;
        [Inject] private CounterUtility _counterUtility;
        [Inject] private IDataService _dataService;
        [Inject] private DiContainer _diContainer;
        public BattleGameState( LoadingCurtain curtain)
        {
            _curtain = curtain;
            _infoHandler = new BattleStateInfoHandler();
        }

        public void Enter()
        {
            _battleController = new BattleControllersHandler().GetBattleController(_gameInfoService.IsBossFight() ? BattleType.BOSS : BattleType.RANDOM);
            _gameInfoService.IsEnemyCardsOpenByAds = false;
            LogoScreen.isCurrentSession = true;
            SetupBattle();
        }

        public void Exit()
        {
            _infoHandler = null;
            _battleController = null;
        }

        private async void SetupBattle()
        {
            _battleController = new BattleControllersHandler().GetBattleController(_gameInfoService.IsBossFight() ? BattleType.BOSS : BattleType.RANDOM);
            _infoHandler = new BattleStateInfoHandler();
            _battleController.Init(_gameFactory, _signalBus, _counterUtility, _dataService, _diContainer);
            await _battleController.SetupBattle(_infoHandler, _signalBus);
            _curtain.Hide(() =>
            {
                _signalBus.Fire<ShowTutorialSignal>();
                //Messenger.Broadcast(MessengerKeys.TUTOR);
            });
        }

    }
}