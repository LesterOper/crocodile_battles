using App;
using App.Chests;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using Zenject;

public class PanelForTesting : MonoBehaviour
{
    [SerializeField] private ChestSetup _chestSetup;
    private IDataService _dataService;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus, IDataService dataService)
    {
        _signalBus = signalBus;
        _dataService = dataService;
    }

    public void AddMiniChest()
    {
        _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == ChestType.MINI).chestAmount += 1;
            _dataService.UpdateChestCollectionData();
            _signalBus.Fire<HasChestSignal>();
    }
    public void AddMediumChest()
    {
        _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == ChestType.MEDIUM).chestAmount += 1;
        _dataService.UpdateChestCollectionData();
        _signalBus.Fire<HasChestSignal>();
    }
    public void AddMegaChest()
    {
        _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == ChestType.MEGA).chestAmount += 1;
        _dataService.UpdateChestCollectionData();
        _signalBus.Fire<HasChestSignal>();
    }
    
    
}
