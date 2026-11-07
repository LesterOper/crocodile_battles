using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace App.AddressablesTest
{
    public class TransferSceneButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("garage 2"); });
        }
    }
}