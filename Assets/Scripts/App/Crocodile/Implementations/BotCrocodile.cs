using App.Achievements;
using App.Cards;
using App.Signals;
using Zenject;

namespace App.Crocodile.Implementations
{
    public class BotCrocodile : CrocodileBase
    {
        private ICardsHandler _cardsHandler;
        private BotTacticController botTactic;
        private bool _isEnemyFreeze;
        public BotCrocodile(bool isPlayer, SignalBus signalBus) :
            base(isPlayer, signalBus)
        {
            _signalBus.Subscribe<ActivateBoosterSignal>(ActivateFreezeBooster);
        }

        private void ActivateFreezeBooster(ActivateBoosterSignal signal)
        {
            if(signal.BoosterType != BoosterType.FREEZE)
                return;
            _isEnemyFreeze = !signal.IsUsed;
        }

        public override void HideOpenedCards()
        {
            if(!_isEnemyFreeze)
                botTactic.HideOpenedCards();
        } 

        public override void RandomizeCardsForCurrentRound()
        {
            base.RandomizeCardsForCurrentRound();
            if (!_isEnemyFreeze)
            {
                botTactic = new BotTacticController( _crocodileController,  _cardsHandler, _cardsHandler.GetWeaponSetup(), _dataService, _gameInfoService);
                botTactic.ChooseCards();
            }
        }
        
        public override void ListenToFasteners(CrocodileController _crocodileController)
        {
            base.ListenToFasteners(_crocodileController);
            foreach (var fastener in _fasteners)
            {
                fastener.AddListener(WeaponEquipped);
            }
        }

        public override void Setup(CrocodileController crocodileController) => base.Setup(crocodileController);

        public override void AutoEquippingCrocodile()
        {
            if(!_isEnemyFreeze)
                botTactic.EquipCard();
        } 

        public override void SetCardsHandler(ICardsHandler cardsHandler) => _cardsHandler = cardsHandler;
    }
}