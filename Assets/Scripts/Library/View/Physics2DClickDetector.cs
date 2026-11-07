using UnityEngine;
using UnityEngine.EventSystems;

namespace Library.View
{
    public class Physics2DClickDetector : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ClickHandler();

        private bool _pressed;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public event ClickHandler OnClick;
    }
}