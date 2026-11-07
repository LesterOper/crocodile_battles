using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace Library.Dialogs
{
    public class Toast : MonoBehaviour
    {
        [SerializeField] private Button _closeArea;
        [SerializeField] private Localize _messageText;
        [SerializeField] private Localize _titleText;

        public void Show(RectTransform parentTransform, string textKey, string titleKey)
        {
            _closeArea.onClick.AddListener(Hide);
            _messageText.SetTerm(textKey);
            _titleText.SetTerm(titleKey);

            GetComponent<RectTransform>().SetParent(parentTransform);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localPosition = new Vector3(0, 0, 0);
            gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay());
        }

        public void Show(RectTransform parentTransform, string textKey, string titleKey, params object[] objects)
        {
            _closeArea.onClick.AddListener(Hide);
            // _messageText.SetTerm(textKey, objects);
            _titleText.SetTerm(titleKey);
            GetComponent<RectTransform>().SetParent(parentTransform);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localPosition = new Vector3(0, 0, 0);
            gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private IEnumerator<WaitForSeconds> HideAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            Hide();
        }
    }
}