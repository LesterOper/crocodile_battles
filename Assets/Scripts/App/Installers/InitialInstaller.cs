using System.Collections;
using System.Collections.Generic;
using App.CodeBase.Infrastructure.States;
using CodeBase.Infrastructure;
using UnityEngine;
using Zenject;

public class InitialInstaller : MonoInstaller
{
    //[SerializeField] private GameBootstrapper _gameBootstrapper;
    public override void InstallBindings()
    {
        //Container.Bind<GameStateMachine>().FromMethod(() => _gameBootstrapper.StateMachine).AsSingle();
    }
}
