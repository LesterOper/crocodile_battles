using System.Threading.Tasks;
using App.AddressablesManager.Addressables;
using App.AddressablesTest;
using App.Counters;
using App.Crocodile;
using App.Crocodile.Implementations;
using App.Crocodile.Interfaces;
using App.DAO;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using Battle;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Factory
{
  public class GameFactory : IGameFactory
  {
    private CrocodileController playerCrocodile;
    private CrocodileController enemyCrocodile;
    [Inject]private IDataService _dataService;
    [Inject]private SignalBus _signalBus;
    [Inject] private CounterUtility _counterUtility;
    [Inject] private IGameInfoService _gameInfoService;
    [Inject]private BathSetup _bathSetup;
    [Inject]private BattleSetup _battleSetup;
    [Inject]private WeaponSetup _weaponSetup;
    [Inject]private NameGenerator _nameGenerator;

    public void Setup(SignalBus signalBus)
    {
      _signalBus = signalBus;
    }
#pragma warning disable 1998
    public async Task<CrocodileController> CreateCrocodile(IBattleController battleController, Vector3 at, string name,
#pragma warning restore 1998
      bool isPlayer)
    {
      CrocodileController controller = InstantiateCrocodileController(name, at, isPlayer, _weaponSetup, _battleSetup, _nameGenerator.GetRandomName(), _signalBus);
      
      if (isPlayer) playerCrocodile = controller;
      else enemyCrocodile = controller;
      
      GameObject bath = InitializeCrocodile(controller, _bathSetup, isPlayer);
      GameObject bathObject = GameObject.Instantiate(bath, controller.transform);
      bathObject.layer = LayerMask.NameToLayer(isPlayer ? "Player" : "Enemy");
      
      BathFastenersHandler bathFastenersHandler = bathObject.GetComponent<BathFastenersHandler>();
      bathFastenersHandler.BathRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

      if (!isPlayer)
      {
        InverseRigidbodyForEnemy(bathFastenersHandler.BathRigidbody);
        bathFastenersHandler.RotateWaves();
      }
      controller.SetBathFastenerHandler(bathFastenersHandler);
      return controller;
    }

    private GameObject InitializeCrocodile(ICrocodileController _controller, BathSetup bathSetup, bool isPlayer)
    {
      BathView bathView = bathSetup.GetBathByType(isPlayer ? 
        _dataService.GetBathData().CrocodileBathType : 
        EnemyGenerator.GetEnemyBathType(_dataService));
      ICrocodile crocodile;
      if (isPlayer)
        crocodile = new PlayerCrocodile( isPlayer, _signalBus);
      else crocodile = new BotCrocodile( isPlayer, _signalBus);
      _controller.SetupCrocodile(_dataService,_counterUtility, crocodile, _gameInfoService);
      return bathView.BathObject;
    }

    private static Rigidbody2D InverseRigidbodyForEnemy(Rigidbody2D rigidbody2D)
    {
      var transform = rigidbody2D.transform;
      var localScale = transform.localScale;
      localScale.x *= -1;
      transform.localScale = localScale;
      return rigidbody2D;
    }


    public void Cleanup()
    {
      if (playerCrocodile != null && enemyCrocodile != null)
      {
        GameObject.Destroy(this.playerCrocodile.gameObject);
        GameObject.Destroy(this.enemyCrocodile.gameObject);
      }
    }

    private static CrocodileController InstantiateCrocodileController(string name, Vector3 at, bool isPlayer, 
      WeaponSetup weaponSetup, BattleSetup battleSetup, string playerName, SignalBus signalBus)
    {
      var go = new GameObject(name);
      go.transform.position = at;
      var controller = go.AddComponent<CrocodileController>();
      var equipingHandler = go.AddComponent<WeaponEquipingHandler>();
      controller.SetWeaponEquippingHandler(equipingHandler);
      controller.SetPlayerName(playerName);
      controller.SetBattleSetup(battleSetup);
      equipingHandler.SetupWeaponEquipping(isPlayer, signalBus);
      return controller;
    }


    private static async Task<BathSetup> GetBathSetup()
    {
      return await AddressableManager.Instance.SetupHelper.GetSetup<BathSetup>(SetupType.BathSetup);
    }

    private static async Task<BattleSetup> GetBattleSetup()
    {
      return await AddressableManager.Instance.SetupHelper.GetSetup<BattleSetup>(SetupType.BattleSetup);
    }

    private static async Task<NameGenerator> GetNameGenerator()
    {
      return await AddressableManager.Instance.SetupHelper.GetSetup<NameGenerator>(SetupType.NameGenerator);
    }
    private static async Task<WeaponSetup> GetWeaponSetup()
    {
      return await AddressableManager.Instance.SetupHelper.GetSetup<WeaponSetup>(SetupType.WeaponSetup);
    }
  }
}