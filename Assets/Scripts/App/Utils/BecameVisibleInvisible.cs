using UnityEngine;

namespace App.Utils
{
    public class BecameVisibleInvisible : MonoBehaviour
    {
        private void OnBecameVisible()
        {
            gameObject.SetActive(true);
        }

        private void OnBecameInvisible()
        {
            gameObject.SetActive(false);
        }
    }
}