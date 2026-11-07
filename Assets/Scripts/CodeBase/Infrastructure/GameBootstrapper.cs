using System;
using App.CodeBase.Infrastructure.States;
using App.DAO;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using CodeBase.Logic;
using Library.Common;
using Library.Data;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure
{
  public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
  {
    public LoadingCurtain CurtainPrefab;
    public CrocoDataHelper DataHelper;

    public TutorialController Controller;
    public AdsHelper AdsHelper;

    public RectTransform gdprParent;
    //public GDPRHandler GdprHandler;

    private SignalBus _signalBus;
    private GameStateMachine _stateMachine;
    private DiContainer _diContainer;

    public GameStateMachine StateMachine => _stateMachine;

    [Inject]
    public void Construct(SignalBus signalBus, DiContainer diContainer)
    {
      _signalBus = signalBus;
      _diContainer = diContainer;
    }

    private void Awake()
    {
      Debug.Log("FATAL ERROR : GameBootstapper 'Awake'");
      LoadingCurtain loadingCurtain = _diContainer.InstantiatePrefab(CurtainPrefab).GetComponent<LoadingCurtain>();
      Debug.Log("FATAL ERROR : GameBootstapper 'LoadingCurtain created'");
      _stateMachine = new GameStateMachine(new SceneLoader(this), loadingCurtain,  DataHelper, AdsHelper, _diContainer, gdprParent, Controller);
      Debug.Log("FATAL ERROR : GameBootstapper 'StateMachine created'");
    }

    private void Start()
    {
      Debug.Log("FATAL ERROR : GameBootstapper 'Start'");
      _stateMachine.InjectStates();
      Debug.Log("FATAL ERROR : GameBootstapper 'InjectStates'");
      _stateMachine.Enter<BootstrapState>();
      Debug.Log("FATAL ERROR : GameBootstapper 'Enter bootstrap state'");
      //Controller.Setup();

      DontDestroyOnLoad(gameObject);
    }
  }
}
