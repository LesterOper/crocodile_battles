using System;
using App.PrototypeBattle;
using App.SetupsScript;
using Library.Utils;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.Cards
{
    public class CardInfo : IComparable<CardInfo>, ICardInfo
    { 
        [RequiredMember] [PrimaryKey]
        public int ID  { get; set; }
        public int Level { get; set; } = 0;

        public WeaponType WeaponType { get; set; } = WeaponType.NONE;
        public WeaponActionType type { get; set; } = WeaponActionType.NONE;
        public CardInfo(){}
        
        public CardInfo(ICardInfo cardInfo)
        {
            ID = cardInfo.GetId();
            Level = cardInfo.GetLevel();
            WeaponType = (WeaponType)cardInfo.GetCardType();
            type = WeaponType.GetAttribute<WeaponData>().ActionType;
        }
        
        //public bool InUseForBattle { get; set; } = false;

        public CardInfo(BattleCardInfo cardInfo)
        {
            Level = cardInfo.Level;
            WeaponType = cardInfo.WeaponType;
            type = cardInfo.type;
        }
        
        public bool Equals(ICardInfo card)
        {
            return (WeaponType)card.GetCardType() == WeaponType && card.GetLevel() == Level;
        }

        public override string ToString() => WeaponType.ToString();

        public int CompareTo(CardInfo other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
        
            var idComparison = ID.CompareTo(other.GetId());
            if (idComparison != 0) return idComparison;
        
            var levelComparison = Level.CompareTo(other.GetLevel());
            if (levelComparison != 0) return levelComparison;
        
            var weaponTypeComparison = WeaponType.CompareTo((WeaponType)other.GetCardType());
            if (weaponTypeComparison != 0) return weaponTypeComparison;
        
            return type.CompareTo((WeaponType)other.GetCardType());
        }

        public bool IsInUseForBattle() => false;
        public void SetIsInUseForBattle(bool isUse)
        {
            return;
        }

        public int GetLevel() => Level;
        public void SetLevel(int level)
        {
            Level = level;
        }

        public int GetCardType() => (int) WeaponType;
        public bool IsBathCard() => false;
        public int GetId() => ID;
    }
}

public interface ICardInfo
{
    bool IsInUseForBattle();
    void SetIsInUseForBattle(bool isUse);
    int GetLevel();
    void SetLevel(int level);
    int GetCardType();
    bool IsBathCard();
    int GetId();
    bool Equals(ICardInfo cardInfo);
}

