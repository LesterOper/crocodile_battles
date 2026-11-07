using System;
using AddressablesManager.Addressables;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.AddressablesManager.Addressables
{
    [Serializable]
    public class ParticleAddressable
    {
        [SerializeField] private ParticleType _type;
        [SerializeField] private AssetReference _assetReference;

        public AssetReference AssetReference => _assetReference;
        public ParticleType Type => _type;
    }
}