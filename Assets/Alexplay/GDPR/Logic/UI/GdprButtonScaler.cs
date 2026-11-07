using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Alexplay.GDPR.Logic.UI
{
    public class GdprButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CanvasGroup _buttonCanvasGroup;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            BreakTweens();
            
            _buttonCanvasGroup.DOFade(0.75f, 0.25f);
            transform.DOScale(0.9f, 0.05f).OnComplete(() => 
                transform.DOScale(0.95f, 0.1f).OnComplete(() => 
                    transform.DOScale(0.9f, 0.1f)));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            BreakTweens();
            
            _buttonCanvasGroup.DOFade(1f, 0.25f);
            transform.DOScale(1f, 0.05f).OnComplete(() => 
                transform.DOScale(0.95f, 0.1f).OnComplete(() => 
                    transform.DOScale(1f, 0.1f)));
        }

        private void BreakTweens()
        {
            _buttonCanvasGroup.DOPause();
            _buttonCanvasGroup.DOKill();
            
            transform.DOPause();
            transform.DOKill();
        }
    }
}
