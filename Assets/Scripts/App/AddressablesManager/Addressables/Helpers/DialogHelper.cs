using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.AddressablesTest;
using Library.Dialogs;
using UnityEngine;

namespace App.AddressablesManager.Addressables.Helpers
{
    public class DialogHelper : PrefabsHelper
    {
         public RectTransform ParentRectTransform;
        [SerializeField] private List<DialogAddressable> _dialogs;

        public async Task<GameObject> GetDialog(DialogType dialogType) => await GetDialog(dialogType, ParentRectTransform);
        public async Task<TDialogType> GetDialog<TDialogType>(DialogType dialogType) where TDialogType : Dialog
        {
            var dialog =  await GetDialog(dialogType, ParentRectTransform);
            dialog.SetActive(true);
            return dialog.GetComponent<TDialogType>();
        }

        private async Task<GameObject> GetDialog(DialogType dialogType, RectTransform canvas)
        {
             var dialog = DialogAddressable(dialogType);
 
             var prop = new Properties(Vector3.zero, Quaternion.identity, canvas);
             return await SpawnPrefab(dialog.AssetReference, prop);           
        }

        private DialogAddressable DialogAddressable(DialogType dialogType) => _dialogs.First(d => d.Type == dialogType);
        
    }
}