using System;
using System.Collections.Generic;
using App.Achievements;
using App.Chests;
using App.Counters;
using App.PrototypeBattle;
using App.SetupsScript;
using Library.Collections;
using UnityEngine;

namespace App.Dialogs.BattlePass
{
    [CreateAssetMenu(fileName = "BattlePassPrizeContainer", menuName = "Setups/BattlePassPrizeContainer")]
    public class BattlePassPrizeContainer : ScriptableObject
    {
        [SerializeField] private List<PrizeItems> prizeItems;

        public int Count => prizeItems.Count;
        public List<PrizeItems> GetPrizeItemList() => prizeItems;
    }

    [Serializable]
    public class PrizeItem
    {
        [SerializeField] private BattlePassPrizeType prizeType;
        [SerializeField] private Sprite prizeIcon;
        [SerializeField] private MoneyPrizeItem moneyPrizeItem;
        [SerializeField] private ChestPrizeItem chestPrizeItem;
        [SerializeField] private BathCardsPrizeItem bathCardsPrizeItem;
        [SerializeField] private WeaponsCardsPrizeItem weaponsCardsPrizeItem;
        [SerializeField] private BoosterPrizeItem boosterPrizeItem;
        public BattlePassPrizeType PrizeTypeProperty => prizeType;
        public Sprite PrizeIconProperty => prizeIcon;

        public IBattlePassPrizeItem MoneyPrizeItemProperty => moneyPrizeItem;

        public IBattlePassPrizeItem ChestPrizeItemProperty => chestPrizeItem;
        public IBattlePassPrizeItem BoosterPrizeItemProperty => boosterPrizeItem;
        public IBattlePassPrizeItem BathCardsPrizeItemProperty => bathCardsPrizeItem;

        public IBattlePassPrizeItem WeaponsCardsPrimeItemProperty => weaponsCardsPrizeItem;

        public int GetAmount()
        {
            switch (prizeType)
            {
                case BattlePassPrizeType.СHEST : return chestPrizeItem.AmountProperty;
                case BattlePassPrizeType.PREM_MONEY : return MoneyPrizeItemProperty.AmountProperty;
                case BattlePassPrizeType.BATH_CARDS : return BathCardsPrizeItemProperty.AmountProperty;
                case BattlePassPrizeType.WEAPON_CARDS : return WeaponsCardsPrimeItemProperty.AmountProperty;
                case BattlePassPrizeType.DEFAULT_MONEY : return MoneyPrizeItemProperty.AmountProperty;
                case BattlePassPrizeType.BOOSTER: return BoosterPrizeItemProperty.AmountProperty;
                default: return 0;
            }
        }
    }

    [Serializable]
    public class PrizeItems
    {
        [SerializeField] private bool isBoss;
        [SerializeField] private BathType bossBathType;
        [SerializeField] private bool needDefaultPrize;
        [SerializeField] private int startAmountBattles;
        [SerializeField] private int needBattles;
        [SerializeField] private PrizeItem defaultPrize;
        [SerializeField] private PrizeItem vipPrize;

        public PrizeItem DefaultPrize => defaultPrize;

        public PrizeItem VipPrize => vipPrize;

        public bool IsBoss => isBoss;

        public int NeedBattles => needBattles;

        public bool NeedDefaultPrize => needDefaultPrize;

        public int StartAmountBattles => startAmountBattles;

        public BathType BossBathType => bossBathType;
    }

    [Serializable]
    public class BoosterPrizeItem : IBattlePassPrizeItem
    {
        [SerializeField] private BoosterType boosterType;
        [SerializeField] private int amount;
        public BoosterType BoosterTypeProperty => boosterType;
        public int AmountProperty => amount;
        
    }
    [Serializable]
    public class ChestPrizeItem : IBattlePassPrizeItem
    {
        [SerializeField] private ChestType chestType;
        [SerializeField] private int amount;

        public ChestType ChestTypeProperty => chestType;

        public int AmountProperty => amount;
    }
    
    [Serializable]
    public class MoneyPrizeItem : IBattlePassPrizeItem
    {
        [SerializeField] private CounterType counterType;
        [SerializeField] private int amount;

        public CounterType CounterTypeProperty => counterType;

        public int AmountProperty => amount;
    }

    [Serializable]
    public class WeaponsCardsPrizeItem : IBattlePassPrizeItem
    {
        [SerializeField] private List<WeaponType> weaponTypes;
        [SerializeField] private int amount;

        public List<WeaponType> WeaponTypesProperty => weaponTypes;

        public int AmountProperty => amount;

        public WeaponType Roll() => weaponTypes.Random();
    }
    
    [Serializable]
    public class BathCardsPrizeItem : IBattlePassPrizeItem
    {
        [SerializeField] private List<BathType> bathTypes;
        [SerializeField] private int amount;

        public List<BathType> BathTypesProperty => bathTypes;

        public int AmountProperty => amount;

        public BathType Roll() => bathTypes.Random();
    }

    public interface IBattlePassPrizeItem
    {
        int AmountProperty { get; }
    }
}