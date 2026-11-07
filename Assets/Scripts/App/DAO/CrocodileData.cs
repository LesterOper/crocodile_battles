using App.SetupsScript;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class CrocodileData
    {
  
        [RequiredMember] [PrimaryKey]
        public int ID  { get; set; }
   
        [RequiredMember]
        public long CrocodileLevel { get; set; }
        [RequiredMember]
   
        public long DefaultMoney{ get; set; }
    
        [RequiredMember]
        public long PremMoney{ get; set; }
    
        [RequiredMember]
        public CharacterType CharacterType { get; set; }
    
        [RequiredMember]
        public long BattlesCountWin { get; set; }

        [RequiredMember] 
        public long BossBattlesWinCount { get; set; }

        [RequiredMember]
        public long BattlesCountLoose { get; set; }
    }
}