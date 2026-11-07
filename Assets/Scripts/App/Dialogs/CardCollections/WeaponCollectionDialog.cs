using System.Collections.Generic;
using System.Linq;
using App.Analytic;
using App.Cards;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library;
using Library.Dialogs;
using Library.Utils;
using Library.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.CardCollections
{
    public class WeaponCollectionDialog : Dialog, RecycleListView.IListAdapter
    {
        [SerializeField] private ScrollRect scrollRectCollection;
        [SerializeField] private FilterButtonHandler allCards;
        [SerializeField] private FilterButtonHandler attackLong;
        [SerializeField] private FilterButtonHandler attackClose;
        [SerializeField] private FilterButtonHandler defence;
        [SerializeField] private FilterButtonHandler special;
        [SerializeField] private RecycleListView listViewCollection;
        [SerializeField] private RecycleListView listViewBattle;
        private WeaponSetup _weaponSetup;
        [SerializeField] private DeckController deckController;
        [SerializeField] private Button autobuildButton;
        [SerializeField] private TextMeshProUGUI cardCounText;
        public Vector2 spasingGridCards;
        //private IDataService _dataService;
        private FilterType _filterType = FilterType.NONE;
        public GameObject FirstElem;
        private List<CardInfo> _currentFilteredWeaponCards;
        private IGameInfoService _gameInfoService;
        private IAnalytics _analytics;


        [Inject]
        public void Construct(IDataService dataService, IGameInfoService gameInfoService, IAnalytics analytics)
        {
            _dataService = dataService;
            _gameInfoService = gameInfoService;
            _analytics = analytics;
        }
        
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.TryUnsubscribe<TutorialSendTargetSignal>(Tutorial);
        }

        public void SetupWeaponSetup(WeaponSetup weaponSetup)
        {
            _weaponSetup = weaponSetup;
            deckController.SetWeaponSetup(weaponSetup);
        }

        private void SetButtonListener(FilterType filterType)
        {
            _gameInfoService.ClearByFilterType(filterType);
            SetupButton(filterType, spasingGridCards);
            _signalBus.Fire<NewCardsSignal>();
        }

        protected override void Start()
        {
            base.Start();
            
            cardCounText.text = _dataService.GetCardCollectionData().Collection.Count.ToString();
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            InitFilter();
            ChangeSprite(_filterType);
            Filter();
            allCards.AddListener(()=> SetButtonListener(FilterType.ALL));
            attackLong.AddListener(()=> SetButtonListener(FilterType.LONG_DISTANCE));
            attackClose.AddListener(()=> SetButtonListener(FilterType.CLOSE_DISTANCE));
            defence.AddListener(()=> SetButtonListener(FilterType.DEFENCE));
            special.AddListener(()=> SetButtonListener(FilterType.ONE_ACTION));
            
            
            _signalBus.Subscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.Subscribe<TutorialSendTargetSignal>(Tutorial);
            listViewCollection.SetAdapter(this);
            listViewCollection.ItemsPadding = spasingGridCards;
            listViewCollection.OnItemsChanged();
            listViewBattle.SetAdapter(this);
            listViewBattle.ItemsPadding = spasingGridCards;
            listViewBattle.OnItemsChanged();
            gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
            _signalBus.Fire<NewCardsSignal>();
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.MOVE_TO_GARAGE)
            {
                _analytics.SendEvent(AmplitudeTutorialEvent.tutor_weapon_button_complete);
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<ShowTutorialSignal>();
            }

            _analytics.SendEvent(AmplitudeEvent.ui_change_weapon_show);
        }
        
        private void Tutorial(TutorialSendTargetSignal signal)
        {
            TutorialStepType tutorialStepType = _dataService.GetTutorialCurrentStep();

            switch (tutorialStepType)
            {
                case TutorialStepType.FIRST_WEAPON_POINTER:
                {
                    _signalBus.Fire(new TutorialPointerSignal() {PointerTarget = FirstElem.transform, 
                        PointerEndTarget = deckController.transform, WeaponType = WeaponType.PADDLE, IsMergeTutorial = false});
                    break;
                }
                
                case TutorialStepType.AUTO_BUILD_POINTER_HELP:
                {
                    _dataService.SetExtensionUnlock(Extension.AUTO_BUILD_BUTTON, true);
                    _signalBus.Fire<TutorialButtonSignal>();
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = autobuildButton.transform});
                    break;
                }

                case TutorialStepType.CLOSE_WEAPON_COLLECTION_DIALOG_POINTER_HELP:
                {
                    _signalBus.Fire(new TutorialPointerSignal(){PointerTarget = _closeButton.transform});
                    break;
                }
                
            }
        }

        private void SetupButton(FilterType filterType, Vector2 spacing)
        {
            SetFilter(filterType);
            ChangeSprite(_filterType);
            Filter();
            listViewCollection.ItemsPadding = spacing;
            listViewCollection.OnItemsChanged();
            listViewBattle.ItemsPadding = spacing;
            listViewBattle.OnItemsChanged();
        }

        private void Filter() => scrollRectCollection.verticalNormalizedPosition = 1f;
        
        private void ChangeSprite(FilterType filterType)
        {
            allCards.ChangeSprite(FilterType.ALL == filterType);
            attackLong.ChangeSprite(FilterType.LONG_DISTANCE == filterType);
            attackClose.ChangeSprite(FilterType.CLOSE_DISTANCE == filterType);
            defence.ChangeSprite(FilterType.DEFENCE == filterType);
            special.ChangeSprite(FilterType.ONE_ACTION == filterType);
        }

        private void InitFilter() => SetFilter(FilterType.ALL);

        private void SetFilter(FilterType type)
        {
            _filterType = type;
            GroupWeapon();
            deckController.GroupWeapons(_filterType);
        }
        
        private void GroupWeapon()
        {
            if (_filterType == FilterType.ALL)
            {
                _currentFilteredWeaponCards = _dataService.GetCardCollectionData().Collection;
            }
            else
            {
                _currentFilteredWeaponCards = _dataService.GetCardCollectionData().Collection
                    .FindAll(card => card.WeaponType.GetAttribute<WeaponData>().FilterType == _filterType);
            }

            
            if (_filterType == FilterType.LONG_DISTANCE)
                _currentFilteredWeaponCards.AddRange(_dataService.GetCardCollectionData().Collection.FindAll(card =>
                    card.WeaponType == WeaponType.MINA_BOMB || card.WeaponType == WeaponType.BOMB_ILLNESS));

            if (_filterType == FilterType.ONE_ACTION)
                _currentFilteredWeaponCards.RemoveAll(card =>
                    card.WeaponType == WeaponType.MINA_BOMB || card.WeaponType == WeaponType.BOMB_ILLNESS);
            
            
            _currentFilteredWeaponCards.Sort(delegate(CardInfo x, CardInfo y)
            {
                if (x.WeaponType == WeaponType.NONE && y.WeaponType == WeaponType.NONE) return 0;
                else if (x.WeaponType == WeaponType.NONE) return -1;
                else if (y.WeaponType == WeaponType.NONE) return 1;
                else return x.WeaponType.CompareTo(y.WeaponType);
            });
            
            List<CardInfo> tempFilteredWeaponCards = new List<CardInfo>(_currentFilteredWeaponCards);
            List<CardInfo> singlesCards = new List<CardInfo>();
            _currentFilteredWeaponCards.Clear();
            foreach (CardInfo card in tempFilteredWeaponCards)
            {
                if(_currentFilteredWeaponCards.Contains(card) || singlesCards.Contains(card))
                    continue;
                List<CardInfo> tempList = tempFilteredWeaponCards.FindAll(c => c.WeaponType == card.WeaponType);
                if (tempList.Count <= 1)
                {
                    singlesCards.AddRange(tempList);
                    continue;
                }
                tempList.Sort(delegate(CardInfo x, CardInfo y)
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
                
                _currentFilteredWeaponCards.AddRange(withoutSinglesList);
                tempList.Clear();
            }
            _currentFilteredWeaponCards.Reverse();
            _currentFilteredWeaponCards.AddRange(singlesCards);
            tempFilteredWeaponCards.Clear();
        }

        private void UpdateItems()
        {
            cardCounText.text = _dataService.GetCardCollectionData().Collection.Count.ToString();
            listViewCollection.OnItemsChanged();
            listViewBattle.OnItemsChanged();
            switch (_filterType)
            {
                case FilterType.CLOSE_DISTANCE:
                    SetupButton(FilterType.CLOSE_DISTANCE, spasingGridCards);
                    break;
                case FilterType.LONG_DISTANCE:
                    SetupButton(FilterType.LONG_DISTANCE, spasingGridCards);
                    break;
                case FilterType.DEFENCE:
                    SetupButton(FilterType.DEFENCE, spasingGridCards);
                    break;
                case FilterType.ONE_ACTION:
                    SetupButton(FilterType.ONE_ACTION, spasingGridCards);
                    break;
                case FilterType.ALL:
                    SetupButton(FilterType.ALL, spasingGridCards);
                    break;
            }
        }
        

        public override void Hide()
        {
            if(IsTutorialInProcessing())
                return;
            _gameInfoService.ClearAllFilteredCards();
            _signalBus.Fire<NewCardsSignal>();
            if (_dataService.GetTutorialCurrentStep() == TutorialStepType.CLOSE_WEAPON_COLLECTION_DIALOG_POINTER_HELP)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _dataService.SetExtensionUnlock(Extension.BATH_COLLECTION_BUTTON, true);
                _signalBus.Fire<TutorialButtonSignal>();
                _signalBus.Fire<TutorialSendTargetSignal>();
            }
            _dataService.UpdateCardCollectionData();
            _dataService.UpdateBattleCardInfoData();
            base.Hide();
        }

        private bool IsTutorialInProcessing() => !_dataService.GetTutorialStepComplete(TutorialStepType.AUTO_BUILD_POINTER_HELP);

        public int GetItemsCount()
        {
            if (_filterType == FilterType.NONE) 
                return 0;
            return _currentFilteredWeaponCards.Count;
        }

        public int GetViewTypeNumber(int position) => 0;

        public void SetupView(int position, int type, GameObject itemObject)
        {
            if (position == 0) FirstElem = itemObject;
            CardInfo cardInfo = _currentFilteredWeaponCards[position]; 
            DeckCard weaponCardView = itemObject.GetComponent<DeckCard>();
            weaponCardView.Setup(_weaponSetup, new BattleCardInfo(cardInfo), deckController.gameObject.transform, false, UpdateItems);
            weaponCardView.ActivateNewCardIcon();
            itemObject.transform.localScale = new Vector3(0.8f, 0.8f, 0);
            itemObject.GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
