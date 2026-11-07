using App.CodeBase.Infrastructure.States;
using CodeBase.Infrastructure.Services;
using CodeBase.Logic;
using Library.Common;
using Library.Data;
using Zenject;

namespace CodeBase.Infrastructure
{
  public class Game
  {
    public readonly GameStateMachine StateMachine;

    public Game(ICoroutineRunner coroutineRunner, LoadingCurtain curtain, DataHelper dataHelper, AdsHelper adsHelper, SignalBus signalBus)
    {
      //StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), curtain, AllServices.Container, dataHelper, adsHelper, signalBus);
    }
  }
}