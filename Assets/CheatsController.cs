using App.Counters;
using App.DAO;
using App.Dialogs.BattlePass;
using Core.Cheats;
using Library.Common.Purchases;
using UnityEngine;
using Zenject;

public class CheatsController : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [Inject] private IDataService _dataService;
    [Inject] private SignalBus _signalBus;
    [Inject] private CounterUtility _counterUtility;
    [Inject] private BattlePassUtility _battlePass;
    [Inject] private PurchaseController _purchaseController;
    private bool initialized;

    public void InitCheats()
    {
        if (!initialized)
        {
            Cheats.Initialize(container, true);
            Cheats.InitCrocoCheats(_dataService, _signalBus, _counterUtility, _battlePass, _purchaseController);
            initialized = true;
        }
        Cheats.Show();
    }
}
