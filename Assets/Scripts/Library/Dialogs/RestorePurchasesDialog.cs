using UnityEngine;
using UnityEngine.UI;

namespace Library.Dialogs
{
    public class RestorePurchasesDialog : Dialog
    {
        [SerializeField] private Button okButton;

        protected override void Start()
        {
            base.Start();
            okButton.onClick.AddListener(Hide);
        }
    }
}