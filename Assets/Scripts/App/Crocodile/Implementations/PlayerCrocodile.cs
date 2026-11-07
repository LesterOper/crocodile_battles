using System.Collections.Generic;
using App.Cards;
using App.Crocodile.Fasteners;
using App.SetupsScript;
using Battle.Equipment;
using Zenject;

namespace App.Crocodile.Implementations
{
    public class PlayerCrocodile : CrocodileBase
    {
        private ICardsHandler _cardsHandler;
        
        public PlayerCrocodile( bool isPlayer, SignalBus signalBus) :
            base( isPlayer, signalBus) { }
        
        public override void ListenToFasteners(CrocodileController _crocodileController)
        {
            base.ListenToFasteners(_crocodileController);
            foreach (var fastener in _fasteners)
            {
                fastener.AddListener(WeaponEquipped);
            }
        }

        public override void WeaponEquipped(IEquipment equipment, bool hasFastener, int level) 
        {
            base.WeaponEquipped(equipment, hasFastener, level);
            equipment.isUsed = true;
        }

        private Fastener GetFastener(List<Fastener> freeFasteners, WeaponActionType actionType) => 
            freeFasteners.Find(fastener => fastener.GetActionType() == actionType);

        public override void AutoEquippingCrocodile()
        {
            
        }

        public override void Setup(CrocodileController crocodileController) => 
            base.Setup(crocodileController);

        public override void SetCardsHandler(ICardsHandler cardsHandler) => _cardsHandler = cardsHandler;
    }
}