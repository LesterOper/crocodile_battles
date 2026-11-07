using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Library.CanvasNamespace
{
    public class StepByStepScrollController : MonoBehaviour  , IBeginDragHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;

        private int _cardsCount;
        private float _distance = 0f;
        private void Start()
        {
            CalculateScrollDistance();
        }

        private void CalculateScrollDistance()
        {
            if (_content.childCount > 12)
            {
                _distance = Vector2.Distance(_content.GetChild(0).transform.position, _content.GetChild(4).transform.position);
            }
            else
            {
                _distance = 0;
            }
           
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.delta.y > 0)
            {
                if (_scrollRect.verticalNormalizedPosition >0)
                {
                    _canvasGroup.alpha = 0;
                    _content.DOMoveY(_content.transform.position.y + _distance, 0.2f).OnComplete(() =>
                    {
                        _canvasGroup.DOFade(1, 0.1f);
                    });
                }
            }
            else
            {
                if (_scrollRect.verticalNormalizedPosition < 1)
                {
                    _canvasGroup.alpha = 0;
                    _content.DOMoveY(_content.transform.position.y - _distance, 0.2f).OnComplete(() =>
                    {
                        _canvasGroup.DOFade(1, 0.1f);
                    });
                }
            }
        }
    }
}
