using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassRecycleListItem : MonoBehaviour
    {
        [SerializeField] private BattlePassBossPrizeView bossPrizeView;
        [SerializeField] private BattlePassPrizeView defaultPrizeView;
        [SerializeField] private BattlePassPrizeView vipPrizeView;
        [SerializeField] private BattlePassPointHandler pointHandler;
        [SerializeField] private BattlePassProgressBar progressBar;
        [SerializeField] private Image _background;
        [Inject] private BattlePassPrizeContainer _container;
        [Inject] private BattlePassUtility _battlePassUtility;

        private int _prizeIndex;
        
        public void Setup(PrizeItems prizeItem, int prizeIndex)
        {
            _prizeIndex = prizeIndex;
            if(prizeItem.IsBoss)
                SetupBoss(prizeItem);
            else
                SetupPrize(prizeItem);
            
            ActivateExactView(prizeItem.IsBoss, prizeItem.NeedDefaultPrize);
            if(_battlePassUtility.GetBattlePassStepProgress(_prizeIndex) >= _container.GetPrizeItemList()[prizeIndex].NeedBattles)
                _battlePassUtility.SetBattlePassStepComplete(_prizeIndex);
            _background.gameObject.SetActive(_battlePassUtility.GetBattlePassStepComplete(_prizeIndex));
            ActivatePrizes();
            progressBar.Setup(prizeIndex, _battlePassUtility, _container);
            RecalculateRect();
        }
        
        private void RecalculateRect()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
        }

        private void ActivatePrizes()
        {
            bool complete = _battlePassUtility.GetBattlePassStepComplete(_prizeIndex);
            bool claimed = _battlePassUtility.IsPrizeClaimed(_prizeIndex);
            pointHandler.UpdateView();
            bossPrizeView.ChangeOpenState(complete && claimed == false);
        }

        private void ActivateExactView(bool isBoss, bool isNeedDefaultPrize)
        {
            bossPrizeView.gameObject.SetActive(isBoss);
            defaultPrizeView.SetVisibility(!isBoss && isNeedDefaultPrize);
            vipPrizeView.SetVisibility(!isBoss);
            
            /*if(!isNeedDefaultPrize)
                _battlePassUtility.SetPrizeClaimed(_prizeIndex);*/
        }

        private void SetupBoss(PrizeItems prizeItem)
        {
            bossPrizeView.Setup(prizeItem);
        }

        private void SetupPrize(PrizeItems prizeItem)
        {
            defaultPrizeView.Setup(prizeItem.DefaultPrize, _prizeIndex, false, _battlePassUtility);
            vipPrizeView.Setup(prizeItem.VipPrize, _prizeIndex, true, _battlePassUtility);
            pointHandler.Setup(_prizeIndex, _battlePassUtility);
        }
    }
}