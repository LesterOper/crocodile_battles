using System;
using App.Achievements;
using App.Counters;
using App.DAO;
using App.Signals;
using I2.Loc;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BuyBoosterDialog : Dialog
{
    [SerializeField] private Image boosterIcon;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI boosterCost;
    [SerializeField] private TextMeshProUGUI boosterTitle;
    //[Inject] private IDataService _dataService;
    //[Inject] private SignalBus _signalBus;
    [Inject] private CounterUtility _counterUtility;
    private long _boosterCost;
    private BoosterType _boosterType;

    private void OnDisable()
    {
        buyButton.onClick.RemoveAllListeners();
    }

    public void Setup(BoosterInfoSetup boosterInfo)
    {
        _boosterType = boosterInfo.BoosterType;
        boosterIcon.sprite = boosterInfo.BoosterIcon;
        _boosterCost = boosterInfo.BoosterCost;
        boosterCost.text = _boosterCost.ToString();
        boosterTitle.text = LocalizationManager.GetTranslation(boosterInfo.BoosterTitle);
        buyButton.onClick.AddListener(BuyBooster);
    }

    private void BuyBooster()
    {
        if(_counterUtility.GetMoney(CounterType.DEFAULT_MONEY) < _boosterCost)
            return;
        _dataService.GetBoosterCollectionData().BoosterCollection.Find(booster => booster.BoosterType == _boosterType)
            .BoosterAmount++;
        _dataService.UpdateBoosterCollectionData();
        _counterUtility.AppendMoney(CounterType.DEFAULT_MONEY, -_boosterCost);
        _signalBus.Fire(new BoosterAmountChanged(){BoosterType = _boosterType});
    }
}