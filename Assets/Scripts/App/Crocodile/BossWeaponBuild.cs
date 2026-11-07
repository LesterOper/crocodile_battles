using System;
using App.SetupsScript;
using UnityEngine;

public class BossWeaponBuild : IDisposable
{
    private bool disposed = false;
    private BossBuildSettings bossBuildSettings;
    public BossWeaponBuild()
    {
        bossBuildSettings = Resources.Load<BossBuildSettings>("BossSettings");
    }

    public Boss GetBoss(BathType bathType) => bossBuildSettings.GetBossWeapons(bathType);

    public BossWeaponGroup GetWeaponGroup(ChestWeaponGroupType type) => bossBuildSettings.GetWeapons(type);

    public int BossBathLevel(BathType bathType) => bossBuildSettings.GetBossWeapons(bathType).BathLevel;
        
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
        
    protected virtual void Dispose(bool disposing)
    {
        if(disposed) return;
        if (disposing)
        {
                
        }

        disposed = true;
    }

    ~BossWeaponBuild()
    {
        Dispose(false);
    }

}