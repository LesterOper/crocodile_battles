using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.AddressablesManager.Addressables.Helpers
{
    public class AddressableHelper<T> : MonoBehaviour
    {
        protected virtual void Awake()
        {
//            DontDestroyOnLoad(gameObject);
        }

        public delegate void AddressableAssetLoaded(string assetGuid);
        public event AddressableAssetLoaded  AddressableAssetLoadedHandler;
        
        protected readonly Dictionary<AssetReference, AsyncOperationHandle<T>> _asyncOperationHandles = 
            new Dictionary<AssetReference, AsyncOperationHandle<T>>();

        public async Task<T> Get(AssetReference assetReference)
        {
            if (assetReference.RuntimeKeyIsValid() == false)
            {
                throw new InvalidKeyException("Invalid Key" + assetReference.RuntimeKey);
            }

            if (_asyncOperationHandles.ContainsKey(assetReference))
            {
                var handler = _asyncOperationHandles[assetReference];
                if (handler.Status == AsyncOperationStatus.Succeeded)
                {
                    return await Task.FromResult(handler.Result);
                }
                else
                {
                    return await handler.Task;
                }
            }

            return await Load(assetReference);
        }

        public bool AnyLoading() => _asyncOperationHandles.Any((pair => pair.Value.IsDone == false));

        private async Task<T> Load(AssetReference assetReference)
        {
            var handler = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(assetReference);
            handler.Completed += handle => { OnAssetLoaded(assetReference); };
            _asyncOperationHandles[assetReference] = handler;
            return await handler.Task;
        }

        private void OnAssetLoaded(AssetReference assetReference)
        {
            AddressableAssetLoadedHandler?.Invoke(assetReference.ToString());
        }

        public void Free(AssetReference assetReference)
        {
            if (!_asyncOperationHandles.ContainsKey(assetReference))
                return;
            UnityEngine.AddressableAssets.Addressables.Release(_asyncOperationHandles[assetReference]);
            _asyncOperationHandles.Remove(assetReference);
        }
    }
    

}