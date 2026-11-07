using System;
using System.Collections.Generic;
using System.Linq;
using App.PrototypeBattle;
using App.SetupsScript;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "BossSettings", menuName = "Settings/Bosses")]
public class BossBuildSettings : ScriptableObject
{
    [SerializeField] private List<BossWeaponGroup> groups;
    [SerializeField] private List<Boss> bosses;

    public Boss GetBossWeapons(BathType bathType) => bosses.Find(local => local.BossIndex == bathType);

    public BossWeaponGroup GetWeapons(ChestWeaponGroupType groupType) =>
        groups.Find(local => local.GroupType == groupType);
}

[Serializable]
public class BossWeaponGroup
{
    [SerializeField] private ChestWeaponGroupType groupType;
    [SerializeField] private List<WeaponType> cardsGroup;

    public ChestWeaponGroupType GroupType => groupType;
    public List<WeaponType> CardsGroup => cardsGroup;
}

[Serializable]
public class BossBuild
{
    [SerializeField] private ChestWeaponGroupType groupType;
    [SerializeField] private int chance;
    public ChestWeaponGroupType GroupType => groupType;
    public int Chance => chance;

}

[Serializable]
public class Boss
{
    [SerializeField] private BathType bathType;
    [SerializeField] private int bathLevel;
    [SerializeField] private List<BossBuild> bossBuilds;
    [SerializeField] private List<BossLevelMod> _levelMods;

    public BathType BossIndex => bathType;
    public int BathLevel => bathLevel;
    
    public BossBuild RandomizeGroup()
    {
        float sum = bossBuilds.Sum(local => local.Chance);
        float rand = Random.Range(0, sum);
        int index = Count(rand, bossBuilds);
        return bossBuilds[index];
    } 
    
    private int Count(float rand, List<BossBuild> list)
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
        return _levelMods[index].Level;
    } 
    
    private int CountLevel(float rand, List<BossLevelMod> list)
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
}

[Serializable]
public class BossLevelMod
{
    [SerializeField] private int level;
    [SerializeField] private long chance;

    public int Level => level;
    public long Chance => chance;
}