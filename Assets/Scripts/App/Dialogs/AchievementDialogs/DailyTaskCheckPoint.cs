using System;
using System.Collections;
using System.Collections.Generic;
using App.Achievements;
using App.Chests;
using App.DAO;
using App.Signals;
using Library;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using Zenject;

public class DailyTaskCheckPoint : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image readyEffect;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image notification;

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite notReadySprite;
    [SerializeField] private Image emptyBar;
    [SerializeField] private Image fullBar;

    [SerializeField] private bool isFinalCheckPoint;

    [Inject] private DialogsContainer _dialogContainer;
    [Inject] private SignalBus _signalBus;
    private IDataService _dataService;
    private TaskPrizeInformation _prizeInfo;
    private long _dailyTaskCompletedDays;
    public void Setup(bool isCompleted, int taskCompleted, IDataService dataService, TaskPrizeInformation prizeInformation, long dailyTaskCompletedDays)
    {
        _dataService = dataService;
        _prizeInfo = prizeInformation;
        _dailyTaskCompletedDays = dailyTaskCompletedDays;
        if (isFinalCheckPoint)
        {
            SetupFinalPrize(isCompleted);
            return;
        }
        icon.sprite = isCompleted ? readySprite : notReadySprite;
        number.color = isCompleted ? new Color(0.25f, 0.5f, 0.41f, 1) : Color.black;
        dayText.color = isCompleted ? new Color(0.25f, 0.5f, 0.41f, 1) : Color.black;
        notification.gameObject.SetActive(isCompleted);
        fullBar.fillAmount = (float)taskCompleted / 3;

    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("FinalPrizeIsReady", 0) == 1 && PlayerPrefs.GetInt("FinalPrizeClaimed", 0) == 0 && isFinalCheckPoint)
        {
            SetupFinalPrize(true);
        }
    }

    private void SetupFinalPrize(bool isCompleted)
    {
        if(isCompleted)
            PlayerPrefs.SetInt("FinalPrizeIsReady",1);
        //icon.sprite = _prizeInfo.GetPrizeIcon();
        readyEffect.gameObject.SetActive(isCompleted);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isFinalCheckPoint)
            return;
        if(PlayerPrefs.GetInt("FinalPrizeIsReady") == 0)
            return;
        
        ClaimChest(PrizeType.GIGA_CHEST, _prizeInfo.GetPrizeAmout());
        var dialog = _dialogContainer.ShowPurchasingNotification();
        dialog.Setup(icon.sprite,1);
        if(_dailyTaskCompletedDays == 5)
            _dataService.SetLong(Constants.DailyTasksCompletedDays, 0);
    }
    
    private void ClaimChest(PrizeType prizeType, int amount)
    {
        ChestType chestType = ChestType.NONE;
        switch (prizeType)
        {
            case PrizeType.SMALL_CHEST:
                chestType = ChestType.MINI;
                break;
            case PrizeType.MEDIUM_CHEST:
                chestType = ChestType.MEDIUM;
                break;
            case PrizeType.MEGA_CHEST:
                chestType = ChestType.MEGA;
                break;
            case PrizeType.GIGA_CHEST:
                chestType = ChestType.GIGA;
                break;
        }
        var chestInfo = _dataService.GetChestCollectionData().ChestCollection.Find(chest => chest.type == chestType);
        chestInfo.chestAmount += amount;
        
        _dataService.UpdateChestCollectionData();
        _signalBus.Fire<UpdateChestSlotsSignal>();
        PlayerPrefs.SetInt("FinalPrizeIsReady",0);
        PlayerPrefs.SetInt("FinalPrizeClaimed", 1);
    }
}
