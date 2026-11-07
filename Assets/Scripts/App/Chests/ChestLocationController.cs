using System.Collections.Generic;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using Zenject;

namespace App.Chests
{
    public class ChestLocationController : MonoBehaviour
    {
        [SerializeField] private ChestActivation _chestActivator;
        [SerializeField] private List<ChestHandler> _chests;
        [SerializeField] private ChestSlot _chestSlot;
        
        private IDataService _dataService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService)
        {
            _signalBus = signalBus;
            _dataService = dataService;
        }
        public ChestSlot ChestSlot
        {
            get { return _chestSlot; }
            private set { }
        }

        private void Start()
        {
            _signalBus.Subscribe<ActivateChestSignal>(ActivateChest);
            _signalBus.Subscribe<ChestCollectedSignal>(UpdateSlot);
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ActivateChestSignal>(ActivateChest);
            _signalBus.TryUnsubscribe<ChestCollectedSignal>(UpdateSlot);
        }
        private void Awake()
        {
            InitSlotInfo();
            InitChests();
        }
        
        private void InitSlotInfo()
        {
            if (_dataService.GetChestSlotsData().Slots.Count < 4)
            {
                _dataService.GetChestSlotsData().Slots.Add(new SlotInfo()
                    {ChestSlot = _chestSlot, ChestType = ChestType.NONE, Empty = true});
                _dataService.UpdateChestSlotsData();
            }
        }

        private void InitChests()
        {
            if (_dataService.GetChestSlotsData().Slots.Find(info => info.ChestSlot == _chestSlot).Empty)
            {
                foreach (var chest in _chests)
                {
                    chest.gameObject.SetActive(false);
                }
                _chestActivator.IsSlotEmpty = true;
            }
            else
            {
                var chestType = _dataService.GetChestSlotsData().Slots.Find(info => info.ChestSlot == _chestSlot)
                    .ChestType;
                _chestActivator.gameObject.SetActive(false);
                _chestActivator.IsSlotEmpty = false;
                _chests.Find(chest=>chest.ChestType==chestType).gameObject.SetActive(true);
            }
        }

        private void ActivateChest(ActivateChestSignal signal)
        {
            if (signal.SlotToPlace == (int) _chestSlot)
            {
                _chestActivator.gameObject.SetActive(false);
                _chestActivator.IsSlotEmpty = false;
                ChestType chestType = signal.ChestType;
                ChestHandler activatedChest = _chests.Find(chest => chest.ChestType == chestType);
                activatedChest.gameObject.SetActive(true);
                activatedChest.SetupChest();
                _signalBus.Fire<HasChestSignal>();
            }
            _signalBus.Fire<UpdateChestSlotsSignal>();
        }

        private void UpdateSlot(ChestCollectedSignal signal)
        {
            if (signal.ChestSlot == _chestSlot)
            {
                foreach (var chest in _chests)
                {
                    chest.gameObject.SetActive(false);
                }
                _chestActivator.gameObject.SetActive(true);
                _chestActivator.IsSlotEmpty = true;
                _signalBus.Fire<UpdateChestSlotsSignal>();
            }
        }
    }
    
    public enum ChestSlot
    {
        NONE = 0,
        SLOT_1 =1,
        SLOT_2 =2,
        SLOT_3 =3,
        SLOT_4 =4,
    }
}