using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace App.AddressablesManager.Addressables.Helpers
{
    [Serializable]
    public class DialogAddressable
    {
        [SerializeField] private AssetReference _assetReference;
        [SerializeField] private DialogType _dialogType;

        public AssetReference AssetReference => _assetReference;
        public DialogType Type => _dialogType;
    }
}