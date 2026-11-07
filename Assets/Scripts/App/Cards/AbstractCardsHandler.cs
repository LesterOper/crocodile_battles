using System;
using System.Collections.Generic;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace App.Cards
{
    public abstract class AbstractCardsHandler : MonoBehaviour, ICardsHandler
    {
        [SerializeField] protected Transform CardParent;
        [SerializeField] protected GameObject CardView;
        [SerializeField] protected GameObject OneActionCardView;
        [SerializeField] protected WeaponSetup _setup;
        [Inject] protected WeaponSetup _weaponSetup;
        [Inject]protected IDataService _dataService;
        [Inject] protected IGameInfoService _gameInfoService;
        [Inject] protected CounterUtility _utility;
        protected List<Card> Cards = new List<Card>();
        protected List<ICardInfo> CardsInfo = new List<ICardInfo>();
        protected List<ICardInfo> DiscardedCards = new List<ICardInfo>();
        protected bool isPlayer;
        protected int mana;

        public virtual void Setup(bool isPlayer) => this.isPlayer = isPlayer;
        public List< Card> GetCards() => Cards;
        

        protected virtual void SetCardsInfo()
        {
            var allCards = new List<ICardInfo>();
            List<BattleCardInfo> cardInfos = _dataService.GetBattleCardInfoData().CardInfos;
            foreach (var card in cardInfos)
            {
                allCards.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }
            
            allCards.Sort(delegate(ICardInfo x, ICardInfo y)
            {
                if ((WeaponType)x.GetCardType() == WeaponType.NONE && (WeaponType)y.GetCardType() == WeaponType.NONE) return 0;
                else if ((WeaponType)x.GetCardType() == WeaponType.NONE) return -1;
                else if ((WeaponType)y.GetCardType() == WeaponType.NONE) return 1;
                else return x.GetCardType().CompareTo((WeaponType)y.GetCardType());
            });
                
            CardsInfo = allCards;
        }

        protected virtual void InitializeCards()
        {
            
        }

        public virtual void SetCardUsedGraphic(WeaponType type) { }

        public virtual void SetWeaponEquippingHandler(WeaponEquipingHandler weaponEquipingHandler) { }

        public virtual void AddNextCard() { }

        public List<ICardInfo> GetChoosedCards() => CardsInfo;
        public List<ICardInfo> GetDiscardedCards() => DiscardedCards;
        public virtual List<EnemyCard> GetEnemyCards() => null;

        public virtual void SetShowCardsByAds(Action action) { }

        public virtual void InvokeShowCardsByAds(){ }
        public virtual void CardsWasUsed(){}
        public WeaponSetup GetWeaponSetup() => _weaponSetup;
    }

    
}