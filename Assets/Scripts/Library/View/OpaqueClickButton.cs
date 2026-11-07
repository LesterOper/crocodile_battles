using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    [RequireComponent(typeof(Image))]
    public class OpaqueClickButton : MonoBehaviour
    {
        [SerializeField] private float _alphaThreshold = 0.4f;

        private void Start()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = _alphaThreshold;
        }
    }
}