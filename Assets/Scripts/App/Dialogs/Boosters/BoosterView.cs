using System;
using System.Linq;
using App.Achievements;
using App.Counters;
using App.DAO;
using App.Signals;
using DG.Tweening;
using I2.Loc;
using Library.Dialogs;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BoosterView : MonoBehaviour
{
    [SerializeField] private BoosterSetup boosterSetup;
    [SerializeField] private Button boosterIcon;
    [SerializeField] private Image boosterChosen;
    [SerializeField] private Image buyBooster;
    [SerializeField] private TextMeshProUGUI boostersAmount;
    [SerializeField] private TextMeshProUGUI boosterTitle;
    [SerializeField] private Image popup;
    [SerializeField] private TextMeshProUGUI availabilityText;
    [Inject] private IDataService _dataService;
    [Inject] private SignalBus _signalBus;
    [Inject] private DialogsContainer _dialogsContainer;
    private Transform _contentPos;
    private Transform _boosterParent;
    private BoosterInfo _boosterInfo;
    private BoosterState _boosterState;
    private BoosterInfoSetup _boosterInfoSetup;
    private CounterUtility _counterUtility;
    private Tween popupTween;
    private void Start()
    {
        _contentPos = gameObject.GetComponentInParent<RecycleListView>(true)?.transform;
        _boosterParent = transform.parent;
        _signalBus.Subscribe<BoosterAmountChanged>(UpdateBoosters);
        foreach (var booster in _dataService.GetBoosterCollectionData().BoosterCollection)
        {
            booster.isChosen = false;
        }
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<BoosterAmountChanged>(UpdateBoosters);
        boosterIcon.onClick.RemoveAllListeners();
    }

    private void UpdateBoosters(BoosterAmountChanged signal)
    {
        if (signal.BoosterType == _boosterInfo.BoosterType) 
            SetBoosterState(BoosterState.BUSTER_IN_HAND);
        boostersAmount.text = _boosterInfo.BoosterAmount.ToString();
    }

    public void Setup(BoosterType boosterType)
    {
        _counterUtility = new CounterUtility(_dataService, _signalBus);
        _boosterInfoSetup = boosterSetup.GetBoosterInfoByType(boosterType);
        _boosterInfo = _dataService.GetBoosterCollectionData().BoosterCollection
            .Find(booster => booster.BoosterType == boosterType);
        popup.gameObject.SetActive(_counterUtility.GetMoney(CounterType.BATTLES_WIN)+_counterUtility.GetMoney(CounterType.BOSS_BATTLES_WIN) <= boosterSetup.WinsToUnlockBoosters);
        boosterTitle.text = LocalizationManager.GetTranslation(_boosterInfoSetup.BoosterTitle);
        boosterIcon.image.sprite = _boosterInfoSetup.BoosterIcon;
        boosterIcon.image.DOFade(_counterUtility.GetMoney(CounterType.BATTLES_WIN)+_counterUtility.GetMoney(CounterType.BOSS_BATTLES_WIN) > boosterSetup.WinsToUnlockBoosters
            ? 1
            : 0.5f, 0);
        availabilityText.text = LocalizationManager.GetTranslation("booster_available");
        SetBoosterState(_boosterInfo.BoosterAmount <= 0 ? 
            BoosterState.NO_BUSTERS : BoosterState.BUSTER_IN_HAND);
        boostersAmount.text = _boosterInfo.BoosterAmount.ToString();
        boosterIcon.interactable = _counterUtility.GetMoney(CounterType.BATTLES_WIN)+_counterUtility.GetMoney(CounterType.BOSS_BATTLES_WIN) > boosterSetup.WinsToUnlockBoosters;
        boosterIcon.onClick.AddListener(OnBusterClicked);
    }

    private void OnBusterClicked()
    {
        if (_counterUtility.GetMoney(CounterType.BATTLES_WIN)+_counterUtility.GetMoney(CounterType.BOSS_BATTLES_WIN) < boosterSetup.WinsToUnlockBoosters)
        {
            return;
        }
        switch (_boosterState)
        {
            case BoosterState.NO_BUSTERS:
                var dialog = _dialogsContainer.ShowBuyBoosterDialog();
                dialog.Setup(_boosterInfoSetup);
                break;
            case BoosterState.BUSTER_CHOSEN:
                ChooseBuster(_boosterInfo.BoosterType, false, BoosterState.BUSTER_IN_HAND);
                break;
            case BoosterState.BUSTER_IN_HAND:
                ChooseBuster(_boosterInfo.BoosterType, true, BoosterState.BUSTER_CHOSEN);
                break;
        }
    }

    private void ChooseBuster(BoosterType boosterType, bool isChoose, BoosterState state)
    {
        SetBoosterState(state);
        _dataService.GetBoosterCollectionData().BoosterCollection
            .FirstOrDefault(booster => booster.BoosterType == boosterType).isChosen = isChoose;
        _dataService.UpdateBoosterCollectionData();
    }


    private void SetBoosterState(BoosterState state)
    {
        switch (state)
        {
            case BoosterState.NONE:
                break;
            case BoosterState.NO_BUSTERS:
                _boosterState = BoosterState.NO_BUSTERS;
                boosterChosen.gameObject.SetActive(false);
                buyBooster.gameObject.SetActive(true);
                boostersAmount.gameObject.SetActive(false);
                break;
            case BoosterState.BUSTER_CHOSEN:
                _boosterState = BoosterState.BUSTER_CHOSEN;
                boosterChosen.gameObject.SetActive(true);
                buyBooster.gameObject.SetActive(false);
                boostersAmount.gameObject.SetActive(false);
                break;
            case BoosterState.BUSTER_IN_HAND:
                _boosterState = BoosterState.BUSTER_IN_HAND;
                boosterChosen.gameObject.SetActive(false);
                buyBooster.gameObject.SetActive(false);
                boostersAmount.gameObject.SetActive(true);
                break;
        }
    }
}

public enum BoosterState
{
    NONE = 0,
    NO_BUSTERS = 1,
    BUSTER_CHOSEN = 2,
    BUSTER_IN_HAND = 3
}