using System;
using System.Collections.Generic;
using App.DAO;
using CodeBase.Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;
using VoxelBusters.EssentialKit;

namespace Library.Common
{
    public class SaveProgress
    {
        //protected const string DATA_KEY = "data";
        private const string SAVING_DATA_TIME_KEY = "SavingHelper_save_time";
        private SavingHelper.OperationCompletedHandler _completeHandler;
        public void SaveGame(SavingHelper.OperationCompletedHandler completedHandler, IDataService dataService)
        {
            if (SavingHelper.InAction)
            {
                Debug.Log("[SavingHelper] - SaveGame - InAction: " + SavingHelper.InAction);
                completedHandler(false);
                Debug.Log("[SavingHelper] - SaveGame - completeHandler");
                return;
            }
            
            CompleteSaveGame(completedHandler, dataService);
        }
        private void CompleteSaveGame(SavingHelper.OperationCompletedHandler completedHandler, IDataService dataService)
        {
            if (!SavingHelper.InAction)
            {
                SavingHelper.InAction = true;
                _completeHandler = completedHandler;
                Debug.Log("[SavingHelper] - CompleteSaveGame - CompleteSaveGame");
                CloudServices.OnSynchronizeComplete += OnGameSaved;
                Debug.Log("[SavingHelper] - CompleteSaveGame - Listener - OnSynchronize");
                var data = dataService.GetAllData();
                Debug.Log("[SavingHelper] - CompleteSaveGame - DATA " + data.Count);
                Debug.Log("[SavingHelper] - CompleteSaveGame - DATA " + data); 
                    
                string dataStr = SerializeSavingData(data);
                Debug.Log("[SavingHelper] - CompleteSaveGame - DATA " + dataStr);
                if (string.IsNullOrEmpty(dataStr))
                {
                    Debug.Log("[SavingHelper] - CompleteSaveGame - STR Empty " + dataStr);
                    SavingHelper.InAction = false;
                    CloudServices.OnSynchronizeComplete  -= OnGameSaved;
                    return;
                }
                CloudServices.Synchronize();
                Debug.Log("[SavingHelper] - CompleteSaveGame - DeleteData");
                CloudServices.SetString(SavingHelper.DATA_KEY, dataStr);
                Debug.Log("[SavingHelper] - CompleteSaveGame - SetData");
                CloudServices.Synchronize();
                Debug.Log("[SavingHelper] - CompleteSaveGame - Synchronize");
            }
        }
        
        private void OnGameSaved(CloudServicesSynchronizeResult result)
        {
            CloudServices.OnSynchronizeComplete  -= OnGameSaved;
            Debug.Log("[SavingHelper] - OnGameSaved - Completed");

            if (_completeHandler != null)
            {
                Debug.Log("[SavingHelper] - OnGameSaved - Completed - _completeHandler != null");
                _completeHandler(result.Success);
                Debug.Log("[SavingHelper] - OnGameSaved - Completed - _completeHandler - Invoke");
                _completeHandler = null;
            }

            SavingHelper.InAction = false;
        }
        
        private string SerializeSavingData(Dictionary<Type, object[]> data)
        {
            var savingValuesDict = new Dictionary<string, string>();
            Debug.Log("[SavingHelper] - Serialization : " + data.Count);
            int i = 0;
            foreach (var dataItem in data)
            {
                Debug.Log("[SavingHelper] - Serialization cycle START iteration  " + i + "   " + dataItem.Key.FullName);

                savingValuesDict[dataItem.Key.FullName ?? throw new NullReferenceException("KEY.FULLNAME IS NULL")] = JsonConvert.SerializeObject(dataItem.Value);
                Debug.Log("[SavingHelper] - Serialization cycle END iteration  " + i + "   " + dataItem.Key.FullName);
                i++;
            }

            Debug.Log("[SavingHelper] - Serialization cycle end");
            savingValuesDict[SAVING_DATA_TIME_KEY] = DateTime.UtcNow.Ticks.ToString();
            Debug.Log("[SavingHelper] - Serialization end");
            return JsonConvert.SerializeObject(savingValuesDict);
        }
    }

    public class SaveLoadProgressKeyContainer
    {
        private const string DATA_KEY = "data";
        public static string BathDataKey = DATA_KEY + "_BathData";
        public static string BathsCollectionDataKey = DATA_KEY + "_BathsCollectionData";
        public static string CardsCollectionDataKey = DATA_KEY + "_CardsCollectionData";
        public static string BattleCardsCollectionDataKey = DATA_KEY + "_BattleCardsCollectionData";
        public static string ChestInfoKey = DATA_KEY + "_ChestInfo";
        public static string SlotInfoKey = DATA_KEY + "_SlotInfo";
        public static string CardInfoKey = DATA_KEY + "_CardInfo";
    }
}