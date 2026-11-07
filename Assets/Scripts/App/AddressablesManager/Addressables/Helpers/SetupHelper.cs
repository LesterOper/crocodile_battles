using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.AddressablesManager.Addressables.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace App.AddressablesTest
{
    public class SetupHelper : AddressableHelper<ScriptableObject>
    {
        [SerializeField] private List<Setup> _setups;
        public async Task<T> GetSetup<T>(SetupType setupType) where T : ScriptableObject
        {
            return await GetSetup(setupType) as T;
        }
        public async Task<ScriptableObject> GetSetup(SetupType setupType)
        {
            var setup = _setups.First(s => s.Type == setupType);
            return await Get(setup.AssetReference);
        }
        public void FreeSetup(SetupType setupType)
        {
            var assetReference = _setups.First(s => s.Type == setupType).AssetReference;
            Free(assetReference);
        }

        private void FreeSetup(AssetReference assetReference)
        {
            Free(assetReference);
        }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            foreach (var setup in _setups)
            {
                FreeSetup(setup.AssetReference); 
            }
        }
    }
    
    [Serializable]
    public class Setup
    {
        [SerializeField] private AssetReference _assetReference;
        [SerializeField] private SetupType _type;

        public AssetReference AssetReference => _assetReference;
        public SetupType Type => _type;
    }
}