using App.Counters;
using CodeBase.Infrastructure;
using Library;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs
{
    public class PassiveDialog : Dialog
    {
        [SerializeField] private TextMeshProUGUI defeatedCount;
        [SerializeField] private TextMeshProUGUI winCount;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Image defeatingBack; 
        [SerializeField] private Image winBack;
        [SerializeField] private Sprite highLight;
        [SerializeField] private Sprite noHighLight;
        [SerializeField] private Button OK;

        private CounterUtility _utility;
        private float moneyValue;
    
        protected override void Start()
        {
            base.Start();
            _utility = new CounterUtility();
            OK.onClick.AddListener(Hide);
            CountMoney();
            SetResults();
        }

        private void CountMoney()
        {
            moneyValue = Random.Range(35, 50);
            moneyText.text = "+" + moneyValue;
        }

        private void SetResults()
        {
            int winValue = Random.Range(3, 8);
            int looseValue = Random.Range(1, 5);
            defeatedCount.text = looseValue.ToString();
            winCount.text = winValue.ToString();
        
            defeatingBack.sprite = winValue > looseValue ? noHighLight : highLight;
            winBack.sprite = winValue > looseValue ? highLight : noHighLight;
            _utility.AppendMoney(CounterType.BATTLES_WIN, winValue);
            _utility.AppendMoney(CounterType.BATTLES_LOOSE, looseValue);
        }

        public override void Hide()
        {
            base.Hide();
            _utility.AppendMoney(CounterType.DEFAULT_MONEY, moneyValue);
            //PassiveIncomeUtils.SetLastSession();
        }
    }
}