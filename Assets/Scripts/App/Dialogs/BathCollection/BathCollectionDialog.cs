using System.Collections.Generic;
using System.Linq;
using App.Analytic;
using App.DAO;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Dialogs;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.BathCollection
{
    public class BathCollectionDialog : Dialog, RecycleListView.IListAdapter
    {
        [SerializeField] private ScrollRect scrollRectCollection;
        [SerializeField] private RecycleListView listViewBath;
        [SerializeField] private BathPreview bathPreview;
        [SerializeField] private TextMeshProUGUI bathLevel;
        [SerializeField] private TextMeshProUGUI bathHP;
        [SerializeField] private TextMeshProUGUI bathCount;
        [SerializeField] private ParticleSystem effect;
        [SerializeField] private Transform bathPreviewForTutor;
        public Vector2 spacingGridBath;
        //[Inject]private IDataService _dataService;
        public GameObject FirstElem;
        private List<BathInCollectionData> _currentFilteredBathCards;
        [Inject]private IGameInfoService _gameInfoService;
        [Inject] private IAnalytics _analytics;
        private BathSetup _bathSetup;

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<UpdateBathSignal>(UpdateBathInfo);
            _signalBus.TryUnsubscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.TryUnsubscribe<TutorialSendTargetSignal>(Tutorial);
        }
        
        public void Setup(BathSetup bathSetup) => _bathSetup = bathSetup;
        
        protected override void Start()
        {
            base.Start();
            
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            scrollRectCollection.verticalNormalizedPosition = 1f;
            CroupBathes();
            InitBathInfo();
            
            listViewBath.SetAdapter(this);
            listViewBath.ItemsPadding = spacingGridBath;
            listViewBath.OnItemsChanged();
            gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.MOVE_TO_BATH_COLLECTION_DIALOG)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<ShowTutorialSignal>();
            }
            _signalBus.Subscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.Subscribe<TutorialSendTargetSignal>(Tutorial);
            _signalBus.Subscribe<UpdateBathSignal>(UpdateBathInfo);
            _analytics.SendEvent(AmplitudeEvent.ui_change_bath_show);
        }
        
        private void InitBathInfo()
        {
            CardCharactaristicsUtils cardCharactaristicsUtils = new CardCharactaristicsUtils();
            BathData bath = _dataService.GetBathData();
            bathCount.text = _dataService.GetBathCollection().BathDatas.Count.ToString();
            bathLevel.text = bath.Level.ToString();
            bathHP.text = cardCharactaristicsUtils.GetBathHPValueByLevel(bath.Level, bath.CrocodileBathType).ToString();
        }
        
        private void UpdateBathInfo(UpdateBathSignal signal)
        {
            effect.Play();
            long HP = signal.BathHP; 
            int LVL = signal.BathLvl; 
            bathLevel.text = LVL.ToString();
            bathHP.text = HP.ToString();
        }
        
        private void Tutorial(TutorialSendTargetSignal signal)
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();

            switch (tutorialStepType)
            {
                case TutorialStepType.FIRST_BATH_POINTER:
                {
                    _signalBus.Fire(new TutorialPointerSignal() {PointerTarget = FirstElem.transform, PointerEndTarget = bathPreviewForTutor, IsMergeTutorial = true});
                    break;
                }

                case TutorialStepType.CLOSE_BATH_COLLECTION_DIALOG_POINTER_HELP:
                {
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = _closeButton.transform});
                    break;
                }
            }
        }

        private void CroupBathes()
        {
            _currentFilteredBathCards = new List<BathInCollectionData>(_dataService.GetBathCollection().BathDatas);
            _currentFilteredBathCards.Sort(delegate(BathInCollectionData x, BathInCollectionData y)
            {
                if (x.CrocodileBathType == BathType.NONE && y.CrocodileBathType == BathType.NONE) return 0;
                else if (x.CrocodileBathType == BathType.NONE) return -1;
                else if (y.CrocodileBathType == BathType.NONE) return 1;
                else return x.CrocodileBathType.CompareTo(y.CrocodileBathType);
            });
            List<BathInCollectionData> tempFilteredBathData = new List<BathInCollectionData>(_currentFilteredBathCards);
            List<BathInCollectionData> singlesCards = new List<BathInCollectionData>();
            _currentFilteredBathCards.Clear();
            foreach (var card in tempFilteredBathData)
            {
                if (_currentFilteredBathCards.Contains(card) || singlesCards.Contains(card) ) 
                    continue;
                List<BathInCollectionData> tempList =
                    tempFilteredBathData.FindAll(c => c.CrocodileBathType == card.CrocodileBathType);
                if (tempList.Count <= 1)
                {
                    singlesCards.AddRange(tempList);
                    continue;
                }
                tempList.Sort(delegate(BathInCollectionData x, BathInCollectionData y)
                {
                    if (x.Level == 0 && y.Level == 0) return 0;
                    else if (x.Level == 0) return -1;
                    else if (y.Level == 0) return 1;
                    else return x.Level.CompareTo(y.Level);
                });
                tempList.Reverse();
                foreach (var cardByLevel in tempList)
                {
                    var tempListByLevel = tempList.FindAll(c => c.Level == cardByLevel.Level);
                    if (tempListByLevel.Count <= 1)
                    {
                        singlesCards.AddRange(tempListByLevel);
                        continue;
                    }
                }
                var withoutSinglesList = tempList.Except(singlesCards);
                _currentFilteredBathCards.AddRange(withoutSinglesList);
                tempList.Clear();
            }
            _currentFilteredBathCards.Reverse();
            _currentFilteredBathCards.AddRange(singlesCards);
            tempFilteredBathData.Clear();
        }


        private void UpdateItems()
        {
            CroupBathes();
            listViewBath.OnItemsChanged();
        } 
        

        public override void Hide()
        {
            if(IsTutorialInProcessing())
                return;
            _gameInfoService.ClearBaths();
            _signalBus.Fire<NewCardsSignal>();
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.CLOSE_BATH_COLLECTION_DIALOG_POINTER_HELP)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _dataService.SetExtensionUnlock(Extension.BATH_COLLECTION_BUTTON, true);
                _signalBus.Fire<TutorialSendTargetSignal>();
            }
            _dataService.UpdateBathData();
            _dataService.UpdateBathCollection();
            base.Hide();
        }
        
        private bool IsTutorialInProcessing() => !_dataService.GetTutorialStepComplete(TutorialStepType.FIRST_BATH_POINTER);
        public int GetItemsCount() => _currentFilteredBathCards.Count;
        public int GetViewTypeNumber(int position) => 0;
        public void SetupView(int position, int type, GameObject itemObject)
        {
            if (position == 0) FirstElem = itemObject;
            ICardInfo bathData = _currentFilteredBathCards[position];
            Card bathCardView = itemObject.GetComponent<Card>();
            itemObject.transform.GetChild(0).gameObject.SetActive(true);
            bathCardView.Setup(bathData);
            bathCardView.SetDrag(true, bathPreview.transform, UpdateItems);
            bathCardView.BathCardHandlerProperty.SetBathDrag(bathData, bathPreview.transform, false);
            bathCardView.ActivateNewCardIcon();
            itemObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            itemObject.GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
