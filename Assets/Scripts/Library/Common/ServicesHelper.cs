using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Library.Common
{
    [DisallowMultipleComponent]
    public class ServicesHelper : MonoBehaviour
    {
        private void OnEnable()
        {
            // register for events
            CloudServices.OnUserChange              += OnUserChange;
            CloudServices.OnSavedDataChange         += OnSavedDataChange;
            CloudServices.OnSynchronizeComplete     += OnSynchronizeComplete;
        }

        private void OnDisable()
        {
            // unregister from events
            CloudServices.OnUserChange              -= OnUserChange;
            CloudServices.OnSavedDataChange         -= OnSavedDataChange;
            CloudServices.OnSynchronizeComplete     -= OnSynchronizeComplete;
        }
        
        // Register for the CloudServices.OnSyncronizeComplete event
// ...
        private void OnSynchronizeComplete(CloudServicesSynchronizeResult result)
        {
            Debug.Log("Received synchronize finish callback.");
            Debug.Log("Status: " + result.Success);
            // By this time, you have the latest data from cloud and you can start reading.
        }

        private void OnUserChange(CloudServicesUserChangeResult result, Error error)
        {
            Debug.Log("[ServicesHelper] : user change : account status = " + result.User.AccountStatus);
            // When account was changed
        }

        private void OnSavedDataChange(CloudServicesSavedDataChangeResult result)
        {
            Debug.Log("[ServicesHelper] : saving data change : status = success");
            // Check local keys and keys which was saved in cloud
        }
    }
}