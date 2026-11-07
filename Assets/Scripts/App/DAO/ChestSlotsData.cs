using System.Collections.Generic;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class ChestSlotsData
    {
        [RequiredMember]private List<SlotInfo> slots = new List<SlotInfo>();
        
        public ChestSlotsData() { } 
        public ChestSlotsData(List<SlotInfo> slotInfos)
        {
            slots = slotInfos;
        }
    
        public List<SlotInfo> Slots
        {
            get => slots;
            set => slots = value;
        }
    }
}