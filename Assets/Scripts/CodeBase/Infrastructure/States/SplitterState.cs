using App;
using App.CodeBase.Infrastructure.States;
using CodeBase.Logic;
using UnityEngine.SceneManagement;
using Zenject;

namespace CodeBase.Infrastructure.States
{
  public class SplitterState : IState
  {
    private const string BattleScene = "BattleScene";
    
    [Inject]private readonly GameStateMachine _gameStateMachine;
    
    public void Exit() { }

    public void Enter()
    {
      CustomEventManager.CompleteFightState.RemoveAllListeners();
      if (SceneManager.GetActiveScene().name == BattleScene)
      {
        _gameStateMachine.Enter<BattleGameState>(); 
      }
      else
      {
        _gameStateMachine.Enter<GameLoopState>();
      }
    }
  }
}