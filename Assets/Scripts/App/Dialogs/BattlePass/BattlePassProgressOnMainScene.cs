using App.DAO;
using App.Dialogs.EndGameDialog;
using App.Signals;
using App.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassProgressOnMainScene : MonoBehaviour
    {
        [SerializeField] private BattlePassNotification _notification;
        [SerializeField] private Image progress;
        [SerializeField] private TextMeshProUGUI prev;
        [SerializeField] private TextMeshProUGUI next;
        [SerializeField] private TextMeshProUGUI currentAndNeed;
        [SerializeField] private Image bossPreview;
        [Inject] private IDataService _dataService;
        [Inject] private BattlePassPrizeContainer _container;
        [Inject] private SignalBus _signalBus;
        [Inject] private BattlePassUtility _battlePassUtility;
        
        private void Start()
        {
            int current = (int) _battlePassUtility.GetBattlePassCurrentStep();
            int currentProgress = (int) _battlePassUtility.GetBattlePassStepProgress(current);
            int need = _container.GetPrizeItemList()[current].NeedBattles;
            if (currentProgress >= need)
            {
                if (!_dataService.GetTutorialStepComplete(TutorialStepType.BOSS_OPEN) && current >= 2)
                {
                    _signalBus.Fire(new SetTutorialStepSignal(){StepType = TutorialStepType.BOSS_OPEN});
                    _signalBus.Fire<ShowTutorialSignal>();
                }
                _battlePassUtility.SetBattlePassStepComplete(current);
                current = current + 1;
                currentProgress = (int) _battlePassUtility.GetBattlePassStepProgress(current);
                need = _container.GetPrizeItemList()[current].NeedBattles;
            }
            progress.fillAmount = currentProgress / (float)need;
            prev.text = "" + (current);
            next.text = "" + (current + 1);
            currentAndNeed.text = currentProgress + "/" + need;
            _notification.Init();
            if(_container.GetPrizeItemList()[current].IsBoss)
                bossPreview.gameObject.SetActive(true);
            if(_container.GetPrizeItemList()[Mathf.Max(0,current-1)].BossBathType == _dataService.GetCurrentBoss())
                bossPreview.gameObject.SetActive(true);
                
        }
    }
}