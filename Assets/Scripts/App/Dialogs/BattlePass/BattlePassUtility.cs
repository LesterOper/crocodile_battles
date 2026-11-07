using System.Collections.Generic;
using App.Analytic;
using App.Cards;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassUtility
    {
        private const string KEY = "BattlePassPrize_";
        private const string KEY_PREM = "BattlePassPrizePrem_";
        private const string KEY_PROGRESS = "BattlePassStepProgress_";
        private const string KEY_CURRENT_STEP = "BattlePassCurrentStepProgress";
        private const string KEY_STEP_COMPLETE = "BattlePassStepComplete";
        private readonly IDataService _dataService;
        private readonly CounterUtility _counterUtility;
        private readonly SignalBus _signalBus;
        public bool IsBattlePassBought => _isBattlePassBought;
        private bool _isBattlePassBought;
        

        [Inject]
        public BattlePassUtility(IDataService dataService, CounterUtility counterUtility, SignalBus signalBus)
        {
            _dataService = dataService;
            _counterUtility = counterUtility;
            _signalBus = signalBus;
        }

        public void ActivateBattlePass() => _isBattlePassBought = true;

        public void ApplyPrize(PrizeItem prizeItems, int prizeIndex, bool prem, IGameInfoService infoService,
            SignalBus signalBus, IAnalytics analytics)
        {
            if (PlayerPrefs.GetInt("PRIZE_COLLECT_FOR_ANALYTICS" + prizeIndex, 0) != 1)
            {
                analytics.SendEvent(AmplitudeEvent.battle_pass_step_collect, new Dictionary<string, object>(){{"type", "usual"},{"lvl", prizeIndex}});
                PlayerPrefs.SetInt("PRIZE_COLLECT_FOR_ANALYTICS"+prizeIndex, 1);
            }
            ApplyExactPrize( prizeItems, prizeIndex, prem, infoService, signalBus);
        } 
            

        public void SetPrizeClaimed(int index, bool isClaimed = true)
        {
            _dataService.SetLong(KEY + index, isClaimed ? 1 : 0);
        }

        public void SetPremPrizeClaimed(int index, bool isClaimed = true) => 
            _dataService.SetLong(KEY_PREM + index, isClaimed ? 1 : 0);

        public bool IsPrizeClaimed(int index)
        {
            return _dataService.GetLong(KEY + index) == 1;
        }
    
        public bool IsPremPrizeClaimed(int index)
        {
            return _dataService.GetLong(KEY_PREM + index) == 1;
        }

        public void AppendBattlePassStepProgress(int index)
        {
            _dataService.SetLong(KEY_PROGRESS + index, _dataService.GetLong(KEY_PROGRESS + index) + 1);
        }

        public long GetBattlePassStepProgress(int index)
        {
            return _dataService.GetLong(KEY_PROGRESS + index);
        }

        public void SetBattlePassCurrentStep(int index) => _dataService.SetLong(KEY_CURRENT_STEP, index);

        public long GetBattlePassCurrentStep() => _dataService.GetLong(KEY_CURRENT_STEP);

        public void SetBattlePassStepComplete(int index) => _dataService.SetLong(KEY_STEP_COMPLETE + index, 1);

        public bool GetBattlePassStepComplete(int index) => _dataService.GetLong(KEY_STEP_COMPLETE + index) == 1;
    
        private void ApplyExactPrize(PrizeItem prizeItem, int prizeIndex, bool prem, IGameInfoService infoService, SignalBus signalBus)
        {
            switch (prizeItem.PrizeTypeProperty)
            {
                case BattlePassPrizeType.DEFAULT_MONEY : ApplyMoneyPrize((MoneyPrizeItem)prizeItem.MoneyPrizeItemProperty); break;
                case BattlePassPrizeType.PREM_MONEY : ApplyMoneyPrize((MoneyPrizeItem)prizeItem.MoneyPrizeItemProperty); break;
                case BattlePassPrizeType.СHEST : ApplyChestPrize((ChestPrizeItem)prizeItem.ChestPrizeItemProperty); break;
                case BattlePassPrizeType.BATH_CARDS : ApplyBathCardsPrize((BathCardsPrizeItem)prizeItem.BathCardsPrizeItemProperty, infoService, signalBus); break;
                case BattlePassPrizeType.WEAPON_CARDS : ApplyWeaponCardsPrize((WeaponsCardsPrizeItem)prizeItem.WeaponsCardsPrimeItemProperty, infoService, signalBus); break;
                case BattlePassPrizeType.BOOSTER : ApplyBoosterPrize((BoosterPrizeItem)prizeItem.BoosterPrizeItemProperty); break;
            }
            if(prem) SetPremPrizeClaimed(prizeIndex);
            else SetPrizeClaimed(prizeIndex);
        }

        private void ApplyChestPrize(ChestPrizeItem prizeItem)
        {
            _dataService.GetChestCollectionData().ChestCollection.Find(local => local.type == prizeItem.ChestTypeProperty)
                .chestAmount += prizeItem.AmountProperty;
            _dataService.UpdateChestCollectionData();
            _signalBus.Fire<UpdateChestSlotsSignal>();
        }

        private void ApplyBoosterPrize(BoosterPrizeItem prizeItem)
        {
            _dataService.GetBoosterCollectionData().BoosterCollection
                .Find(local => local.BoosterType == prizeItem.BoosterTypeProperty)
                .BoosterAmount += prizeItem.AmountProperty;
            _dataService.UpdateBoosterCollectionData();
        }

        private void ApplyMoneyPrize(MoneyPrizeItem prizeItem)
        {
            _counterUtility.AppendMoney(prizeItem.CounterTypeProperty, prizeItem.AmountProperty);
        }

        private void ApplyWeaponCardsPrize(WeaponsCardsPrizeItem prizeItem, IGameInfoService infoService, SignalBus signalBus)
        {
            var cards = new List<ICardInfo>();
            int amount = prizeItem.AmountProperty;
            for (int i = 0; i < amount; i++)
            {
                WeaponType weaponType = prizeItem.Roll();
                CardInfo card = new CardInfo()
                {
                    Level = 1, WeaponType = weaponType, type = weaponType.GetAttribute<WeaponData>().ActionType
                };
                cards.Add(card);
                _dataService.GetCardCollectionData().Collection.Add(card);
            }
            infoService.SetNewCardsInfo(signalBus, new List<ICardInfo>(cards));
            _dataService.UpdateCardCollectionData();
        }

        private void ApplyBathCardsPrize(BathCardsPrizeItem prizeItem, IGameInfoService gameInfoService, SignalBus signalBus)
        {
            var cards = new List<BathInCollectionData>();
            int amount = prizeItem.AmountProperty;
            for (int i = 0; i < amount; i++)
            {
                BathType bathType = prizeItem.Roll();
                BathInCollectionData card = new BathInCollectionData()
                {
                    Level = 1, CrocodileBathType = bathType
                };
                _dataService.GetBathCollection().BathDatas.Add(card);
                cards.Add(card);
            }
            gameInfoService.SetNewBathsCardInfo(signalBus, new List<BathInCollectionData>(cards));
            _dataService.UpdateBathCollection();
        }
    }
}