using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.DAO;
using App.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MergeButtonNotification : MonoBehaviour
{
    [SerializeField] private Sprite canMergeSprite;
    [SerializeField] private Sprite noMergeSprite;
    [SerializeField] private Image mergeButtonIcon;
    [Inject] private IDataService _dataService;
    [Inject] private SignalBus _signalBus;
    private FilterUniqItems _filterUniqItems;
    private List<ICardInfo> allWeaponCards = new List<ICardInfo>();

    private void OnEnable()
    {
        _signalBus.Subscribe<MergeAvailableSignal>(SetNotification);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<MergeAvailableSignal>(SetNotification);
    }


    void Start()
    {
        _filterUniqItems = new FilterUniqItems();
        SetNotification();
    }

    private bool CheckMerge()
    {
        var allBath = _dataService.GetBathCollection().BathDatas;
        var allCollectionWeapons = _dataService.GetCardCollectionData().Collection;
        var allBattleWeapons = _dataService.GetBattleCardInfoData().CardInfos;
        allWeaponCards.Clear();
        allWeaponCards.AddRange(allCollectionWeapons);
        allWeaponCards.AddRange(allBattleWeapons);
        if (_filterUniqItems.Dublicates(allBath))
            return true;
        else if (_filterUniqItems.Dublicates(allWeaponCards))
            return true;
        else
            return false;
    }

    private void SetNotification()
    {
        mergeButtonIcon.sprite = CheckMerge() ? canMergeSprite : noMergeSprite;
    }
}
