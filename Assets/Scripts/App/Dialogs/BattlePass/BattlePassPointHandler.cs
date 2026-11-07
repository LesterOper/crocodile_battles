using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.BattlePass
{
    public class BattlePassPointHandler : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountTextView;
        [SerializeField] private Sprite openClaimedPoint;
        [SerializeField] private Sprite nonOpen;
        [SerializeField] private Color openClaimedColor;
        [SerializeField] private Color nonOpenColor;
        private int _itemIndex;
        private BattlePassUtility _passUtils;

        public void Setup(int itemIndex, BattlePassUtility passUtility)
        {
            _itemIndex = itemIndex;
            _passUtils = passUtility;
            amountTextView.text = (itemIndex + 1).ToString();
        }

        public void UpdateView()
        {
            if (_passUtils == null) return;
            bool isComplete = _passUtils.GetBattlePassStepComplete(_itemIndex);
            icon.sprite = isComplete ? openClaimedPoint : nonOpen;
            amountTextView.color = isComplete ? openClaimedColor : nonOpenColor;
        }
    }
}