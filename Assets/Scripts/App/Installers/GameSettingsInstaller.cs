using App.Achievements;
using App.Chests;
using App.SetupsScript;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [SerializeField] private BathSetup bathSetup;
    [SerializeField] private WeaponSetup weaponSetup;
    [SerializeField] private CardsBorderSetup cardsBorderSetup;
    [SerializeField] private BasicCharacterSkinSetup characterSkinSetup;
    [SerializeField] private ChestSetup chestSetup;
    [SerializeField] private NameGenerator _nameGenerator;
    [SerializeField] private BattleSetup battleSetup;
    [SerializeField] private MergeCostSetup mergeCostSetup;
    [SerializeField] private TaskSetup taskSetup;

    public override void InstallBindings()
    {
        Container.BindInstance(bathSetup).AsSingle();
        Container.BindInstance(weaponSetup).AsSingle();
        Container.BindInstance(cardsBorderSetup).AsSingle();
        Container.BindInstance(characterSkinSetup).AsSingle();
        Container.BindInstance(chestSetup).AsSingle();
        Container.BindInstance(_nameGenerator).AsSingle();
        Container.BindInstance(battleSetup).AsSingle();
        Container.BindInstance(mergeCostSetup).AsSingle();
        Container.BindInstance(taskSetup).AsSingle();
    }
}