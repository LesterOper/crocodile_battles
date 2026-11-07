using System;
using System.Collections.Generic;
using System.Linq;
using App.Dialogs.Merge;
using UnityEngine;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "bath_setup", menuName = "Setup/Bathes")]
    public class BathSetup : ScriptableObject
    {
        [SerializeField] private List<BathView> _bathContainer;
        public BathView GetBathByType(BathType bathType) => _bathContainer.FirstOrDefault(obj => obj.BathType == bathType);
    }

    public enum BathType
    {
        [Bath()]NONE = 0,
        [Bath(150, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_1 = 1,
        [Bath(135, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_2 = 2,
        [Bath(135, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_3 = 3,
        [Bath(120, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_4 = 4,
        [Bath(120, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_5 = 5,
        [Bath(120, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_6 = 6,
        [Bath(105, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_7 = 7,
        [Bath(105, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_8 = 8,
        [Bath(100, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_9 = 9,
        [Bath(60, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, 
            FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.LONG_DISTANCE, 
            FilterType.DEFENCE, FilterType.DEFENCE, FilterType.DEFENCE)]BATH_TYPE_10 = 10,
        [Bath(60, FilterType.DEFENCE, FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE, FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_1 = 11,
        [Bath(105, FilterType.CLOSE_DISTANCE, FilterType.LONG_DISTANCE, FilterType.DEFENCE,FilterType.DEFENCE, FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_2 = 12,
        [Bath(120, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE, FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_3 = 13,
        [Bath(135, FilterType.DEFENCE, FilterType.CLOSE_DISTANCE, FilterType.LONG_DISTANCE, FilterType.LONG_DISTANCE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_4 = 14,
        [Bath(135, FilterType.DEFENCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_5 = 15,
        [Bath(150, FilterType.DEFENCE, FilterType.CLOSE_DISTANCE, FilterType.CLOSE_DISTANCE, FilterType.LONG_DISTANCE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE,FilterType.NONE)]BOSS_6 = 16,
    }

    public class BathAttribute : Attribute
    {
        private long _bathHP;
        private List<FilterType> _fasteners = new List<FilterType>();
        public BathAttribute(){}

        public BathAttribute(long HP, FilterType type, FilterType type1, FilterType type2, FilterType type3, FilterType type4, FilterType type5, FilterType type6, FilterType type7, FilterType type8)
        {
            _bathHP = HP;
            if(type != FilterType.NONE)
                _fasteners.Add(type);
            if(type1 != FilterType.NONE)
                _fasteners.Add(type1);
            if(type2 != FilterType.NONE)
                _fasteners.Add(type2);
            if(type3 != FilterType.NONE)
                _fasteners.Add(type3);
            if(type4 != FilterType.NONE)
                _fasteners.Add(type4);
            if(type5 != FilterType.NONE)
                _fasteners.Add(type5);
            if(type6 != FilterType.NONE)
                _fasteners.Add(type6);
            if(type7 != FilterType.NONE)
                _fasteners.Add(type7);
            if(type8 != FilterType.NONE)
                _fasteners.Add(type8);
        }

        public long HP => _bathHP;

        public List<FilterType> Fasteners => _fasteners;
    }
}