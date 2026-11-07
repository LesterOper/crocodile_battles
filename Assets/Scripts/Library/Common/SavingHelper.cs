using System;
using System.Collections.Generic;
using App.DAO;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Zenject;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class SavingHelper : MonoBehaviour
    {
        #region Data
        
        public delegate void OperationCompletedHandler(bool success);
        public delegate void OperationLoadDataCompletedHandler(Dictionary<Type, object[]> CloudData);
        public const string DATA_KEY = "DATA_SAVE";
        private const string SAVING_DATA_TIME_KEY = "SavingHelper_save_time";

        private OperationCompletedHandler _completeHandler;

        private OperationCompletedHandler _operationCompletedHandler;
        private OperationLoadDataCompletedHandler _completeDataHandler;
        private string _savingData;

        protected bool Available { get; private set; }
        public static  bool InAction { get;  set; }

        #endregion

        [Inject] private IDataService _dataService;
        private LoadProgress _loadProgress;
        private SaveProgress _saveProgress;
        
        private void OnEnable()
        {
            CloudServices.OnSynchronizeComplete += CloudServicesOnOnSynchronizeComplete; 
            CloudServices.OnUserChange += CloudServicesOnOnUserChange; 
            CloudServices.OnSavedDataChange += CloudServicesOnOnSavedDataChange;
            _loadProgress = new LoadProgress();
            _saveProgress = new SaveProgress();
            //CloudServices.Synchronize();
        }

        private void OnDisable()
        {
            CloudServices.OnSynchronizeComplete -= CloudServicesOnOnSynchronizeComplete; 
            CloudServices.OnUserChange -= CloudServicesOnOnUserChange; 
            CloudServices.OnSavedDataChange -= CloudServicesOnOnSavedDataChange; 
        }

        private void CloudServicesOnOnSavedDataChange(CloudServicesSavedDataChangeResult result)
        {
        }

        private void CloudServicesOnOnUserChange(CloudServicesUserChangeResult result, Error error)
        {
        }

        private void CloudServicesOnOnSynchronizeComplete(CloudServicesSynchronizeResult result)
        {
        }

        
        public void LoadCloudData(OperationLoadDataCompletedHandler completedHandler) =>
            _loadProgress.LoadCloudData(completedHandler);

        public void SaveGame(OperationCompletedHandler completedHandler) =>
            _saveProgress.SaveGame(completedHandler, _dataService);

        public void SaveEmptyJson()
        {
            CloudServices.SetString(SavingHelper.DATA_KEY, "");
        }
    }
}