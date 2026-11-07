using App.Chests;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class ChestInfo
    {
        [PrimaryKey] [RequiredMember]
        public ChestType type { get; set; }
        [RequiredMember]
        public int chestAmount{ get; set; }
        [RequiredMember]
        public int readyTimeMinutes{ get; set; }
        [RequiredMember]
        public int cardAmount{ get; set; }
        [RequiredMember]
        public bool canOpen{ get; set; }
        [RequiredMember]
        public bool onLocation{ get; set; }
    }
    
    public class SlotInfo
    {
        [PrimaryKey] [RequiredMember]
        public ChestSlot ChestSlot { get; set; }
        [RequiredMember]
        public ChestType ChestType { get; set; }
        [RequiredMember]
        public bool Empty { get; set; }
    }
}