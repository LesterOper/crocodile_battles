using System;
using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cheats
{
    internal class CheatsSlider : MonoBehaviour, ICheatsElement
    {
        [SerializeField] private TextMeshProUGUI captionLabel;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueText;

        public int MinimalSliderWidth = 400;
        
        public RectTransform RectTransform { get; private set; }
        
        public event Action<float> ValueChanged;

        public void SetCaption(string caption)
        {
            captionLabel.text = caption;
            
            SetWidth(GetWidth());
        }
        
        public void SetWidth(float width)
        {
            RectTransform.SetWidth(width);
        }

        public float GetWidth()
            => Cheats.CalcTextWidth(captionLabel.text) + Cheats.CalcTextWidth(5) + MinimalSliderWidth;

        public void Configure(float min, float max, float initialValue, bool onlyWhole)
        {
            slider.wholeNumbers = onlyWhole;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = initialValue;
        }

        private void OnValueChanged(float newValue)
        {
            valueText.text = slider.wholeNumbers ? $"{newValue:0.}" :$"{newValue:0.00}";
            
            ValueChanged?.Invoke(newValue);
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            
            slider.onValueChanged.AddListener(OnValueChanged);
        }
    }
}