using System.Linq;
using App;
using App.Cards;
using App.Crocodile.Interfaces;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace BattleStates.States
{
    public class BattleTutorialHelper
    {
        private bool tutor;
        private ICrocodileController _playerController;
        private ICardsHandler _playerCards;
        private IBattleViewController _battleViewController;
        private SignalBus _signalBus;
        private IDataService _dataService;

        public BattleTutorialHelper(ICrocodileController player, ICardsHandler playerCards,
            IBattleViewController controller, SignalBus signalBus, IDataService dataService)
        {
            _playerController = player;
            _playerCards = playerCards;
            _battleViewController = controller;
            _signalBus = signalBus;
            _dataService = dataService;
        }
        public void TutorialBattle()
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();

            if (tutorialStepType.GetAttribute<TutorialAttribute>().IsBattle)
            {
                SendTutorialMessage(tutorialStepType);
                tutor = true;
                return;
            }

            tutor = false;
        }

        public void TutorialBattleBroadcast()
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();

            switch (tutorialStepType)
            {
                case TutorialStepType.MANA_HELP:
                {
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }
                case TutorialStepType.SHOOT_POINTER:
                {
                    PointerTargets(WeaponType.BOTTLE_GUN);
                    break;
                }
                
                case TutorialStepType.BOMB_HELP_POINTER:
                {
                    PointerTargets(WeaponType.BOMB_ILLNESS);
                    break;
                }

                case TutorialStepType.BRUSH_HELP:
                {
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }

                case TutorialStepType.HEALING_HELPER:
                {
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
                
                case TutorialStepType.BRUSH_HELP_POINTER:
                {
                    PointerTargets(WeaponType.BRUSH);
                    break;
                }

                case TutorialStepType.UMBRELLA_HELP_POINTER:
                {
                    PointerTargets(WeaponType.UMBRELLA);
                    break;
                }

                case TutorialStepType.HEALING_HELP_POINTER:
                {
                    PointerTargets(WeaponType.LIFEBUOY);
                    break;
                }

                case TutorialStepType.MINA_HELP_POINTER:
                {
                    PointerTargets(WeaponType.MINA_BOMB);
                    break;
                }
            }
        }
        
        private void SendTutorialMessage(TutorialStepType tutorialStepType)
        {
            switch (tutorialStepType)
            {
                
                case TutorialStepType.BOMB_HELP:
                {
                    _signalBus.Fire<ButtonInteractableInTutor>();
                    _signalBus.Fire<ShowTutorialSignal>();
                    _signalBus.Fire<TutorialSendTargetSignal>();
                    break;
                }

                case TutorialStepType.UMBRELLA_HELPER:
                {
                    _signalBus.Fire<ButtonInteractableInTutor>();
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
                
                case TutorialStepType.MINA_HELPER:
                {
                    _signalBus.Fire<ButtonInteractableInTutor>();
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }

                case TutorialStepType.FINISH_HIM:
                {
                    _signalBus.Fire<ShowTutorialSignal>();
                    break;
                }
            }
        }

        private void PointerTargets(WeaponType weaponType)
        {
            Transform targetFastener;
            if (weaponType.GetAttribute<WeaponData>().FilterType == FilterType.ONE_ACTION)
                targetFastener = _playerController.GetCrocodileInfo().BathFastenersHandler.BathRigidbody.transform;
            else
                targetFastener = _playerController.GetCrocodileInfo().BathFastenersHandler
                .GetFasteners().FirstOrDefault(fastener => fastener.GetActionType() == weaponType.GetAttribute<WeaponData>().ActionType)?.transform;
            
            _playerCards.GetCards().ForEach(card => card.ChangeDragState(false)); //.SetDraggingEnabled(false));
            Card targetCard = _playerCards.GetCards()
                .FirstOrDefault(card => (WeaponType)card.GetCardInfo().GetCardType() == weaponType);
            targetCard?.ChangeDragState(true); //SetDraggingEnabled(true);
            
            _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = targetCard.transform, PointerEndTarget = targetFastener, WeaponType = weaponType});
        }
    }
}