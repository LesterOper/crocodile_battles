using System.Collections.Generic;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class BathCollection
    {
        [RequiredMember] public List<BathInCollectionData> BathDatas { get; set; } = new List<BathInCollectionData>();
    }

    public class BathInCollectionData : BathData
    {
        public BathInCollectionData() { }

        public BathInCollectionData(BathInCollectionData bath) : base(bath) {}

        public BathInCollectionData(ICardInfo cardInfo) : base(cardInfo)
        {
            SetIsInUseForBattle(false);
        }
    }
}