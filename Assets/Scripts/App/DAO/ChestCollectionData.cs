using System.Collections.Generic;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class ChestCollectionData
    {
        public ChestCollectionData() { }
        
        [RequiredMember]public List<ChestInfo> ChestCollection { get; set; }
    }
}