using System;
using System.Collections.Generic;
using App.DAO;
using App.Tutorial;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using CodeBase.Logic;
using Library.Common;
using Library.Data;
using UnityEngine;
using Zenject;

namespace App.CodeBase.Infrastructure.States
{
  public class GameStateMachine : StateMachine<IExitableState>
  {
    private IExitableState _activeState;
    private DiContainer _diContainer;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain curtain, DataHelper dataHelper, AdsHelper adsHelper, DiContainer diContainer, RectTransform gdprParent, TutorialController tutorialController)
    {
      states = new Dictionary<Type, IExitableState>
      {
        [typeof(BootstrapState)] = new BootstrapState(sceneLoader, dataHelper,  adsHelper, gdprParent, tutorialController),
        [typeof(LoadLevelState)] = new LoadLevelState(sceneLoader, curtain),
        [typeof(LoadProgressState)] = new LoadProgressState(),
        [typeof(GameLoopState)] = new GameLoopState(),
        [typeof(SplitterState)] = new SplitterState(),
        [typeof(BattleGameState)] = new BattleGameState(curtain),
      };
      _diContainer = diContainer;
    }

    public void InjectStates()
    {
      foreach (var state in states)
      {
        _diContainer.Inject(state.Value);
      }
    }
  }
}