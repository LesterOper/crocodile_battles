using System;
using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.Crocodile;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Tutorial;
using UnityEngine;
using Zenject;

namespace Library.CanvasNamespace
{
    public class EnemyCardHandler : AbstractCardsHandler
    {
        [SerializeField] private EnemyCard _enemyCard;
        
        private List<EnemyCard> _enemyCards = new List<EnemyCard>();
        private Action showCardsByAds;

        public override void SetShowCardsByAds(Action action)
        {
            showCardsByAds = action;
        }

        public override void InvokeShowCardsByAds()
        {
            showCardsByAds.Invoke();
        }

        private List<ICardInfo> GetEnemiesCards()
        {
            List<ICardInfo> allCards = new List<ICardInfo>();
            List<BattleCardInfo> cardInfos = EnemyGenerator.GetEnemiesCards(_utility, _gameInfoService, _dataService);
            foreach (var card in cardInfos)
            {
                allCards.Add(new CardInfo() {ID = card.ID, type = card.type, Level = card.Level, WeaponType = card.WeaponType});
            }

            CardsInfo = allCards;
            return CardsInfo;
        }

        protected override void SetCardsInfo()
        {
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.TUTORIAL_BATTLE)
            {
                CardsInfo.Add(new CardInfo(){Level = 8, WeaponType = WeaponType.BOTTLE_GUN, type = WeaponActionType.ATTACK_LONG_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 10, WeaponType = WeaponType.BLASTER, type = WeaponActionType.ATTACK_LONG_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 2, WeaponType = WeaponType.BLASTER, type = WeaponActionType.ATTACK_LONG_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 6, WeaponType = WeaponType.PADDLE, type = WeaponActionType.ATTACK_CLOSE_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 1, WeaponType = WeaponType.BOMB_ILLNESS, type = WeaponActionType.ONE_ACTION_BOMB});
                CardsInfo.Add(new CardInfo(){Level = 3, WeaponType = WeaponType.WATER_FOUNTAIN, type = WeaponActionType.ATTACK_LONG_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 3, WeaponType = WeaponType.BRUSH, type = WeaponActionType.ATTACK_CLOSE_DISTANCE});
                CardsInfo.Add(new CardInfo(){Level = 3, WeaponType = WeaponType.BOW, type = WeaponActionType.ATTACK_LONG_DISTANCE});
            }
            else 
                CardsInfo = GetEnemiesCards();
        }

        protected override void InitializeCards()
        {
            _enemyCards.Clear();
            SetCardsInfo();
            for (int i = 0; i < 4; i++)
            {
                var card = Instantiate(_enemyCard, CardParent);
                card.Init();
                _enemyCards.Add(card);
            }
        }


        public override void Setup(bool isPlayer)
        {
            base.Setup(isPlayer);
            _enemyCards.ForEach(local =>
            {
                if(local != null)
                    Destroy(local.gameObject);
            });
            InitializeCards();
        }
        
    
        public override void SetCardUsedGraphic(WeaponType type)
        {
            base.SetCardUsedGraphic(type);

            var card = _enemyCards.FirstOrDefault(local =>
                (WeaponType)local.GetCard().GetCardInfo().GetCardType() == type);
            if(card != null)
                card.Show();
        }

        public override List<EnemyCard> GetEnemyCards()
        {
            return _enemyCards;
        }
    }
}