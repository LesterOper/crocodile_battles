using System;
using System.Collections.Generic;
using Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cheats
{
    internal class CheatsLabel : MonoBehaviour, ICheatsElement
    {
        public int MinPreferredWidth = 250;

        [SerializeField] private TextMeshProUGUI captionLabel;

        [SerializeField] private List<Image> bounds = new List<Image>();

        public Func<string> UpdateCallback { get; set; }
        
        public RectTransform RectTransform { get; private set; }

        public event Action CaptionUpdated;

        public void SetBoundsVisible(bool visible)
        {
            for (int i = 0; i < bounds.Count; ++i)
            {
                bounds[i].gameObject.SetActive(visible);
            }
        }

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
            RectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (UpdateCallback == null) return;
            
            var newCaption = UpdateCallback();
            
            if (captionLabel.text == newCaption) return;
            
            SetCaption(newCaption);
                
            CaptionUpdated?.Invoke();
        }
    }
}