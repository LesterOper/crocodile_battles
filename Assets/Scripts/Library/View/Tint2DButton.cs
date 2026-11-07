using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Library.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tint2DButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private Color _normalColor;

        private bool _pressed;
        [SerializeField] private Color _pressedColor;

        private SpriteRenderer _spriteRenderer;

        public Color PressedColor
        {
            get => _pressedColor;
            set
            {
                _pressedColor = value;
                UpdateCurrentColorColor();
            }
        }

        public Color NormalColor
        {
            get => _normalColor;
            set
            {
                _normalColor = value;
                UpdateCurrentColorColor();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
            UpdateCurrentColorColor();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
            UpdateCurrentColorColor();
        }

        public event Action OnClick;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _normalColor = _spriteRenderer.color;
        }

        private void UpdateCurrentColorColor()
        {
            _spriteRenderer.color = _pressed ? _pressedColor : _normalColor;
        }
    }
}