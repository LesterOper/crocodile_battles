using System.Collections.Generic;
using App.Cards;

namespace App.DAO
{
    public class CardCollectionData
    {
        public List<CardInfo> Collection { get; set; } = new List<CardInfo>();

        public CardCollectionData(){}

        public CardCollectionData(List<CardInfo> cardInfos)
        {
            Collection = new List<CardInfo>(cardInfos);
        }
    }
}