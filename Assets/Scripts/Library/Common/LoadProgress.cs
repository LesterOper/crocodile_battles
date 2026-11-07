﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using VoxelBusters.EssentialKit;

namespace Library.Common
{
    public class LoadProgress
    { 
        private SavingHelper.OperationLoadDataCompletedHandler _completeDataHandler;
        public void LoadCloudData(SavingHelper.OperationLoadDataCompletedHandler completedHandler)
        {
            if (!SavingHelper.InAction)
            {
                SavingHelper.InAction = true;
                _completeDataHandler = completedHandler;
                CloudServices.OnSynchronizeComplete += GetCloudProgress;
                Debug.Log("[SavingHelper] - LoadCloudData - Listener");
                CloudServices.Synchronize();
                Debug.Log("[SavingHelper] - LoadCloudData - Synchronized");
            }
        }

        private void GetCloudProgress(CloudServicesSynchronizeResult result)
        {
            CloudServices.OnSynchronizeComplete -= GetCloudProgress;
            Dictionary<Type, object[]> readData;

            Debug.Log("[SavingHelper] - Before reading");
            string cloudString = "-";
            cloudString = CloudServices.GetString(SavingHelper.DATA_KEY); 
            Debug.Log("[SavingHelper] - After reading");
            bool totalResult = result.Success;

            Debug.Log("[SavingHelper] - Result:" + totalResult);
            Debug.Log("[SavingHelper] - CloudString - " + cloudString);

            if (totalResult || !string.IsNullOrEmpty(cloudString))
            {
                Debug.Log("[SavingHelper] - GetCloudProgress - smth in Cloud");
                DeserializeData(out readData);
                Debug.Log("[SavingHelper] - GetCloudProgress - smth in Cloud - read cloud : " + cloudString);
                
                if (_completeDataHandler != null)
                {
                    _completeDataHandler(readData);
                    _completeDataHandler = null;
                }
            }
            
            if(result.Success && string.IsNullOrEmpty(CloudServices.GetString(SavingHelper.DATA_KEY)))
            {
                Debug.Log("[SavingHelper] - GetCloudProgress - nothing in Cloud");
                _completeDataHandler?.Invoke(null);
                _completeDataHandler = null;
            }
            SavingHelper.InAction = false;
        }
        
        
        private void DeserializeData(out Dictionary<Type, object[]> readData)
        {
            var Data = CloudServices.GetString(SavingHelper.DATA_KEY);
            Debug.Log("[SavingHelper] - Deserialization : " + Data);

            var stringDataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Data);
            Debug.Log("[SavingHelper] - Deserialization : " + stringDataDict.Count);
            readData = new Dictionary<Type, object[]>();

            foreach (var stringDataEntry in stringDataDict)
            {
                Debug.Log("[SavingHelper] - Deserialization string data entry : " + stringDataEntry);
                Type type = null;
                foreach (var currentAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Debug.Log("[SavingHelper] - Deserialization current assembly full name : " + currentAssembly.FullName);
                    type = currentAssembly.GetType(stringDataEntry.Key, false, true);
                    Debug.Log("[SavingHelper] - Deserialization type: " + type);
                    if (type != null)
                        break;
                }

                if (type != null)
                {
                    readData[type] =
                        (object[]) JsonConvert.DeserializeObject(stringDataEntry.Value, type.MakeArrayType());
                    Debug.Log("[SavingHelper] - Deserialization read data: " + readData[type]);
                }
            }
        }
    }
}