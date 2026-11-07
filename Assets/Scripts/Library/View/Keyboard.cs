using System;
using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    [RequireComponent(typeof(GameMovingObject))]
    public class Keyboard : MonoBehaviour
    {
        [SerializeField] private Button _delButton;
        [SerializeField] private Vector2 _hiddenPosition;
        [SerializeField] private Button _hideButton;

        private GameMovingObject _movingObject;
        [SerializeField] private Button[] _numberButtons;

        [SerializeField] private Vector2 _visiblePosition;

        public ICallback Callback { get; set; }

        public bool Visible { get; private set; }

        private void Awake()
        {
            _visiblePosition = transform.position;
            _movingObject = GetComponent<GameMovingObject>();

            Visible = true;
        }

        private void Start()
        {
            for (var i = 0; i < _numberButtons.Length; i++)
            {
                var number = i;
                _numberButtons[i].onClick.AddListener(() => InputNumber(number));
            }

            _delButton.onClick.AddListener(DeleteNumber);
            _hideButton.onClick.AddListener(Hide);
        }

        public void InputNumber(int number)
        {
            Callback.OnNumberInput(number);
        }

        public void DeleteNumber()
        {
            Callback.OnDeleteNumber();
        }

        public void Hide()
        {
            Hide(Math.Abs(transform.position.x - _hiddenPosition.x) / 40);
        }

        public void HideImmediately()
        {
            Hide(0);
        }

        private void Hide(float duration)
        {
            if (Visible)
            {
                Visible = false;
                _movingObject.Duration = duration;
                _movingObject.Move(transform.position, _hiddenPosition, null);
                Callback?.OnVisibilityChanged(false);
            }
        }

        public void Show()
        {
            if (!Visible)
            {
                Visible = true;
                _movingObject.Duration = Math.Abs(transform.position.x - _visiblePosition.x) / 40;
                _movingObject.Move(transform.position, _visiblePosition, null);
                Callback?.OnVisibilityChanged(true);
            }
        }

        public interface ICallback
        {
            void OnNumberInput(int number);
            void OnDeleteNumber();
            void OnVisibilityChanged(bool visible);
        }
    }
}