using System;
using App.Dialogs.Merge;
using App.SetupsScript;
using UnityEngine;

namespace App.PrototypeBattle
{
    [Serializable]
    public class WeaponView 
    {
        public WeaponType Type;
        public GameObject WeaponObject;
        public Sprite WeaponSprite;
        public Sprite FastenerSprite;
        public int Mana;
        public int RoundDuration;
        public Sprite GetSprite() => WeaponSprite;
    }

    public enum WeaponType
    {
        [WeaponData()]
        NONE = 0,

        #region CloseBattleWeapon
    
        [WeaponData(WeaponActionType.ATTACK_CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE,true, 25, powerGroup:1, manaCost:2, roundsDuration:2)] 
        BRUSH = 1,
        [WeaponData(WeaponActionType.ATTACK_CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE,true, 9, powerGroup:1, manaCost:3, roundsDuration:2)]
        PADDLE = 2,

        #endregion

        #region DistanceWeapon
    
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 18, powerGroup:2,manaCost:1, roundsDuration:1)] 
        BLASTER = 3,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 15, powerGroup:3,manaCost:2, roundsDuration:2)]
        WATER_FOUNTAIN = 4,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 30, powerGroup:4,manaCost:4, roundsDuration:3)] 
        RIFLE = 5,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 13, powerGroup:1,manaCost:2, roundsDuration:1)]
        BOW = 6,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 19, powerGroup:1,manaCost:4, roundsDuration:2)] 
        BOTTLE_MACHINE_GUN = 7,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 22, powerGroup:4,manaCost:3, roundsDuration:3)] 
        BOTTLE_GUN = 8,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 27, powerGroup:5,manaCost:1, roundsDuration:1)] 
        CATAPULT = 9,
        [WeaponData(WeaponActionType.ATTACK_LONG_DISTANCE, FilterType.LONG_DISTANCE,true, 20,manaCost:1, roundsDuration:2)] 
        TEST_TUBE_GUN = 10,
    
    
        #endregion

        #region Defence
    
        [WeaponData(WeaponActionType.ATTACK_CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE,true, 20,manaCost:1, roundsDuration:3)] 
        MOP = 11,
        [WeaponData(WeaponActionType.DEFENCE, FilterType.DEFENCE,true, 0, 0,manaCost:1, isArmor:true, baseArmorPoint: 50)]
        SNAKE_TUBE = 12,
        [WeaponData(WeaponActionType.DEFENCE, FilterType.DEFENCE,true, 0, 0,manaCost:1, isArmor:true, baseArmorPoint: 50)] 
        UMBRELLA = 13,
    
        #endregion
        

        #region Healers

        [WeaponData(WeaponActionType.DEFENCE, FilterType.ONE_ACTION,true, 0, 10,manaCost:2, roundsDuration:1)]
        BOBBER = 16,
        [WeaponData(WeaponActionType.DEFENCE, FilterType.ONE_ACTION,true, 0, 13,manaCost:3, roundsDuration:1)]
        LIFEBUOY = 17,
        [WeaponData(WeaponActionType.DEFENCE, FilterType.ONE_ACTION,true, 0, 12,manaCost:4, roundsDuration:1)]
        FLOAT_BELT = 18,
        [WeaponData(WeaponActionType.ONE_ACTION_HEAL, FilterType.ONE_ACTION,false, 0, 0, 31,manaCost:2, roundsDuration:1)]
        TUBE = 19,

        #endregion

        
        [WeaponData(WeaponActionType.ONE_ACTION_BOMB, FilterType.ONE_ACTION, false,11, 0, 0,0,10,manaCost:2, roundsDuration:1)]
        BOMB_ILLNESS =21,
        [WeaponData(WeaponActionType.ATTACK_CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, true,30, 0,manaCost:2, roundsDuration:2)]
        SHOWER = 22,
        [WeaponData(WeaponActionType.ONE_ACTION_BOMB, FilterType.ONE_ACTION, false,45, 0,manaCost:1, roundsDuration:1)]
        MINA_BOMB = 23,
    }

    public class WeaponData : Attribute
    {
        public FilterType FilterType { get; }

        public WeaponActionType ActionType { get; }

        public bool IsNeedFastener { get; }

        public long BaseAttackProperty { get; }

        public long BaseHealthPointProperty { get; }

        public long HealProperty { get; }

        public long AccuracyProperty { get; }

        public long PassiveDamageProperty { get; }
    
        public long PowerGroup { get; }

        public int ManaCost { get; }

        public int RoundsDuration { get; }
        public bool IsArmor { get; }
        public long ArmorProperty { get; }

        public WeaponData(WeaponActionType actionType = WeaponActionType.NONE, FilterType filterType = FilterType.NONE, bool isNeedFastener = false, 
            long baseAttack = 0, long baseHealthPoint = 0, 
            long heal = 0, long accuracy = 0, 
            long passiveDamage = 0, long powerGroup = 1, int manaCost = 0, int roundsDuration = 0, bool isArmor = false, long baseArmorPoint = 0)
        {
            ActionType = actionType;
            this.IsNeedFastener = isNeedFastener;
            BaseAttackProperty = baseAttack;
            BaseHealthPointProperty = baseHealthPoint;
            HealProperty = heal;
            AccuracyProperty = accuracy;
            PassiveDamageProperty = passiveDamage;
            FilterType = filterType;
            PowerGroup = powerGroup;
            ManaCost = manaCost;
            RoundsDuration = roundsDuration;
            IsArmor = isArmor;
            ArmorProperty = baseArmorPoint;
        }
    }
}