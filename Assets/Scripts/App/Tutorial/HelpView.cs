using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Tutorial
{
    public class HelpView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform rect;
        [SerializeField] private CloudHelpText _helpText;
        [SerializeField] private Image blackOut;
        [SerializeField] private GameObject standartBlackOut;

        public void Setup(TutorialStepType stepType)
        {
            _helpText.Setup(stepType);
            blackOut.gameObject.SetActive(false);
            standartBlackOut.SetActive(true);
            Show();
        }

        public void SetupWhole()
        {
            rect.gameObject.SetActive(false);
            blackOut.gameObject.SetActive(true);
            standartBlackOut.SetActive(false);
            Show();
        }

        public void SetBlackOutProperties(Transform target)
        {
            blackOut.gameObject.SetActive(true);
            standartBlackOut.SetActive(false);
            blackOut.transform
                .DOMove(target.position, 0.2f)
                .OnComplete(() => blackOut.DOFade(0.5f, 0.0f));
        }
        
        private void Show()
        {
            _canvasGroup
                .DOFade(1, 0.4f)
                .SetDelay(0.3f);
        }

        public void Fade(float alpha = 0, float duration = 0.2f)
        {
            _canvasGroup
                .DOFade(alpha, duration)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}