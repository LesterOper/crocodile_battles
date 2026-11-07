using App.Achievements;
using App.Dialogs.BattlePass;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MainSceneSettingsInstaller", menuName = "Installers/MainSceneSettingsInstaller")]
public class MainSceneSettingsInstaller : ScriptableObjectInstaller<MainSceneSettingsInstaller>
{
    [SerializeField] private BattlePassPrizeContainer battlePassPrizeContainer;
    [SerializeField] private DailyTaskPrizeSetup dailyTaskPrizeSetup;
    [SerializeField] private ChestSettings chestSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(battlePassPrizeContainer).AsSingle();
        Container.BindInstance(dailyTaskPrizeSetup).AsSingle();
        Container.BindInstance(chestSettings).AsSingle();
    }
}