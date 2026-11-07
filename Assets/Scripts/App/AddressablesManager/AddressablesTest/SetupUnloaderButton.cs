using App.AddressablesManager.Addressables;
using UnityEngine;
using UnityEngine.UI;

namespace App.AddressablesTest
{
    public class SetupUnloaderButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                AddressableManager.Instance.SetupHelper.FreeSetup(SetupType.BathSetup);
            });
        }

    }
}