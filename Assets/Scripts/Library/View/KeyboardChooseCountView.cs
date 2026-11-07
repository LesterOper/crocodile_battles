using System.Collections;
using Library.Dialogs;
using Library.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    public class KeyboardChooseCountView : ChooseCountView, Keyboard.ICallback
    {
        private Vector2 _caretInitPosition;

        [SerializeField] private GameObject _caretObject;

        private Coroutine _caretUpdateCoroutine;
        [SerializeField] private Keyboard _keyboard;
        [SerializeField] private Button _keyboardButton;

        public Keyboard Keyboard
        {
            get => _keyboard;
            set
            {
                _keyboard = value;
                if (_keyboard != null)
                    _keyboard.Callback = this;

                OnVisibilityChanged(_keyboard != null && _keyboard.Visible);
            }
        }

        public void OnNumberInput(int number)
        {
            SetCount(Count * 10 + number);
        }

        public void OnDeleteNumber()
        {
            SetCount(Count / 10);
        }

        public void OnVisibilityChanged(bool visible)
        {
            if (visible)
            {
                _caretUpdateCoroutine = StartCoroutine(BlinkCaret());
            }
            else
            {
                if (_caretUpdateCoroutine != null)
                {
                    StopCoroutine(_caretUpdateCoroutine);
                    _caretUpdateCoroutine = null;
                }

                _caretObject.SetActive(false);
            }
        }

        private void Awake()
        {
            _caretInitPosition = _caretObject.transform.localPosition;
            OnVisibilityChanged(false);
        }

        protected override void Start()
        {
            base.Start();

            if (_keyboardButton != null)
                _keyboardButton.onClick.AddListener(() => _keyboard.Show());

            if (_keyboard != null)
            {
                _keyboard.Callback = this;
                _keyboard.HideImmediately();
            }
        }

        protected override void UpdateView()
        {
            base.UpdateView();

            StartCoroutine(UpdateCaretPosition());
        }

        private IEnumerator UpdateCaretPosition()
        {
            yield return new WaitForEndOfFrame();

            var textWidth = ViewUtils.GetOneLineTextWidth(CountText.GetComponent<UnityEngine.UI.Text>());
            var sizeDeltaX = CountText.GetComponent<RectTransform>().sizeDelta.x;
            var x = Mathf.Min(textWidth, sizeDeltaX) / 2f *
                    CountText.transform.localScale.x;

            _caretObject.transform.localPosition = new Vector3(x, _caretInitPosition.y);
        }

        private IEnumerator BlinkCaret()
        {
            while (true)
            {
                _caretObject.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                _caretObject.SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}