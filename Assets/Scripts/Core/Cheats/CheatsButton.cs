using System;
using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cheats
{
    [RequireComponent(typeof(Button))]
    internal class CheatsButton : MonoBehaviour, ICheatsElement
    {
        [SerializeField] private TextMeshProUGUI captionLabel;

        public Button Button { get; private set; }

        public int MinPreferredWidth = 200;

        public Func<bool> IsEnabledCallback { get; set; }

        public RectTransform RectTransform { get; private set; }
        
        public void SetCaption(string caption)
        {
            captionLabel.text = caption;

            SetWidth(GetWidth());
        }
        
        public void SetWidth(float width)
        {
            RectTransform.SetWidth(width);
        }

        public float GetWidth() => Mathf.Max(Cheats.CalcTextWidth(captionLabel.text), MinPreferredWidth);
        
        private void Awake()
        {
            Button = GetComponent<Button>();
            RectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            Button.interactable = IsEnabledCallback?.Invoke() ?? true;
        }
    }
}