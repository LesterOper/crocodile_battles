using App.CodeBase.Infrastructure.States;
using App.DAO;
using CodeBase.Infrastructure.Services;
using CodeBase.Logic;
using Zenject;

namespace CodeBase.Infrastructure.States
{
    public class GameLoopState : IState
    {
        [Inject] private IGameInfoService _gameInfoService;
        [Inject] private IDataService _dataService;
        
        public void Exit()
        {

        }

        public void Enter()
        {
            _gameInfoService.SetIsBossFight(false);
            _dataService.SetLastSeesionTicks();
        }

    }
}