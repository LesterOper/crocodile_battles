using System;
using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.DAO;
using App.SetupsScript;

namespace Library.Data
{
    public class CrocoDataHelper : DataHelper
    {
        protected override List<Type> GetDatabaseTypes()
        {
            var types = base.GetDatabaseTypes();
            types.Add(typeof(ChestInfo));
            types.Add(typeof(SlotInfo));
            types.Add(typeof(BathData));
            types.Add(typeof(CrocodileData));
            types.Add(typeof(CardInfo));
            types.Add(typeof(BattleCardInfo));
            types.Add(typeof(BathInCollectionData));
            types.Add(typeof(TaskInfo));
            types.Add(typeof(BoosterInfo));
            return types;
        }

        public override Dictionary<Type, object[]> GetData()
        {
            /*var data = base.GetData();

            foreach (var dataEntry in _data)
                data[dataEntry.Key] =  dataEntry.Value.Values.ToArray();
            
            return data;*/
            return base.GetData();
        }

        
        
        public void SetCardCollectionData(List<CardInfo> cardInfos)
        {
            DeleteDataFromTable(new CardInfo());

            foreach (var card in cardInfos)
            {
                Set(card);
            }
        }

        public CardCollectionData GetCardCollectionData() => new CardCollectionData(Get<CardInfo>().ToList());

        public BattleCardInfoData GetBattleCardData() => new BattleCardInfoData(Get<BattleCardInfo>().ToList());

        public void SetBattleCardData(List<BattleCardInfo> cardInfos)
        {
            DeleteDataFromTable(new BattleCardInfo());
            
            foreach (var card in cardInfos)
            {
                Set(card);
            }
        }
        
        public CrocodileData GetCrocodileData()
        {
            CrocodileData data =  Get<CrocodileData>().FirstOrDefault();
            if (data == null)
                data = new CrocodileData() {CharacterType = CharacterType.BASIC_1, CrocodileLevel = 1, DefaultMoney = 0, PremMoney = 0};
            return data;
        }

        public void SetCrocodileData(CrocodileData crocodileData) => Set(crocodileData);


        public BathData GetBathData()
        {
            BathData data = Get<BathData>().FirstOrDefault();

            if (data == null)
            {
                return new BathData {ID = 1, Level = 2, CrocodileBathType = BathType.BATH_TYPE_10};
            }

            return data;
        }

        public void SetBathData(BathData bathData) => Set(bathData);

        public List<ChestInfo> GetChestCollectionData() => Get<ChestInfo>().ToList();

        public void SetChestCollectionData(List<ChestInfo> chestInfos)
        {
            foreach (var chest in chestInfos)
            {
                Set(chest);
            }
        }

        public void SetBoostersCollectionData(List<BoosterInfo> boosterInfos)
        {
            foreach (var booster in boosterInfos)
            {
                Set(booster);
            }
        }
        
        public void SetTasksCollectionData(List<TaskInfo> taskInfos)
        {
            foreach (var task in taskInfos)
            {
                Set(task);
            }
        }

        public void SetBathCollectionData(List<BathInCollectionData> bathDatas)
        {
            List<BathInCollectionData> oldCards = Get<BathInCollectionData>().ToList();
            foreach (var bathCard in oldCards)
            {
                Delete(bathCard);
            }
            
            foreach (var bath in bathDatas)
            {
                Set(bath);
            }
        }

        public List<BathInCollectionData> GetBathCollection() => Get<BathInCollectionData>().ToList();

        public void SetChestSlotsData(List<SlotInfo> slotInfos)
        {
            foreach (var slot in slotInfos)
            {
                Set(slot);
            }
        }
        
        public ChestSlotsData GetChestSlotsData() => new ChestSlotsData(Get<SlotInfo>().ToList());

        public void SetLastSessionTicks() => SetLong("LastSession", DateTime.UtcNow.Ticks);
        public long GetLastSessionTicks() => GetLong("LastSession");

        public override void Clear()
        {
            base.Clear();
        }

        public List<TaskInfo> GetTaskCollectionData() => Get<TaskInfo>().ToList();
        public List<BoosterInfo> GetBoosterCollectionData() => Get<BoosterInfo>().ToList();
    }
}