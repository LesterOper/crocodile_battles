using System.Collections.Generic;
using System.Linq;
using App.Analytic;
using App.Chests;
using App.DAO;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs
{
    public class ChestsContainerDialog : Dialog
    {
        [SerializeField] private List<ChestInformationView> chests;
        [SerializeField] private CanvasGroup _canvasGroup;
        private ChestSetup _setup;
        [SerializeField] private Transform targetForTutor;
        //private IDataService _dataService;
        private DialogsContainer _dialogsContainer;
        private IAnalytics _analytics;
        [Inject]
        public void Construct(DialogsContainer  dialogsContainer, IDataService dataService, IAnalytics analytics)
        {
            _dialogsContainer = dialogsContainer;
            _dataService = dataService;
            _analytics = analytics;
        }

        private void PlaceChestForTutor()
        {
            var chest = chests.Find(chestToPlace => chestToPlace.type == ChestType.MINI);
            chest.openButton.onClick.Invoke();
        }

        public void SetChestSetup(ChestSetup chestSetup)
        {
            _setup = chestSetup;
        }
        
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<ChestOnLocationTutorSignal>(PlaceChestForTutor);
            foreach (var chest in chests)
            {
                chest.openButton.onClick.RemoveAllListeners();
            }
        }

        protected override void Start()
        {
            base.Start();
            _signalBus.Subscribe<ChestOnLocationTutorSignal>(PlaceChestForTutor);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.OPEN_CHEST_POINTER_HELP)
            {
                _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = targetForTutor});
            }
            
            foreach (var chest in chests)
            {
                if (_dataService.GetChestCollectionData().ChestCollection.Find(local => local.type == chest.type)
                        .chestAmount > 0)
                {
                    SetChestInfo(chest);
                    SetChestTextTrophiesCount(chest);
                }
                else chest.chestParent.SetActive(false);
            }

            _canvasGroup.DOFade(1, 0.2f);
            gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
        }
        private void SetChestTextTrophiesCount(ChestInformationView informationView)
        {
            informationView.trophiesInfoText.text = _setup.GetChestCharacteristic(informationView.type).trophyCount.ToString() + " " +
                                               informationView.trophiesInfoText.text;
        }
        
        private void SetChestInfo(ChestInformationView view)
        {
            var chestInfo = _dataService.GetChestCollectionData().ChestCollection.Find(info => info.type == view.type);
            SlotInfo slotInfo = _dataService.GetChestSlotsData().Slots
                .Find(slot => (int)slot.ChestSlot == _dataService.GetLong("clicked_slot"));
            int chestAmount;
            bool onLocation;
            if (chestInfo == null)
            {
                chestAmount = 0;
                onLocation = false;
                chestInfo = new ChestInfo();
            }
            else
            {
                chestAmount = chestInfo.chestAmount;
                onLocation = chestInfo.onLocation;
            }
            view.chestAmountText.text =chestAmount.ToString();

            if (view.type == ChestType.GIGA)
            {
                view.BuyGigaButton.onClick.AddListener(() =>
                {
                    chestInfo.chestAmount += 1;
                    _dataService.UpdateChestCollectionData();
                    chestAmount = chestInfo.chestAmount;
                    view.chestAmountText.text =chestAmount.ToString();
                    UpdateChestCounterIcon(view, chestAmount);
                });
            }
            
            view.openButton.onClick.AddListener(() =>
            {
                if (chestAmount > 0)
                {
                    int slotToPlace = 0;
                    if (slotInfo.Empty)
                    {
                        slotToPlace = (int)_dataService.GetLong("clicked_slot");
                    }
                    else
                    {
                        try
                        {
                            slotToPlace = (int)_dataService.GetChestSlotsData().Slots.First(slot => slot.Empty).ChestSlot;
                        }
                        catch 
                        {
                            Hide();
                            return;
                        }
                    }
                    chestInfo.chestAmount -= 1;
                    chestInfo.onLocation = true;
                    _dataService.UpdateChestCollectionData();
                    
                    _dataService.GetChestSlotsData().Slots.Find(slot => (int) slot.ChestSlot == slotToPlace).Empty =
                        false;
                    _dataService.GetChestSlotsData().Slots.Find(slot => (int) slot.ChestSlot == slotToPlace).ChestType =
                        view.type;
                    _dataService.UpdateChestSlotsData();
                    
                    _signalBus.Fire(new ActivateChestSignal() {SlotToPlace = slotToPlace, ChestType = view.type});

                    _analytics.SendEvent(AmplitudeEvent.chest_start, 
                        new Dictionary<string, object>(){{AmplitudePlacement.type.ToString(), chestInfo.type }});
                    Hide();
                }
            });
            UpdateChestCounterIcon(view, chestAmount);
        }

        private void UpdateChestCounterIcon(ChestInformationView view ,int chestAmount)
        {
            if (chestAmount>0)
            {
                view.activeIcon.gameObject.SetActive(true);
                view.notActiveIcon.gameObject.SetActive(false);
                view.openButton.interactable = true;
                view.chestView.DOFade(1, 0.2f);
            }
            else
            {
                view.activeIcon.gameObject.SetActive(false);
                view.notActiveIcon.gameObject.SetActive(true);
                view.openButton.interactable = false;
                view.chestView.DOFade(0.4f, 0.2f);
            }
        }

    }

    [System.Serializable]
    public class ChestInformationView
    {
        public ChestType type;
        public ChestHandler chestHandler;
        public TextMeshProUGUI chestAmountText;
        public TextMeshProUGUI trophiesInfoText;
        public CanvasGroup chestView;
        public Button openButton;
        public Button BuyGigaButton;
        public Image notActiveIcon;
        public Image activeIcon;
        public GameObject chestParent;
    }
    
    
}