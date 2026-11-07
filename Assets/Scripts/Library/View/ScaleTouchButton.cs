using UnityEngine;
using UnityEngine.EventSystems;

namespace Library.View
{
    public class ScaleTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector2 _initialScale;
        [SerializeField] private float _touchedScale = 0.8f;

        public void OnPointerDown(PointerEventData eventData)
        {
            GetComponent<RectTransform>().localScale = _initialScale * _touchedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            GetComponent<RectTransform>().localScale = _initialScale;
        }

        private void Start()
        {
            _initialScale = GetComponent<RectTransform>().localScale;
        }
    }
}