using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.Chests;
using App.Crocodile.Fasteners;
using App.Crocodile.Handlers;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Tutorial;
using BattleStates.States;
using CodeBase.Infrastructure.Services;
using Library.Collections;
using Library.Utils;
using UnityEngine;

namespace App.Crocodile.Implementations
{
    public class BotTacticController
    {
        private CrocodileController _oppositeCroco;
        private CrocodileStatsHandler _statsHandler;
        private ICardsHandler _handler;
        private List<ICardInfo> chosenInThatRound = new List<ICardInfo>();
        private List<Card> chosenInThatRoundCardObjects = new List<Card>();
        private List<Fastener> _fasteners = new List<Fastener>();
        private IDataService _dataService;
        private int _usingCardIndex;
        private WeaponSetup _weaponSetup;
        private IGameInfoService _gameInfoService;
        private List<ICardInfo> _discardedCards = new List<ICardInfo>();
        public BotTacticController(CrocodileController oppositeCroco,
            ICardsHandler handler, WeaponSetup weaponSetup, IDataService dataService, IGameInfoService gameInfoService)
        {
            _usingCardIndex = BattleViewController.roundIndex-1;
            _oppositeCroco = oppositeCroco;
            _handler = handler;
            _handler.SetShowCardsByAds(ShowChosenCards);
            _dataService = dataService;
            _weaponSetup = weaponSetup;
            _gameInfoService = gameInfoService;
        }

        public void EquipCard()
        {
            for (int i = 0; i < chosenInThatRound.Count; i++)
            {
                _oppositeCroco
                    .GetCrocodileInfo()
                    .EquipingHandler
                    .EquipWeapon(chosenInThatRound[i], chosenInThatRoundCardObjects[i], _fasteners[i], chosenInThatRoundCardObjects[i].GetCardInfo().GetLevel());
                chosenInThatRoundCardObjects[i].Setup( chosenInThatRound[i]);
                if(!_gameInfoService.IsEnemyCardsOpenByAds)
                    _handler.SetCardUsedGraphic((WeaponType)chosenInThatRound[i].GetCardType());
            }
        }
        
        private int AvailableCardsCountInRound()
        {
            Deck currentDeck = EnemyGenerator.GetDeck();
            return currentDeck.GetCardsAmount(Random.Range(0f, 1f));
        }

        public void ChooseCards()
        {
            bool isTutorComplete = _dataService.GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END);
            int availableCards = 0;
            availableCards = isTutorComplete ? AvailableCardsCountInRound() : 1;
            if (availableCards > _handler.GetChoosedCards().Count)
                availableCards = 1;
            bool isAdsShown = _gameInfoService.IsEnemyCardsOpenByAds;
            for (int i = 0; i < availableCards; i++)
            {
                while (true)
                {
                    var card = GenerateCard(isTutorComplete);
                    if (!((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().IsNeedFastener)
                    {
                        var check = CheckOneActionWeapon((WeaponType)card.GetCardType());
                        if(check)
                        {
                            Choose(null, card, _handler.GetEnemyCards(), i, isAdsShown);
                            if (isTutorComplete)
                            {
                                _handler.GetChoosedCards().Remove(card);
                                _handler.GetDiscardedCards().Add(card);
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (SetWeaponWithFastener(card, _handler.GetEnemyCards(), i, isAdsShown))
                            break;
                        if (isTutorComplete)
                        {
                            _handler.GetChoosedCards().Remove(card);
                            _handler.GetDiscardedCards().Add(card);
                        }
                        break;
                    }
                }
            }
        }

        private ICardInfo GenerateCard(bool isTutorComplete)
        {
            if (_handler.GetChoosedCards().Count == 0)
            {
                _handler.GetChoosedCards().AddRange(_handler.GetDiscardedCards());
                _handler.GetDiscardedCards().Clear();
            }
            ICardInfo cardInfo = _handler.GetChoosedCards().Random();

            return isTutorComplete ? cardInfo : _handler.GetChoosedCards()[_usingCardIndex];
        }
        
        private bool SetWeaponWithFastener(ICardInfo card, List<EnemyCard> enemyCards, int i, bool isAdsShown)
        {
            var freeFastener = GetFreeFastener(card);

            if (((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().ActionType ==
                WeaponActionType.DEFENCE)
            {
                if (freeFastener != null)
                {
                    var check = CheckAlreadyEquippedDefenceWeapons((WeaponType)card.GetCardType());
                    var check1 = DefenceWeapons((WeaponType)card.GetCardType());

                    if (!check || !check1) return true;
                    Choose(freeFastener, card, enemyCards, i, isAdsShown);
                    return false;
                }
            }
            else
            {
                freeFastener = GetFreeFastener(card);
                if (freeFastener != null)
                {
                    Choose(freeFastener, card, enemyCards, i, isAdsShown);
                    return false;
                }
            }

            return false;
        }

        private Fastener GetFreeFastener(ICardInfo card)
        {
            List<Fastener> freeFasteners = _oppositeCroco
                .GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .FindAll(fastener => fastener.IsFreeFastener() && !fastener.BotWillUseThisFastener);
            Fastener freeFastener = GetFastener(freeFasteners,
                ((WeaponType)card.GetCardType())
                    .GetAttribute<WeaponData>().ActionType);
            return freeFastener;
        }

        private void Choose(Fastener freeFastener, ICardInfo card, List<EnemyCard> enemyCards, int i, bool isAdsShown)
        {
            _fasteners.Add(freeFastener);
            if (freeFastener != null)
                freeFastener.BotWillUseThisFastener = true;
            chosenInThatRound.Add(card);
            EnemyCard enemyCard = enemyCards.First(local => !local.CardWillUse);
            enemyCard.Setup( card);
            chosenInThatRoundCardObjects.Add(enemyCard.GetCard());
            if (isAdsShown)
                _handler.SetCardUsedGraphic((WeaponType)card.GetCardType());
        }

        private void ShowChosenCards() => 
            chosenInThatRound.ForEach(local => _handler.SetCardUsedGraphic((WeaponType)local.GetCardType()));

        public void HideOpenedCards() => _handler.GetEnemyCards().ForEach(local => local.Hide());

        private Fastener GetFastener(List<Fastener> freeFasteners, WeaponActionType actionType) => 
            freeFasteners.Find(fastener => fastener.GetActionType() == actionType);

        private bool CheckOneActionWeapon(WeaponType weaponType) => 
            chosenInThatRound.Find(local => (WeaponType)local.GetCardType() == weaponType) == null;

        private bool DefenceWeapons(WeaponType weaponType) => 
            chosenInThatRound.Find(local => (WeaponType)local.GetCardType() == weaponType) == null;

        private bool CheckAlreadyEquippedDefenceWeapons(WeaponType weaponType) =>
            _oppositeCroco
                .GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .FindAll(localFast => localFast.GetActionType() == WeaponActionType.DEFENCE)
                .Find(local => local.Card != null && (local != null && (WeaponType)local.Card?.GetCardType() == weaponType)) == null;
    }
}