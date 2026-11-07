using System.Collections.Generic;
using App.Analytic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassProgressCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nextProgressLevel;
        [SerializeField] private TextMeshProUGUI currentAndNeedPoints;
        [SerializeField] private Image progressBar;
        [Inject] private IAnalytics _analytics;
        private BattlePassUtility _battlePassUtility;
        private BattlePassPrizeContainer _container;
        
        public void Setup(BattlePassUtility battlePassUtility, BattlePassPrizeContainer prizeContainer)
        {
            _battlePassUtility = battlePassUtility;
            _container = prizeContainer;
            int index = (int) _battlePassUtility.GetBattlePassCurrentStep();
            PrizeItems prizeItems = _container.GetPrizeItemList()[index];
            if (_battlePassUtility.GetBattlePassStepProgress(index) >= prizeItems.NeedBattles)
            {
                _battlePassUtility.SetBattlePassStepComplete(index);
                _analytics.SendEvent(AmplitudeEvent.battle_pass_step_end, new Dictionary<string, object>(){{"type", prizeItems.IsBoss ? "boss":"usual"},{"lvl",index}});
                _battlePassUtility.SetBattlePassCurrentStep(index + 1);
                index = index + 1;
                prizeItems = _container.GetPrizeItemList()[index];
            }
            nextProgressLevel.text = (index + 1).ToString();
            currentAndNeedPoints.text =
                _battlePassUtility.GetBattlePassStepProgress(index) + "/" + prizeItems.NeedBattles;
            progressBar.fillAmount = _battlePassUtility.GetBattlePassStepProgress(index) / (float)prizeItems.NeedBattles;
        }
    }
}