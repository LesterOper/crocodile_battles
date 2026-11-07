using System;
using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.Chests;
using App.Counters;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library.Collections;
using Library.Utils;
using UnityEngine;
using Random = System.Random;

namespace App.Crocodile
{
    public static class EnemyGenerator
    {
        private const string ENEMY_BATH = "EnemyBath";
        private static List<BattleCardInfo> _battleCards = new List<BattleCardInfo>();
        
        private static IDataService _dataService;
        private static IGameInfoService _gameInfoService;
        private static bool exist = false;

        public static BathType GetEnemyBathType(IDataService dataService) => (BathType)dataService.GetLong(ENEMY_BATH);
        private static DeckSetup _deckSetup;
        private static Deck _deck;
        public static Deck GetDeck() => _deck;
        public static DeckSetup GetDeckSetup() => _deckSetup;
        private static void Validate()
        {
            if(exist) return;
            
            _dataService = AllServices.Container.Single<IDataService>();
            exist = true;
        }
        
        public static void SetBathType(IDataService dataService, BathType bathType)
        {
            //Validate();
            
            if (!dataService.GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
            {
                dataService.SetLong(ENEMY_BATH, (int)BathType.BATH_TYPE_3);
                return;
            }

            dataService.SetLong(ENEMY_BATH, (int)bathType);
        }

        public static void SetBathLevel(IDataService dataService, int playerLevelBath, int mod)
        {
            
            var level = playerLevelBath + mod;
            if (level <= 0) level = 1;
            if (level >= 9) level = 9;
            
            dataService.SetLong(ENEMY_BATH + "_level", level);
        }

        public static int GetBathLevel(IDataService dataService, IGameInfoService gameInfoService)
        {
            
            if (gameInfoService.IsBossFight())
            {
                BossWeaponBuild build = new BossWeaponBuild();
                return build.BossBathLevel(GetEnemyBathType(dataService));
            }
            else if (!dataService
                .GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
            {
                return 6;
            }
            return (int)dataService.GetLong(ENEMY_BATH +"_level");
        }

        public static List<BattleCardInfo> GetEnemiesCards(CounterUtility counterUtility, IGameInfoService gameInfoService, IDataService dataService)
        {
            SetEnemyCards(counterUtility, dataService, gameInfoService.IsBossFight());
            return _battleCards;
        }

        private static int GetAverageLevelOfPlayerCards(IDataService dataService)
        {
            List<FilterType> fasteners = ((BathType) dataService.GetLong(ENEMY_BATH)).GetAttribute<BathAttribute>().Fasteners;
            List<BattleCardInfo> cards = new List<BattleCardInfo>(dataService.GetBattleCardInfoData().CardInfos.FindAll(card =>
                fasteners.Contains(card.WeaponType.GetAttribute<WeaponData>().FilterType) ||
                !card.WeaponType.GetAttribute<WeaponData>().IsNeedFastener));
            return (int)Math.Round(cards.Sum(local => local.Level) / (double)cards.Count, MidpointRounding.AwayFromZero);
        }
        
        private static void SetEnemyCards(CounterUtility counterUtility, IDataService dataService, bool isBoss)
        {
            _deckSetup = Resources.Load<DeckSetup>("deck_setup");
            if (!isBoss)
                _deck = _deckSetup.GetDeck((int) counterUtility.GetMoney(CounterType.BATTLES_WIN));
            else
            {
                _battleCards.Clear();
                BathType type = (BathType) (dataService.GetLong(ENEMY_BATH));
                _deck = _deckSetup.GetBossDeck(type);
            }
                
            int weapLevel = 0;
            foreach (var card in _deck.GetCards)
            {
                weapLevel = _deck.GetCardLevel();
                if (weapLevel <= 0) weapLevel = 1;
                if (weapLevel > 10) weapLevel = 10;
                if (card.GetWeaponType.GetAttribute<WeaponData>().FilterType == FilterType.ONE_ACTION)
                {
                    _battleCards.Add(new BattleCardInfo()
                    {
                        Level = weapLevel, WeaponType = card.GetWeaponType,
                        type = card.GetWeaponType.GetAttribute<WeaponData>().ActionType
                    });
                }
                else
                {
                    _battleCards.Add(new BattleCardInfo(){Level = weapLevel, 
                        type = card.GetWeaponType.GetAttribute<WeaponData>().ActionType, 
                        WeaponType = card.GetWeaponType});
                }
            }
        }
        private static void GetBossWeapons(IDataService dataService)
        {

            _battleCards.Clear();
            BathType type = (BathType) (dataService.GetLong(ENEMY_BATH));
            BossWeaponBuild bossBuild = new BossWeaponBuild();
            Boss boss = bossBuild.GetBoss(type);
            BossWeaponGroup weaponGroup;
            int count = dataService.GetBattleCardInfoData().CardInfos.Count;
            for (int i = 0; i < count * 2; i++)
            {
                weaponGroup = bossBuild.GetWeaponGroup(boss.RandomizeGroup().GroupType);
                var weap = weaponGroup.CardsGroup.Random();
                var level = boss.RandomizeLevel();
                if (level <= 0) level = 1;                     
                if (level > 10) level = 10;                
                BattleCardInfo card = new BattleCardInfo(){Level = level, WeaponType = weap, type = weap.GetAttribute<WeaponData>().ActionType}; 
                _battleCards.Add(card);
            }
            bossBuild.Dispose();
        }
    }
}