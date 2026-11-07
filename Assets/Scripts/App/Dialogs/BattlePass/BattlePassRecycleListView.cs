using App.DAO;
using App.Signals;
using App.Tutorial;
using Library.View;
using UnityEngine;
using Zenject;

namespace App.Dialogs.BattlePass
{
    public class BattlePassRecycleListView : MonoBehaviour, RecycleListView.IListAdapter
    {
        [SerializeField] private RecycleListView recycleListView;
        [Inject] private BattlePassPrizeContainer prizeContainer;
        [Inject] private IDataService _dataService;
        [Inject] private SignalBus _signalBus;

        public void Setup()
        {
            recycleListView.SetAdapter(this);
            recycleListView.OnItemsChanged();
            Transform bossView = recycleListView.GetComponentInChildren<BattlePassBossPrizeView>(false).transform;
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.BOSS_POINTER)
            {
                _signalBus.Fire(new SetTutorialStepSignal{StepType = TutorialStepType.BOSS_BUTTON_POINTER});
                _signalBus.Fire(new TutorialPointerSignal{PointerTarget = bossView});
            }
        }

        public int GetItemsCount()
        {
            return prizeContainer.GetPrizeItemList().Count;
        }

        public int GetViewTypeNumber(int position)
        {
            return 0;
        }

        public void SetupView(int position, int type, GameObject itemObject)
        {
            BattlePassRecycleListItem item = itemObject.GetComponent<BattlePassRecycleListItem>();
            item.transform.localScale = Vector3.one;
            item.Setup(prizeContainer.GetPrizeItemList()[position], position);
        }
    }
}