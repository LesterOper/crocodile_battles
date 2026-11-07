using System.Threading.Tasks;
using App.AddressablesManager.Addressables;
using App.SetupsScript;
using UnityEngine;
using UnityEngine.UI;

namespace App.AddressablesTest
{
    public class SetupLoaderButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        /*private void Start()
        {
            GetComponent<Button>().onClick.AddListener(async () => await InstantiateSprite());
        }

        private async Task InstantiateSprite()
        {
            var setup = (BathSetup) await AddressableManager.Instance.SetupHelper.GetSetup(SetupType.BathSetup);
            var sprite = setup.GetBathByType(BathType.BATH_TYPE_1).BathIcon;
            icon.sprite = sprite;
        }*/
    }
}