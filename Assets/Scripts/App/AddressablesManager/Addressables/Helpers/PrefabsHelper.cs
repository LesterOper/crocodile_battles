using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.AddressablesManager.Addressables.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace App.AddressablesTest
{
    public class PrefabsHelper : AddressableHelper<GameObject>
    {
        [SerializeField] private bool _releaseReferenceAfterDestroying = true;
        protected readonly Dictionary<AssetReference, List<GameObject>> spawnedPrefabs =
            new Dictionary<AssetReference, List<GameObject>>();
         
        public async Task<GameObject> SpawnPrefab(AssetReference assetReference, Properties properties)
        {
            await Get(assetReference);
            if (_asyncOperationHandles[assetReference].Status == AsyncOperationStatus.Succeeded)
            {
                return await SpawnAfterLoading(assetReference, properties);
            }
            else
            {
                throw new NullReferenceException("[PrefabsHelper] AsyncOperationHandler has not loaded a reference");
            }
        }


        private async Task<GameObject> SpawnAfterLoading(AssetReference assetReference, Properties properties)
        {
            var handler = assetReference.InstantiateAsync(properties.pos, properties.rotation, properties.parent);
            await handler.Task;
            
            if (spawnedPrefabs.ContainsKey(assetReference) == false)
            {
                spawnedPrefabs[assetReference] = new List<GameObject>();
            }
            spawnedPrefabs[assetReference].Add(handler.Result);
            var notify = handler.Result.AddComponent<NotifyOnDestroy>();
            notify.Destroyed += Remove;
            notify.AssetReference = assetReference;
            return handler.Result;
        }
        
        private void Remove(AssetReference assetReference, NotifyOnDestroy obj)
        {
            Addressables.ReleaseInstance(obj.gameObject);
            spawnedPrefabs[assetReference].Remove(obj.gameObject);
            if (_releaseReferenceAfterDestroying && spawnedPrefabs[assetReference].Count == 0)
            {
                if(_asyncOperationHandles[assetReference].IsValid())
                    Addressables.Release(_asyncOperationHandles[assetReference]);

                _asyncOperationHandles.Remove(assetReference);
            }
        }

        private void RemoveAll()
        {
            foreach (var keyValue in _asyncOperationHandles)
            {
                if(_asyncOperationHandles[keyValue.Key].IsValid())
                    Addressables.Release(_asyncOperationHandles[keyValue.Key]);
            }
            _asyncOperationHandles.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            if (_releaseReferenceAfterDestroying == false)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
//            RemoveAll();
        }

        private void OnDestroy()
        {
            RemoveAll();
        }
    }
    
    public struct Properties
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Transform parent;

        public Properties(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            this.pos = pos;
            this.rotation = rot;
            this.parent = parent;
        }
    }
}