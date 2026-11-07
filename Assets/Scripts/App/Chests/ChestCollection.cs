using App.DAO;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace App.Chests
{
    public class ChestCollection : MonoBehaviour
    {
        [Inject]private ChestSetup _chestSetup;
        [Inject]private IDataService _dataService;
        private void Start()
        {
            if (_dataService.GetChestCollectionData().ChestCollection.Count == 0)
            {
                AddChestData(ChestType.MINI);
                AddChestData(ChestType.MEDIUM);
                AddChestData(ChestType.MEGA);
                AddChestData(ChestType.GIGA);
            }
        }

        private void AddChestData(ChestType type)
        {
            _dataService.GetChestCollectionData().ChestCollection.Add(new ChestInfo
            {
                type = type, chestAmount = 0,readyTimeMinutes = GetReadyChestTime(type),
                cardAmount = _chestSetup.GetChestCharacteristic(type).trophyCount, canOpen = false, onLocation = false
            });
            _dataService.UpdateChestCollectionData();
        }
        
        private int GetReadyChestTime(ChestType type)
        {
            return _chestSetup.GetChestCharacteristic(type).readyTimeMinutes;
        }
    }
}