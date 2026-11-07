using System;
using System.Collections.Generic;
using App.Crocodile.Fasteners;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using App.Signals;
using Library;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Controllers
{
    public class WeaponRemover
    {
        private CardCharactaristicsUtils _utils;
        private ICrocodileController _player;
        private ICrocodileController _enemy;
        private SignalBus _signalBus;
        public WeaponRemover(ICrocodileController player, ICrocodileController enemy, SignalBus signalBus)
        {
            _utils = new CardCharactaristicsUtils();
            _player = player;
            _enemy = enemy;
            _signalBus = signalBus;
        }

        public void RemoveWeapons()
        { 
            RemovableFasteners(_player, true);
            RemovableFasteners(_enemy, false);
            RemoveArmor(_player, true);
            RemoveArmor(_enemy, false);
            
            _enemy.RandomizeCardsForCurrentRound();
        }
        
        private void RemovableFasteners(ICrocodileController croco, bool destroy)
        {
            List<Fastener> fasteners = croco.GetCrocodileInfo().BathFastenersHandler.GetFasteners();

            var deathWeapons = fasteners.FindAll(local => local.GetCurrentEquipment() != null && 
                                                          (local.RoundsDuration - local.ActiveRoundCount) <=0 && 
                                                          !local.GetCurrentEquipment().GetWeaponType().GetAttribute<WeaponData>().IsArmor);
            deathWeapons.ForEach(local => RemoveWeapon(local, croco, destroy));
        }

        private void RemoveArmor(ICrocodileController croco, bool destroy)
        {
            if(croco.GetCrocodileInfo().StatsHandler.IsArmorActive())
                return;
            List<Fastener> fasteners = croco.GetCrocodileInfo().BathFastenersHandler.GetFasteners();
            var deathWeapons = fasteners.FindAll(local => local.GetCurrentEquipment() != null && 
                                                          local.GetCurrentEquipment().GetWeaponType().GetAttribute<WeaponData>().IsArmor);
            deathWeapons.ForEach(local => RemoveWeapon(local, croco, destroy));
        }
        

        private void RemoveWeapon(Fastener fastener, ICrocodileController croco, bool destroy)
        {
            if(fastener.CardObject == null)
                return;
            WeaponType weaponType = (WeaponType)fastener.Card.GetCardType();
            ChangeStats(croco, fastener.Card);
            if (weaponType == WeaponType.UMBRELLA)
            {
                croco.GetCrocodileInfo().LongDistanceImmun = false;
            }
            if (weaponType == WeaponType.SNAKE_TUBE)
                croco.GetCrocodileInfo().CloseDistanceImmun = false;
            _signalBus.Fire(new RemoveCardFromListSignal(){Card = fastener.CardObject});
            if(destroy)
                GameObject.Destroy(fastener.CardObject.gameObject);
            GameObject.Destroy(fastener.GetCurrentEquipment().GetObject());
            fastener.BackCardFromFastener();
        }

        private void ChangeStats(ICrocodileController croco, ICardInfo cardInfo)
        {
            croco.GetCrocodileInfo().StatsHandler.CurrentAttackPointProperty -=
                _utils.GetAttackValueByLevel(cardInfo.GetLevel(), (WeaponType)cardInfo.GetCardType());
        }

        
    }
}