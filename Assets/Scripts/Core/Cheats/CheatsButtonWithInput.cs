using System;
using System.Globalization;
using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cheats
{
    internal class CheatsButtonWithInput : MonoBehaviour, ICheatsElement
    {
        public enum InputType
        {
            Integer,
            Float,
            String,
        }
        
        [SerializeField] private TextMeshProUGUI captionLabel;
        [SerializeField] private Button button;
        [SerializeField] private TMP_InputField input;
        
        public int MinPreferredWidth = 200;
        
        public RectTransform RectTransform { get; private set; }
        
        public InputType Type { get; private set; }
        
        public Func<bool> IsEnabledCallback { get; set; }

        public event Action<int> ButtonClickedInteger;

        public event Action<float> ButtonClickedFloat;

        public event Action<string> ButtonClickedString;
        
        public void SetCaption(string caption)
        {
            captionLabel.text = caption;

            SetWidth(GetWidth());
        }
        
        public void SetWidth(float width)
        {
            RectTransform.SetWidth(width);
        }

        public void SetType(InputType type)
        {
            Type = type;

            switch (type)
            {
                case InputType.Integer:
                    input.contentType = TMP_InputField.ContentType.IntegerNumber;
                    break;
                case InputType.Float:
                    input.contentType = TMP_InputField.ContentType.DecimalNumber;
                    break;
                case InputType.String:
                    input.contentType = TMP_InputField.ContentType.Standard;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public float GetWidth() => Mathf.Max(Cheats.CalcTextWidth(captionLabel.text), MinPreferredWidth);

        private void OnButtonClick()
        {
            switch (Type)
            {
                case InputType.Integer:
                    if (int.TryParse(input.text, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultI))
                    {
                        ButtonClickedInteger?.Invoke(resultI);
                    }
                    break;
                case InputType.Float:
                    if (float.TryParse(input.text, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultF))
                    {
                        ButtonClickedFloat?.Invoke(resultF);
                    }
                    break;
                case InputType.String:
                    ButtonClickedString?.Invoke(input.text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            
            button.onClick.AddListener(OnButtonClick);
        }
        
        private void Update()
        {
            button.interactable = IsEnabledCallback?.Invoke() ?? true;
        }
    }
}