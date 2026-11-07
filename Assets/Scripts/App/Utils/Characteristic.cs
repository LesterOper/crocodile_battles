using TMPro;
using UnityEngine;

namespace App.Utils
{
    public class Characteristic : MonoBehaviour
    {
        [SerializeField] private bool decrease;
        [SerializeField] private bool isSignNeed;
        [SerializeField] private Color altColor = Color.red;
        private TextMeshProUGUI _value;
        private Color baseColor;

        private void Awake()
        {
            _value = GetComponent<TextMeshProUGUI>();
            baseColor = _value.color;
        }

        public void SetupCharacteristic(string value)
        {
            if (_value == null)
                _value = GetComponent<TextMeshProUGUI>();
            if (isSignNeed)
            {
                if (decrease)
                    _value.text = "-" + value;
                else
                    _value.text = "+" + value;
            }
            else
            {
                _value.text = value;
            }
        }

        public void SetColor(bool needBaseColor) => 
            _value.color = needBaseColor ? baseColor : altColor;
    }
}