using System.Collections.Generic;
using App.Cards;

namespace App.DAO
{
     public class BattleCardInfoData
     {
          public List<BattleCardInfo> CardInfos { get; set; } = new List<BattleCardInfo>();
          
          public BattleCardInfoData(){}

          public BattleCardInfoData(List<BattleCardInfo> cardInfos)
          {
               CardInfos = new List<BattleCardInfo>(cardInfos);
          }
     }
}