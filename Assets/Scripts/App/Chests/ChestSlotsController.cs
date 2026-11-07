using System.Linq;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using ModestTree;
using UnityEngine;
using Zenject;

namespace App.Chests
{
    public class ChestSlotsController : MonoBehaviour
    {
        [SerializeField] private ChestActivation[] chestSlots = null;
        private SignalBus _signalBus;
        private IDataService _dataService;

        [Inject]
        public void Construct(SignalBus signalBus, IDataService dataService)
        {
            _signalBus = signalBus;
            _dataService = dataService;
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<UpdateChestSlotsSignal>(InitSlotVisibility);
        }

        void Start()
        {
            InitSlotVisibility();
            _signalBus.Subscribe<UpdateChestSlotsSignal>(InitSlotVisibility);
        }
        
        private void InitSlotVisibility()
        {
            ChestActivation firstEmptySlot;
            try
            {
                firstEmptySlot = chestSlots.First(slot =>slot.IsSlotEmpty);
            }
            catch 
            {
                return;
            }

            if (firstEmptySlot == null) return;
            
            if (_dataService.GetChestCollectionData().ChestCollection
                    .Find(local => local.chestAmount > 0) != null)
            {
                firstEmptySlot.gameObject.SetActive(true);
                    
            }
            else
            {
                firstEmptySlot.gameObject.SetActive(false);
            }
            foreach (var slot in chestSlots.Except(firstEmptySlot))
                slot.gameObject.SetActive(false);
            
        }
    }
}