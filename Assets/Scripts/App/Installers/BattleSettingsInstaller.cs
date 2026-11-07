using App.SetupsScript;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BattleSettingsInstaller", menuName = "Installers/BattleSettingsInstaller")]
public class BattleSettingsInstaller : ScriptableObjectInstaller<BattleSettingsInstaller>
{
    [SerializeField] private BattleSetup battleSetup;
    [SerializeField] private NameGenerator nameGenerator;
    [SerializeField] private WeightsRandomBathSettings weightsRandomBathSettings;
    [SerializeField] private BossBuildSettings bossSettings;
    public override void InstallBindings()
    {
        Container.BindInstance(battleSetup);
        Container.BindInstance(bossSettings);
        Container.BindInstance(nameGenerator);
        Container.BindInstance(weightsRandomBathSettings);
    }
}