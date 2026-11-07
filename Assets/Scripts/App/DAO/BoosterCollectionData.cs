using System.Collections.Generic;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class BoosterCollectionData
    {
        public BoosterCollectionData() { }
        [RequiredMember]public List<BoosterInfo> BoosterCollection { get; set; }
        
    }
}