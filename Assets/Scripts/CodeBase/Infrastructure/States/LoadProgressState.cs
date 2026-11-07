using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library.Utils;
using Zenject;

namespace CodeBase.Infrastructure.States
{
  public class LoadProgressState : IState
  {
    [Inject]private readonly GameStateMachine _gameStateMachine;
    [Inject]private readonly IDataService _dataService;
    
    public void Enter()
    {
      if(_dataService.GetTutorialCurrentStep().GetAttribute<TutorialAttribute>().IsBattle)
        _gameStateMachine.Enter<LoadLevelState, string>("BattleScene");
      else 
        _gameStateMachine.Enter<LoadLevelState,string>("Main");
    }

    public void Exit()
    {
      
    }
  }
}