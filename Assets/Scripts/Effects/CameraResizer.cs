using UnityEngine;

namespace Effects
{
    public class CameraResizer : MonoBehaviour
    {
        private void Awake()
        {
            float ratio = (float)Screen.width / (float)Screen.height;
            if (ratio < 1.5f)
            {
                GetComponent<Camera>().orthographicSize = 6.59f;
            }
        }
    }
}
