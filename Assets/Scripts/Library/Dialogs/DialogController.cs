using System.Collections.Generic;
using System.Linq;
using App.SetupsScript;
using UnityEngine;
using Zenject;

namespace Library.Dialogs
{
    public class DialogController
    {
        public delegate void CountChangeHandler(int count);

        private readonly List<GameObject> _activeDialogs = new List<GameObject>();
        private readonly RectTransform _containerTransform;
        
        
        //private ButtonSound _soundScript;

        public DialogController(RectTransform containerTransform)
        {
            _containerTransform = containerTransform;
            
        }

        public event CountChangeHandler OnCountChanged;

        public void ShowDialog(GameObject dialogObject)
        {
            var dialog = dialogObject.GetComponent<Dialog>();
            dialog
                .AddShownHandler(OnDialogShown)
                .AddHiddenHandler(OnDialogHidden)
                .Show(_containerTransform);

            var rectTransform = dialogObject.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localPosition = new Vector3(0, 0, 0);
        }

        public bool HasActiveDialog()
        {
            return _activeDialogs.Count > 0;
        }

        public List<Dialog> GetDialogs()
        {
            return _activeDialogs.Select(o => o.GetComponent<Dialog>()).ToList();
        }

        public void CloseTopDialog()
        {
            _activeDialogs[_activeDialogs.Count - 1].GetComponent<Dialog>().Hide();
        }

        public void OnDialogShown(GameObject dialog)
        {
            _activeDialogs.Add(dialog);
            OnCountChanged?.Invoke(_activeDialogs.Count);
        }

        public void OnDialogHidden(GameObject dialog)
        {
            _activeDialogs.Remove(dialog);
            OnCountChanged?.Invoke(_activeDialogs.Count);
        }
    }
}