using System.Collections;
using System.Collections.Generic;
using App.Achievements;
using App.Counters;
using App.DAO;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class PurchasedDialog : Dialog
{
    [SerializeField] private Image purchaseImage;
    [SerializeField] private Button okButton;
    [SerializeField] private TextMeshProUGUI purchaseAmountText;
    [SerializeField] private TextMeshProUGUI boosterTitle;
    //[Inject] private IDataService _dataService;
    //[Inject] private SignalBus _signalBus;
    [Inject] private CounterUtility _counterUtility;
    private long _boosterCost;
    private BoosterType _boosterType;
    
    
    public void Setup(Sprite purchaseIcon, long purchaseAmount)
    {
        purchaseImage.sprite = purchaseIcon;
        purchaseAmountText.text = purchaseAmount.ToString();
        okButton.onClick.AddListener(Hide);
    }

    public void SetupButton(UnityAction action)
    {
        okButton.onClick.AddListener(action);
    }
}
