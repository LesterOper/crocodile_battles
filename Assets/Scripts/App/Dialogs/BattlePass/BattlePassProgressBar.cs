using App.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.BattlePass
{
    public class BattlePassProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _leftBarFilling;
        [SerializeField] private Image _rightBarFilling;
        private int _index;
        private BattlePassUtility _passUtility;
        private BattlePassPrizeContainer _prizeContainer;

        public void Setup(int index, BattlePassUtility passUtility, BattlePassPrizeContainer prizeContainer)
        {
            _index = index;
            _passUtility = passUtility;
            _prizeContainer = prizeContainer;
            SetBars();
        }

        private void SetBars()
        {
            if ( _passUtility.GetBattlePassStepComplete(_index + 1) )
            {
                _leftBarFilling.fillAmount = 1;
                _rightBarFilling.fillAmount = 1;
            }
            else if ( _passUtility.GetBattlePassStepComplete(_index) )
            {
                _leftBarFilling.fillAmount = 1;
                CalcRightBar();
            }
            else if ( _passUtility.GetBattlePassStepComplete(_index - 1) )
            {
                _rightBarFilling.fillAmount = 0;
                CalcLeftBar();
            }
            else
            {
                _rightBarFilling.fillAmount = 0;
                _leftBarFilling.fillAmount = 0;   
            }
            
            if ( _index == 0 ) _leftBarFilling.fillAmount = 0;

            if ( _prizeContainer.Count - 1 == _index ) _rightBarFilling.fillAmount = 0;

        }

        private void CalcLeftBar()
        {
            int needBattles = _prizeContainer.GetPrizeItemList()[_index].NeedBattles;
            long hasBattles = _passUtility.GetBattlePassStepProgress(_index);
            _leftBarFilling.fillAmount = (float) MathA.Map((float)hasBattles / needBattles, 0.5f, 1f, 0f, 1f);
        }

        private void CalcRightBar()
        {
            int needBattles = _prizeContainer.GetPrizeItemList()[_index + 1].NeedBattles;
            long hasBattles = _passUtility.GetBattlePassStepProgress(_index + 1);
            _rightBarFilling.fillAmount = (float) MathA.Map((float) hasBattles / needBattles, 0f, 0.5f, 0f, 1f);
        }
    }
}