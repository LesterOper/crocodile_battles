using App.Dialogs.BattlePass;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.EndGameDialog
{
    public class BattlePassNotification : MonoBehaviour
    {
        [SerializeField] private Image notify;
        [Inject] private BattlePassPrizeContainer _container;
        [Inject] private SignalBus _signalBus;
        [Inject] private BattlePassUtility _battlePassUtility;
        private Sequence _sequence;

        private void OnEnable()
        {
            _signalBus.Subscribe<BattlePassNotificationSignal>(UpdateNotify);
        }

        public void Init()
        {
            CheckNotify();
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<BattlePassNotificationSignal>(UpdateNotify);
        }

        private void UpdateNotify(BattlePassNotificationSignal signal)
        {
            notify.gameObject.SetActive(false);
            CheckNotify();
        }

        private void CheckNotify()
        {
            int prizesCount = _container.GetPrizeItemList().Count;
            bool isBattlePassBought = _battlePassUtility.IsBattlePassBought;
            
            for (int i = 0; i < prizesCount; i++)
            {
                PrizeItems prizeItem = _container.GetPrizeItemList()[i];
                bool isStepCompleted = _battlePassUtility.GetBattlePassStepComplete(i);
                if(isStepCompleted == false) break;
                
                if (prizeItem.IsBoss || IsPrizesClaimed(i, isBattlePassBought, prizeItem) ) continue;
                
                ShowNotify();
                return;
            }
            
            HideNotify();
        }

        private bool IsPrizesClaimed(int index, bool isBattlePassBought, PrizeItems prizeItems)
        {
            if (prizeItems.NeedDefaultPrize && _battlePassUtility.IsPrizeClaimed(index) == false) return false;
            if (isBattlePassBought && _battlePassUtility.IsPremPrizeClaimed(index) == false) return false;
            return true;
        }

        private void ShowNotify()
        {
            notify.gameObject.SetActive(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(notify.DOFade(1, 0.5f));
            _sequence.Append(notify.DOFade(0f, 0.5f));
            _sequence.OnKill(() => notify.DOFade(0, 0));
            _sequence.SetLoops(-1, LoopType.Restart);
        }

        private void HideNotify()
        {
            notify.gameObject.SetActive(false);
        }
    }

    public struct BattlePassNotificationSignal
    {
        
    }
}