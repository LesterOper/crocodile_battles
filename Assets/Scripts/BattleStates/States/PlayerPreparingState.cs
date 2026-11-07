using App;
using App.Cards;
using App.Controllers;
using App.Crocodile.Interfaces;
using App.DAO;
using App.PrototypeBattle;
using App.Signals;
using App.Tutorial;
using Battle;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using Library;
using Zenject;

namespace BattleStates.States
{
    public class PlayerPreparingState : BattleState
    {
        private  ICrocodileController _enemyController;
        private ICrocodileController _playerController;
        private BattleStateInfoHandler _infoHandler;
        private ICardsHandler playerCards;
        private ICardsHandler enemyCards;
        private ICoroutineRunner _runner;
        private WeaponRemover _remover;
        private BattleTutorialHelper tutorial;
        private bool tutor;
        private long _idleTicks = 0;
        private long _startIdleTicks = 0;
        private long _battleTimer;
        private SignalBus _signalBus;
        private IDataService _dataService;
        private IGameInfoService _gameInfoService;
        
        public PlayerPreparingState(BattleStateMachine battleStateMachine, IBattleViewController battleViewController
            , ICrocodileController enemyController, IBattleController battleController
            , ICrocodileController playerController, ICardsHandler playerCardsHandler
            , ICardsHandler enemyCardHandler, BattleStateInfoHandler handler, ICoroutineRunner runner) 
            : base(battleStateMachine, battleViewController)
        {
            playerCards = playerCardsHandler;
            enemyCards = enemyCardHandler;
            _enemyController = enemyController;
            _enemyController.SetCardsHandler(enemyCardHandler);
            _playerController = playerController;
            _playerController.SetCardsHandler(playerCardsHandler);
            _infoHandler = handler;
            _runner = runner;
           
            //AddListeners();
        }

        private void AddListeners()
        {
            _signalBus.Subscribe<WeaponEquippedSignal>(OnWeaponEquipped);
            _signalBus.Subscribe<ClearSubscribersAfterFightSignal>(RemoveListener);
            _signalBus.Subscribe<BattleTutorialSignal>(tutorial.TutorialBattleBroadcast);
            _signalBus.Subscribe<GoToFightSignal>(GoToFight);
        }

        private void GoToFight()
        {
            OnComplete();
        }

        private void RemoveListener()
        {
            _signalBus.TryUnsubscribe<WeaponEquippedSignal>(OnWeaponEquipped);
            _signalBus.TryUnsubscribe<BattleTutorialSignal>(tutorial.TutorialBattleBroadcast);
            _signalBus.TryUnsubscribe<GoToFightSignal>(GoToFight);
        }
        

        private void CheckCrocoImmun()
        {
            CheckImmun(_playerController);
            CheckImmun(_enemyController);
        }
        
        private void CheckImmun(ICrocodileController croco)
        {
            var umbrella = croco.GetCrocodileInfo().BathFastenersHandler.GetFasteners()
                .Find(local => local.Card != null && (WeaponType)local.Card?.GetCardType() == WeaponType.UMBRELLA);
            if (umbrella != null)
            {
                croco.GetCrocodileInfo().LongDistanceImmun = true;
                umbrella.GetCurrentEquipment().EnableCollider(true);
            }
            
            var snakeTube = croco.GetCrocodileInfo().BathFastenersHandler.GetFasteners()
                .Find(local => local.Card != null && (WeaponType)local.Card?.GetCardType() == WeaponType.SNAKE_TUBE);
            if (snakeTube != null)
            {
                croco.GetCrocodileInfo().CloseDistanceImmun = true;
                snakeTube.GetCurrentEquipment().EnableCollider(true);
            }
        }

        [Inject]
        private void Construct(SignalBus signalBus, IDataService dataService, IGameInfoService gameInfoService)
        {
            _signalBus = signalBus;
            _dataService = dataService;
            _gameInfoService = gameInfoService;
            tutorial = new BattleTutorialHelper(_playerController, playerCards, base.battleViewController, _signalBus, _dataService);
            _remover = new WeaponRemover(_playerController, _enemyController, _signalBus);
            AddListeners();
        }

        public override void Enter()
        {
            if (_dataService.GetTutorialStepComplete(TutorialStepType.READY_BUTTON_HELP_4))
                battleViewController.ChangeReadyButtonState(true);
            if(_dataService.GetTutorialCurrentStep() == TutorialStepType.TUTORIAL_BATTLE)
                battleViewController.ChangeReadyButtonState(false);
            _remover.RemoveWeapons();
            //CheckCrocoImmun();
            tutorial.TutorialBattle();
            battleViewController.SetPreparingText();
            CustomEventManager.InvokeActivateCardsEvent(true);
        }

        public override void Exit()
        {
            //RemoveListener();
            battleViewController.ChangeReadyButtonState(false);
            CustomEventManager.InvokeActivateCardsEvent(false);
        }

        private void OnWeaponEquipped(WeaponEquippedSignal signal)
        {
            _playerController.GetCrocodileInfo().EquipingHandler.ClearBombSlot();
            if (!_infoHandler.IsAllCards || !_infoHandler.IsEnemyAllCards)
            { 
                _enemyController.AutoEquippingCrocodile();
            }
            playerCards.AddNextCard();
        }
    }
}