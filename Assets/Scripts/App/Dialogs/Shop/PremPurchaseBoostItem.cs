using TMPro;
using UnityEngine;

namespace App.Dialogs.Shop
{
    public class PremPurchaseBoostItem : PremPurchaseItem
    {
        [SerializeField] private TextMeshProUGUI[] _amountTexts;
        [SerializeField] private string _amountPrefix;

        protected override void UpdateView()
        {
            base.UpdateView();
            foreach (TextMeshProUGUI text in _amountTexts) 
                text.SetText($"{_amountPrefix}{_purchase.Amount}");
        }
    }
}