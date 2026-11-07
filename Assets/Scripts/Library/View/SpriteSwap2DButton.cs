using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Library.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSwap2DButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private Sprite _normalSprite;

        private bool _pressed;
        [SerializeField] private Sprite _pressedSprite;

        private SpriteRenderer _spriteRenderer;

        public Sprite PressedSprite
        {
            get => _pressedSprite;
            set
            {
                _pressedSprite = value;
                UpdateCurrentSprite();
            }
        }

        public Sprite NormalSprite
        {
            get => _normalSprite;
            set
            {
                _normalSprite = value;
                UpdateCurrentSprite();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
            UpdateCurrentSprite();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
            UpdateCurrentSprite();
        }

        public event Action OnClick;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _normalSprite = _spriteRenderer.sprite;
        }

        private void UpdateCurrentSprite()
        {
            _spriteRenderer.sprite = _pressed ? _pressedSprite : _normalSprite;
        }
    }
}