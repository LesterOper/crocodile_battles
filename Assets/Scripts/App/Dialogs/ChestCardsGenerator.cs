using System.Collections.Generic;
using App.Achievements;
using App.Analytic;
using App.Cards;
using App.Chests;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library;
using Library.Collections;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Dialogs
{
    public class ChestCardsGenerator
    {
        private const string OpenedChestCount = "OPENED_CHEST_COUNT";
        private List<CardInfo> generatedCardInChest = new List<CardInfo>();
        private List<BathInCollectionData> _bathObjects = new List<BathInCollectionData>();
        private ChestHandler _chestHandler;
        private ChestSetup _chestSetup;
        private IDataService _dataService;
        private IGameInfoService _gameInfoService;
        public List<CardInfo> CetGeneratedWeaponsCards() => generatedCardInChest;
        public List<BathInCollectionData> GetGeneratedBathCards() => _bathObjects;
        private SignalBus _signalBus;
        private IAnalytics _analytics;
        public ChestCardsGenerator(ChestHandler chestHandler, ChestSetup chestSetup, SignalBus signalBus, IGameInfoService gameInfoService, IDataService dataService, IAnalytics analytics)
        {
            _chestHandler = chestHandler;
            _chestSetup = chestSetup;
            _signalBus = signalBus;
            _dataService = dataService;
            _gameInfoService = gameInfoService;
            _analytics = analytics;
        }
        public void GenerateCardSet()
        {
            ChestSettings chestSettings = Resources.Load<ChestSettings>("ChestSettings");
            Chest chest = chestSettings.GetSetting(_chestHandler.ChestType);
            ChestGroup group; 
            for (int i = 0; i < _chestSetup.GetChestCharacteristic(_chestHandler.ChestType).trophyCount; i++)
            {
                if (_dataService.GetLong(OpenedChestCount) == 0)
                {
                    CardInfo paddle = new CardInfo()
                    {
                        WeaponType = WeaponType.PADDLE,
                        Level = 1,
                        type = WeaponActionType.ATTACK_CLOSE_DISTANCE
                    };
                    CardInfo bobber = new CardInfo()
                    {
                        WeaponType = WeaponType.BOBBER,
                        Level = 1,
                        type = WeaponActionType.DEFENCE
                    };
                    _signalBus.Fire(new TakeCardSignal(){CardLevel = 1, FromMegre = false, WeaponType = WeaponType.PADDLE, BathType = BathType.NONE});
                    _dataService.GetCardCollectionData().Collection.Add(paddle);
                    _signalBus.Fire(new TakeCardSignal(){CardLevel = 1, FromMegre = false, WeaponType = WeaponType.BOBBER, BathType = BathType.NONE});
                    _dataService.GetCardCollectionData().Collection.Add(bobber);
                    generatedCardInChest.Add(paddle);
                    generatedCardInChest.Add(bobber);
                    if(!_dataService.GetTutorialStepComplete(TutorialStepType.MERGE))
                        CheckForADublicate(null, paddle);
                    i++;
                    continue;
                }
                group = chest.RandomizeGroup();
                int randLevel = chest.RandomizeLevel();
                var rand = Random.Range(0, 10);
                if (rand > 7)
                {
                    BathType bathType = chestSettings.GetChestLootGroup(group.Type).GetBathsGroup().Random();
                    BathInCollectionData bath = new BathInCollectionData()
                        {CrocodileBathType = bathType, Level = randLevel};
                    if(!_dataService.GetTutorialStepComplete(TutorialStepType.MERGE))
                        CheckForADublicate(bath);
                    _signalBus.Fire(new TakeCardSignal(){CardLevel = bath.Level, FromMegre = false, WeaponType = WeaponType.NONE, BathType = bath.CrocodileBathType});
                    _dataService.GetBathCollection().BathDatas.Add(bath);
                    _bathObjects.Add(bath);
                }
                else
                {
                    WeaponType weaponType = chestSettings.GetChestLootGroup(group.Type).GetGroup().Random();
                    CardInfo cardInfo = new CardInfo()
                    {
                        WeaponType = weaponType,
                        Level = randLevel,
                        type = weaponType.GetAttribute<WeaponData>().ActionType
                    };
                    if(!_dataService.GetTutorialStepComplete(TutorialStepType.MERGE))
                        CheckForADublicate(null, cardInfo);
                    _signalBus.Fire(new TakeCardSignal(){CardLevel = cardInfo.Level, FromMegre = false, WeaponType = cardInfo.WeaponType, BathType = BathType.NONE});
                    _dataService.GetCardCollectionData().Collection.Add(cardInfo);
                    generatedCardInChest.Add(cardInfo);
                }
            }
            _gameInfoService.SetNewCardsInfo(_signalBus, new List<ICardInfo>(generatedCardInChest));
            _gameInfoService.SetNewBathsCardInfo(_signalBus, _bathObjects);
            _dataService.SetLong(OpenedChestCount, _dataService.GetLong(OpenedChestCount)+1);
            _dataService.UpdateCardCollectionData();
            _dataService.UpdateBathCollection();
        }
        
        private void CheckForADublicate(BathInCollectionData bathInCollectionData = null, CardInfo cardInfo = null)
        {
            _dataService.SetExtensionUnlock(Extension.MERGE, true);
            if (bathInCollectionData != null)
            {
                List<BathInCollectionData> bathInCollectionDatas = _dataService.GetBathCollection().BathDatas;
                if (bathInCollectionDatas.Exists(bath => bath.CrocodileBathType == bathInCollectionData.CrocodileBathType && bath.Level == bathInCollectionData.Level))
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_merge_start);
                }
                else
                {
                    BathData bathData = _dataService.GetBathData();
                    if (bathData.Equals(bathInCollectionData))
                    {
                        _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                        _analytics.SendEvent(AmplitudeTutorialEvent.tutor_merge_start);
                    }
                }
            
                return;
            }

            if (cardInfo != null)
            {
                List<CardInfo> cards = _dataService.GetCardCollectionData().Collection;

                if (cards.Exists(card => card.WeaponType == cardInfo.WeaponType && card.Level == cardInfo.Level))
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                    _analytics.SendEvent(AmplitudeTutorialEvent.tutor_merge_start);
                }
                else
                {
                    List<BattleCardInfo> battleCards = _dataService.GetBattleCardInfoData().CardInfos;

                    if (battleCards.Exists(card => card.WeaponType == cardInfo.WeaponType && card.Level == cardInfo.Level))
                    {
                        _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.MERGE});
                        _analytics.SendEvent(AmplitudeTutorialEvent.tutor_merge_start);
                    }
                }
                return;
            }
        }
        
        public void GenerateStarterCardsPack()
        {
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.PADDLE, Level = 1, type = WeaponActionType.ATTACK_CLOSE_DISTANCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BOBBER, Level = 1, type = WeaponActionType.DEFENCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BOTTLE_GUN, Level = 1, type = WeaponActionType.ATTACK_LONG_DISTANCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BOW, Level = 1, type = WeaponActionType.ATTACK_LONG_DISTANCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BOMB_ILLNESS, Level = 1, type = WeaponActionType.ONE_ACTION_BOMB});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.FLOAT_BELT, Level = 1, type = WeaponActionType.DEFENCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.SNAKE_TUBE, Level = 1, type = WeaponActionType.DEFENCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BLASTER, Level = 1, type = WeaponActionType.ATTACK_LONG_DISTANCE});
            generatedCardInChest.Add(new CardInfo(){WeaponType = WeaponType.BOTTLE_MACHINE_GUN, Level = 1, type = WeaponActionType.ATTACK_LONG_DISTANCE});

            _dataService.GetCardCollectionData().Collection.AddRange(new List<CardInfo>(generatedCardInChest));
            
            _bathObjects.Add(new BathInCollectionData(){CrocodileBathType = BathType.BATH_TYPE_9, Level = 1});
            _dataService.GetBathCollection().BathDatas.AddRange(_bathObjects);

            _gameInfoService.SetNewCardsInfo(_signalBus, new List<ICardInfo>(generatedCardInChest));
            _gameInfoService.SetNewBathsCardInfo(_signalBus, _bathObjects);
            _dataService.UpdateBathCollection();
            _dataService.UpdateCardCollectionData();
        }
    }
}