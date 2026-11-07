using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.OilRush.App
{
    public class CanvasResizer : MonoBehaviour
    {
        private const float BaseCanvasHeight = 720f;
        private const float BaseCanvasWidth = 1280f;
        private const float BaseScreenRatio = 16 / 9f;

        private void Awake()
        {
            SetupCanvas();
        }

        private void SetupCanvas()
        {
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(
                BaseCanvasWidth,
                BaseCanvasHeight * Mathf.Max(1f, (Screen.height / (float) Screen.width) / BaseScreenRatio));

            transform.localScale =new Vector3( 0.01f, 0.01f, 0.01f);
        }
    }
}
