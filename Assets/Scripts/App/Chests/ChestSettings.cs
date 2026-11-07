using System;
using System.Collections.Generic;
using System.Linq;
using App.Chests;
using App.PrototypeBattle;
using App.SetupsScript;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ChestSettings", menuName = "Settings/Chests")]
public class ChestSettings : ScriptableObject
{
    [SerializeField] private List<ChestLootGroup> groups;
    [SerializeField] private ChestSetting chests;

    public Chest GetSetting(ChestType chestType) => chests.GetChestSetting(chestType);

    public ChestLootGroup GetChestLootGroup(ChestWeaponGroupType weaponGroupType) =>
        groups.Find(local => local.ChestWeaponGroupType == weaponGroupType);
}

[Serializable]
public class ChestLootGroup
{
    [SerializeField] private ChestWeaponGroupType _chestWeaponGroupType;
    [SerializeField] private List<WeaponType> weaponsGroup;
    [SerializeField] private List<BathType> bathsGroup;
    
    public List<WeaponType> GetGroup() => weaponsGroup;
    public List<BathType> GetBathsGroup() => bathsGroup;
    public ChestWeaponGroupType ChestWeaponGroupType => _chestWeaponGroupType;

}

[Serializable]
public class Chest
{
    [SerializeField] private ChestType type;
    [SerializeField] private List<ChestGroup> _chestGroups;
    [SerializeField] private List<LevelMod> _levelMods;
    public ChestType Type => type;
    public List<ChestGroup> ChestGroups => _chestGroups;

    public ChestGroup RandomizeGroup()
    {
        float sum = _chestGroups.Sum(local => local.Chance);
        float rand = Random.Range(0, sum);
        int index = Count(rand, _chestGroups);
        return _chestGroups[index];
    }
    
    private int Count(float rand, List<ChestGroup> list)
    {
        long sumBuf = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sumBuf += list[i].Chance;
            if (rand < sumBuf)
                return i;
        }
        return 0;
    }

    private int CountLevel(float rand, List<LevelMod> list)
    {
        long sumBuf = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sumBuf += list[i].Chance;
            if (rand < sumBuf)
                return i;
        }
        return 0;
    }
    
    public int RandomizeLevel()
    {
        float sum = _levelMods.Sum(local => local.Chance);
        float rand = Random.Range(0, sum);
        int index = CountLevel(rand, _levelMods);
        return (int)_levelMods[index].Level;
    }
}

[Serializable]
public class ChestGroup
{
    [SerializeField] private ChestWeaponGroupType type;
    [SerializeField] private long chance;
    public long Chance => chance;

    public ChestWeaponGroupType Type => type;
}

[Serializable]
public class LevelMod
{
    [SerializeField] private int level;
    [SerializeField] private long chance;
    public long Chance => chance;
    public long Level => level;
}

[Serializable]
public class ChestSetting
{
    [SerializeField]private List<Chest> chests;

    public Chest GetChestSetting(ChestType chestType) => chests.Find(local => local.Type == chestType);
}

public enum ChestWeaponGroupType
{
    GROUP_A = 0,
    GROUP_B = 1,
    GROUP_C = 2,
    GROUP_D = 3,
    GROUP_E = 4,
}
