using System.Threading.Tasks;
using App.Cards;
using App.Controllers;
using App.Counters;
using App.Crocodile;
using App.Crocodile.Interfaces;
using App.DAO;
using BattleStates;
using BattleStates.States;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using Library.CanvasNamespace;
using UnityEngine;
using Zenject;

namespace Battle
{
    public abstract class BattleController : IBattleController
    {
        protected IBattleViewController battleViewController;
        protected ICrocodileController playerController;
        protected ICrocodileController enemyController;
        protected ICoroutineRunner coroutineRunner;
        protected ICardsHandler _abstractPlayerCardsHandler;
        protected ICardsHandler _abstractEnemyCardsHandler;
        protected IGameFactory gameFactory;
        protected IDataService _dataService;
        protected IBattleRule battleRule;
        protected BattleStateInfoHandler _infoHandler;
        protected BattleStateMachine battleStateMachine;
        protected FightingController _controller;
        protected SignalBus _signalBus;
        protected CounterUtility _counterUtility;
        private DiContainer _diContainer;
        public IBattleRule BattleRule => battleRule;
        public virtual void Init(IGameFactory gameFactory, SignalBus signalBus, CounterUtility counterUtility, IDataService dataService, DiContainer diContainer)
        {
            _counterUtility = counterUtility;
            this.gameFactory = gameFactory;
            _signalBus = signalBus;
            _dataService = dataService;
            _diContainer = diContainer;
            coroutineRunner = GameObject.FindObjectOfType<CoroutineRunner>();   // todo
            battleViewController = GameObject.FindObjectOfType<BattleViewController>(); // todo
            _controller = new FightingController();
        }

        public virtual void InitializeCards()
        {
            _abstractPlayerCardsHandler = null;
            _abstractEnemyCardsHandler = null;
            _abstractPlayerCardsHandler = GameObject.FindObjectOfType<PlayerCardsHandler>();
            _abstractPlayerCardsHandler.Setup(true);
            _abstractEnemyCardsHandler =  GameObject.FindObjectOfType<EnemyCardHandler>();
            _abstractEnemyCardsHandler.Setup(false);
        }
        
#pragma warning disable 1998
        public virtual async Task SetupBattle(BattleStateInfoHandler infoHandler, SignalBus signalBus)
#pragma warning restore 1998
        {
            InitializeCards();
            _infoHandler = infoHandler;
            _infoHandler.SetCardsHandlers(playerController, enemyController);
            _controller.SetupFightingController(playerController, enemyController, coroutineRunner);
            battleStateMachine = null;
            battleStateMachine = new BattleStateMachine(this, coroutineRunner, playerController, enemyController, 
                battleViewController, _abstractPlayerCardsHandler, _abstractEnemyCardsHandler,_infoHandler,_controller, _diContainer);
            battleStateMachine.InjectDependencies();
        }
        
        protected virtual async Task InitializePlayers()
        {
            var playerZeroPoint = BattlePoints.Instance.GetPointTransform(BattlePointType.PLAYER_ZERO_POINT);
            var enemyZeroPoint = BattlePoints.Instance.GetPointTransform(BattlePointType.ENEMY_ZERO_POINT);
            
            await CreatePlayers(playerZeroPoint.transform.position, enemyZeroPoint.transform.position);
            
            playerController.StartCrocodile(playerZeroPoint, battleViewController.Curve(AnimationType.LIGHT), battleViewController.Curve(AnimationType.LIGHT_1), enemyController, coroutineRunner, true, _signalBus);
            enemyController.StartCrocodile(enemyZeroPoint, battleViewController.Curve(AnimationType.LIGHT), battleViewController.Curve(AnimationType.LIGHT_1), playerController, coroutineRunner, true, _signalBus);
        }

        protected virtual void SetEnemyBathData()
        {
            
        }
        
        protected virtual async Task CreatePlayers(Vector3 playerPos, Vector3 enemyPos)
        {
            playerController = await gameFactory.CreateCrocodile(this, playerPos, "[Player]", true);
            enemyController = await gameFactory.CreateCrocodile(this, enemyPos, "[Enemy]", false);
        }
    }
}