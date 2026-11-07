using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    public class ItemsSlider : MonoBehaviour
    {
        public delegate void ExpandChangeHandler(bool expanded);

        [SerializeField] private float _animDuration = 0.2f;
        [SerializeField] private Button _button;

        private Vector2 _initPosition;

        [SerializeField] private RectTransform[] _items;

        private float _progress;
        private List<Vector2> _visiblePositions;

        public bool Expanded { get; private set; }

        public event ExpandChangeHandler OnExpandChanged;

        private void Start()
        {
            _initPosition = _button.GetComponent<RectTransform>().anchoredPosition;

            _visiblePositions = new List<Vector2>();

            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                _visiblePositions.Add(item.anchoredPosition);
                item.anchoredPosition = _initPosition;
            }

            _progress = 0;
            Expanded = false;

            _button.onClick.AddListener(() => SetExpanded(!Expanded, true));
        }

        private void Update()
        {
            if (_progress > 0 && !Expanded)
            {
                _progress = Mathf.Clamp01(_progress - Time.deltaTime / _animDuration);
                UpdateExpandPositions();
            }
            else if (_progress < 1 && Expanded)
            {
                _progress = Mathf.Clamp01(_progress + Time.deltaTime / _animDuration);
                UpdateExpandPositions();
            }
        }

        private void UpdateExpandPositions()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                item.anchoredPosition = Vector2.Lerp(_initPosition, _visiblePositions[i], _progress);
            }
        }

        public void SetExpanded(bool expanded, bool animated)
        {
            if (expanded == Expanded)
                return;

            Expanded = expanded;

            if (!animated)
            {
                _progress = expanded ? 1 : 0;
                UpdateExpandPositions();
            }

            OnExpandChanged?.Invoke(Expanded);
        }
    }
}