using App.CodeBase.Infrastructure.States;
using CodeBase.Infrastructure.Factory;
using CodeBase.Logic;
using Zenject;

namespace CodeBase.Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";

        [Inject]private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        [Inject]private readonly IGameFactory _gameFactory;

        public LoadLevelState( SceneLoader sceneLoader, LoadingCurtain curtain)
        {
            _sceneLoader = sceneLoader;
            _curtain = curtain;
        }

        public void Enter(string sceneName)
        {
            _curtain
                .SubscribeCurtainShowListener(CurtainHide)
                .Show(null);
      
            void CurtainHide()
            {
                _curtain.UnsubscribeCurtainShowListener(CurtainHide);
                _gameFactory.Cleanup();
                _sceneLoader.Load(sceneName, OnLoaded);
            }
        }

        public void Exit()
        {
     
        }

        private void OnLoaded()
        {
            _gameStateMachine.Enter<SplitterState>();
        }
        


    }
}