using System;
using App.SetupsScript;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class BathData : IComparable<BathData>, ICardInfo
    {
        [RequiredMember] [PrimaryKey]
        public int ID  { get; set; }
        [RequiredMember]
        public BathType CrocodileBathType { get; set; } = BathType.NONE;
        [RequiredMember]
        public int Level { get; set; } = 0;

        public bool InUseForBattle;
        
        public BathData(){}
        public BathData(BathData bathData)
        {
            Level = bathData.Level;
            CrocodileBathType = bathData.CrocodileBathType;
        }

        public BathData(ICardInfo cardInfo)
        {
            Level = cardInfo.GetLevel();
            CrocodileBathType = (BathType) cardInfo.GetCardType();
            ID = cardInfo.GetId();
            InUseForBattle = true;
        }
        
        public BathData(BathInCollectionData bathData)
        {
            Level = bathData.Level;
            CrocodileBathType = bathData.CrocodileBathType;
            InUseForBattle = false;
        }
     
        public void SetData(BathData bathData)
        {
            Level = bathData.Level;
            CrocodileBathType = bathData.CrocodileBathType;
            SetIsInUseForBattle(true);
        }
        
        public bool IsInUseForBattle() => InUseForBattle;
        public void SetIsInUseForBattle(bool isUse)
        {
            InUseForBattle = isUse;
        }

        public int GetLevel() => Level;
        public void SetLevel(int level)
        {
            Level = level;
        }
        public int GetCardType() => (int) CrocodileBathType;

        public bool IsBathCard() => true;
        public int GetId() => ID;
     
        public bool Equals(BathData bathData) => CrocodileBathType == bathData.CrocodileBathType && Level == bathData.Level;

        public bool Equals(ICardInfo cardInfo) => CrocodileBathType == (BathType)cardInfo.GetCardType() && Level == cardInfo.GetLevel();

        public override string ToString() => CrocodileBathType.ToString() ;

        public int CompareTo(BathData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var idComparison = ID.CompareTo(other.ID);
            if (idComparison != 0) return idComparison;
            var crocodileBathTypeComparison = CrocodileBathType.CompareTo(other.CrocodileBathType);
            if (crocodileBathTypeComparison != 0) return crocodileBathTypeComparison;
            return Level.CompareTo(other.Level);
        }
    }
}