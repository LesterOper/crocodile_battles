using System;
using System.Collections.Generic;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Weapon.WeaponBases;

namespace App.Cards
{
    public interface ICardsHandler
    {
        void Setup(bool isPlayer);
        List<Card> GetCards();
        void SetCardUsedGraphic(WeaponType type);
        void SetWeaponEquippingHandler(WeaponEquipingHandler weaponEquipingHandler);
        void AddNextCard();
        List<ICardInfo> GetChoosedCards();
        List<ICardInfo> GetDiscardedCards();
        List<EnemyCard> GetEnemyCards();
        void SetShowCardsByAds(Action action);
        void InvokeShowCardsByAds();
        void CardsWasUsed();
        WeaponSetup GetWeaponSetup();
    }
}