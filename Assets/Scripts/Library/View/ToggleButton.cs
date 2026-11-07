using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    [RequireComponent(typeof(Button))]
    public abstract class ToggleButton : MonoBehaviour
    {
        public delegate void CheckChangedHandler(bool isChecked);

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                UpdateState();
                OnCheckChanged?.Invoke(_isChecked);
            }
        }

        public event CheckChangedHandler OnCheckChanged;

        protected abstract void UpdateState();

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => IsChecked = !_isChecked);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}