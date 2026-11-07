using System;
using System.Collections.Generic;
using System.Linq;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using UnityEngine;
using Random = UnityEngine.Random;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "WeightsSettings", menuName = "Settings/Weights")]
    public class WeightsRandomBathSettings : ScriptableObject
    {
        [Header("Level by weights")]
        [SerializeField] private List<WeightSetting> weights = new List<WeightSetting>();

        [Space(3)] [Header("Weapons Group Handler")] 
        [SerializeField] private WeaponGroupsRandomizer _weaponsGroupsRandomizer;

        [Space(3)] [Header("Weapons Group")] 
        [SerializeField] private List<WeaponsGroup> _groups;

        public Modificator GetModificator(CounterUtility counterUtility)
        {
            WeightSetting setting = weights.FindLast(local =>
                counterUtility.GetMoney(CounterType.BATTLES_WIN) >= local.battlesCount);
        
            return new Modificator(setting.RollBathTypeModificator(), setting.RollLevelModificator());
        }

        public Modificator GetWeaponsModificator(CounterUtility counterUtility)
        {
            WeightSetting setting = weights.FindLast(local =>
                counterUtility.GetMoney(CounterType.BATTLES_WIN) >= local.battlesCount);
            
            return new Modificator(setting.RollWeaponLevelModificator(), setting.battlesCount);
        }

        public WeaponsGroup GetWeaponGroup()
        {
            ChestWeaponGroupType groupType = _weaponsGroupsRandomizer.RollGroup();
            return _groups.Find(local => local.GroupType == groupType);
        }


        public class Modificator
        {
            public int BathTypeMod;
            public int LevelMod;
            public int WeaponLevelMod;

            public Modificator(int weaponLevelMod)
            {
                WeaponLevelMod = weaponLevelMod;
            }
            
            public Modificator(int bathTypeMod, int levelMod)
            {
                BathTypeMod = bathTypeMod;
                LevelMod = levelMod;
            }
        }

        [Serializable]
        public class WeightSetting
        {
            public int battlesCount;
            public List<Weight> weight;
            public List<Weight> levelWeight;
            public List<Weight> weaponLevelWeight;

            public int RollBathTypeModificator()
            {
                float sum = weight.Sum(typeWeight => typeWeight.weightAmount);
                float rand =Random.Range(0, sum);
                int index = Count(rand, weight);
                return weight[index].mod;
            }

            private int Count(float rand, List<Weight> list)
            {
                int sumBuf = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    sumBuf += list[i].weightAmount;
                    if (rand < sumBuf)
                        return i;
                }

                return 0;
            }

            public int RollLevelModificator()
            {
                int sum = levelWeight.Sum(local => local.weightAmount);
                float rand = Random.Range(0, sum);
                int index = Count(rand, levelWeight);
                return levelWeight[index].mod;
            }

            public int RollWeaponLevelModificator()
            {
                int sum = weaponLevelWeight.Sum(local => local.weightAmount);
                float rand = Random.Range(0, sum);
                int index = Count(rand, weaponLevelWeight);
                return weaponLevelWeight[index].mod;
            }
        }

        [Serializable]
        public class Weight
        {
            public int mod;
            public int weightAmount;
        }

        [Serializable]
        public class WeaponsGroup
        {
            [SerializeField] private ChestWeaponGroupType _groupType;
            [SerializeField] private List<WeaponType> _weapons;

            public ChestWeaponGroupType GroupType => _groupType;

            public List<WeaponType> Weapons => _weapons;
        }

        [Serializable]
        public class WeaponsGroupWeightHandler
        {
            [SerializeField] private ChestWeaponGroupType _groupType;
            [SerializeField] private int chance;
            
            public ChestWeaponGroupType GroupType => _groupType;
            public int Chance => chance;
        }

        [Serializable]
        public class WeaponGroupsRandomizer
        {
            [SerializeField] private List<WeaponsGroupWeightHandler> _weaponsGroupWeightHandlers;

            public ChestWeaponGroupType RollGroup()
            {
                float sum = _weaponsGroupWeightHandlers.Sum(local => local.Chance);
                float rand = Random.Range(0, sum);
                return _weaponsGroupWeightHandlers[GetIndexByWeights(rand)].GroupType;
            }

            private int GetIndexByWeights(float rand)
            {
                long sum = 0;
                for (int i = 0; i < _weaponsGroupWeightHandlers.Count; i++)
                {
                    sum += _weaponsGroupWeightHandlers[i].Chance;
                    if (rand < sum)
                        return i;
                }

                return 0;
            }
        }
    }
}