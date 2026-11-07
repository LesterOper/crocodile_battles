using System.Collections.Generic;
using System.Linq;
using App.Counters;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Coffee.UIExtensions;
using DG.Tweening;
using Library;
using Library.Dialogs;
using Library.Utils;
using Library.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App.Dialogs.Merge
{
    public class MergeDialog : Dialog, RecycleListView.IListAdapter
    {
        [SerializeField] private UIParticle _effectSetNewCard = null;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private FilterButtonHandler allCards;
        [SerializeField] private FilterButtonHandler bath;
        [SerializeField] private FilterButtonHandler attack_long;
        [SerializeField] private FilterButtonHandler attack_close;
        [SerializeField] private FilterButtonHandler defence;
        [SerializeField] private FilterButtonHandler special;
        [SerializeField] private MergePlace left;
        [SerializeField] private MergePlace right;
        [SerializeField] private MergeResultView _resultView;
        [SerializeField] private RecycleListView _listView;
        [SerializeField] private int cardMaxLevel = 9;
        [Inject]private WeaponSetup _weaponSetup;
        [Inject]private BathSetup _bathSetup;
        public static bool IsPlayerBathAdded = false;
        public Vector2 spacingGridNormalCards;
        [Inject]private IDataService Data;
        private FilterType _filterType = FilterType.NONE;
        public GameObject firstElem;
        public GameObject lastElem;
        private List<ICardInfo> _currentFilteredCards;
        private List<ICardInfo> allCardsList = new List<ICardInfo>();
        private FilterUniqItems _util;
        [Inject]private CounterUtility util;

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<AddItemInListSignal>(UpdateAddedItem);
            _signalBus.TryUnsubscribe<MergeFilterChangedSignal>(UpdateFilteredCards);
            _signalBus.TryUnsubscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.TryUnsubscribe<TutorialSendTargetSignal>(Tutorial);
            _signalBus.TryUnsubscribe<UnlockFilterButtonsSignal>(UnlockFilterButtons);
        }

        private void SummaryAllCards()
        {
            allCardsList.AddRange(Data.GetBathCollection().BathDatas);
            allCardsList.AddRange(Data.GetCardCollectionData().Collection);
            allCardsList.AddRange(Data.GetBattleCardInfoData().CardInfos);
            //allCardsList.Add(Data.GetBathData());
        }

        protected override void Start()
        {
            base.Start();
            
            if (Data.GetTutorialCurrentStep() == TutorialStepType.MERGE_IN_DIALOG_HELP)
                _filterType = FilterType.CLOSE_DISTANCE;
            SummaryAllCards();
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            _util = new FilterUniqItems();
            InitFilter();
            ChangeSprite(_filterType);
            Filter();
            allCards.AddListener(()=>SetupButton(FilterType.ALL, spacingGridNormalCards));
            bath.AddListener(() => SetupButton(FilterType.BATH, spacingGridNormalCards));
            attack_long.AddListener(() => SetupButton(FilterType.CLOSE_DISTANCE, spacingGridNormalCards));
            attack_close.AddListener(() => SetupButton(FilterType.LONG_DISTANCE, spacingGridNormalCards));
            defence.AddListener(() => SetupButton(FilterType.DEFENCE, spacingGridNormalCards));
            special.AddListener(() => SetupButton(FilterType.ONE_ACTION, spacingGridNormalCards));
            
            
            _listView.SetAdapter(this);
            _listView.OnItemsChanged();
            gameObject.transform.DOPunchScale(Vector3.one*0.15f, 0.2f, 1, 0);
        
            if (!Data.GetTutorialStepComplete(TutorialStepType.MERGE_IN_DIALOG_HELP))
            {
                _signalBus.Fire<CompleteTutorialSignal>();
                _signalBus.Fire<ShowTutorialSignal>();
            }
            _signalBus.Subscribe<MergeFilterChangedSignal>(UpdateFilteredCards);
            _signalBus.Subscribe<UpdateItemsInListSignal>(UpdateItems);
            _signalBus.Subscribe<AddItemInListSignal>(UpdateAddedItem);
            _signalBus.Subscribe<TutorialSendTargetSignal>(Tutorial);
            _signalBus.Subscribe<UnlockFilterButtonsSignal>(UnlockFilterButtons);
        }
    
        private void UpdateItems()
        {
            allCardsList.Clear();
            SummaryAllCards();
            _listView.OnItemsChanged();
        }
        private void UnlockFilterButtons(UnlockFilterButtonsSignal signal)
        {
            bool unlock = signal.Unlock;
            allCards.Interactable(unlock);
            bath.Interactable(unlock);
            attack_long.Interactable(unlock);
            attack_close.Interactable(unlock);
            defence.Interactable(unlock);
            special.Interactable(unlock);
        }

        private void PlayNewCardEffect(Vector3 pos)
        {
            _effectSetNewCard.rectTransform.position = pos;
            _effectSetNewCard.gameObject.SetActive(true);
            _effectSetNewCard.Play();
        }

        private void UpdateAddedItem(AddItemInListSignal signal)
        {
            if ((!left.IsFree() || !right.IsFree()) )
            {
                _signalBus.Fire(new MergeFilterChangedSignal()
                {
                    CardInfo = left.GetCardInfo() == null ? right.GetCardInfo() : left.GetCardInfo()
                });
            }
            else if (!signal.IsAdd)
            {
                SetFilter(_filterType);
            }
            else
            {
                if (_filterType == FilterType.BATH)
                {
                    _currentFilteredCards = allCardsList.FindAll(local => local.IsBathCard());
                    SetFilter(_filterType);
                }
                else
                {
                    _currentFilteredCards = allCardsList.FindAll(local => !local.IsBathCard() &&
                                                                          ((WeaponType) local.GetCardType()).GetAttribute<WeaponData>().FilterType == _filterType);
                    SetFilter(_filterType);
                }
                
            }
            _listView.OnItemsChanged();
        }

        private void InitFilter()
        {
            var filteredBathCards = allCardsList.FindAll(local => local.IsBathCard());
            if (_util.Dublicates(filteredBathCards))
            {
                _filterType = FilterType.BATH;
                SetFilter(_filterType);
                _listView.ItemsPadding = spacingGridNormalCards;
                return;
            }
        
            if (_util.Dublicates(GetWeaponCardsByFilterType(FilterType.LONG_DISTANCE)))
            {
                ResetFilteredCards();
                ActivateFilterWithDublicates(FilterType.LONG_DISTANCE, spacingGridNormalCards);
                return;
            }
        
            if (_util.Dublicates(GetWeaponCardsByFilterType(FilterType.CLOSE_DISTANCE)))
            {
                ResetFilteredCards();
                ActivateFilterWithDublicates(FilterType.CLOSE_DISTANCE, spacingGridNormalCards);
                return;
            }
        
            if (_util.Dublicates(GetWeaponCardsByFilterType(FilterType.DEFENCE)))
            {
                ResetFilteredCards();
                ActivateFilterWithDublicates(FilterType.DEFENCE, spacingGridNormalCards);
                return;
            }
        
            if (_util.Dublicates(GetWeaponCardsByFilterType(FilterType.ONE_ACTION)))
            {
                ResetFilteredCards();
                ActivateFilterWithDublicates(FilterType.ONE_ACTION, spacingGridNormalCards);
                return;
            }
            SetFilter(_filterType);
        }

        private void ResetFilteredCards()
        {
            _currentFilteredCards.Clear();
            _currentFilteredCards = null;
        }

        private List<ICardInfo> GetWeaponCardsByFilterType(FilterType filterType)
        {
            _currentFilteredCards = allCardsList.FindAll(card => !card.IsBathCard() &&
                                                                 ((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().FilterType == filterType);

            var bombCollection = allCardsList.FindAll(card => !card.IsBathCard() &&
                                                              ((WeaponType) card.GetCardType() ==
                                                               WeaponType.BOMB_ILLNESS
                                                               || (WeaponType) card.GetCardType() ==
                                                               WeaponType.MINA_BOMB));
            if(filterType == FilterType.LONG_DISTANCE)
                _currentFilteredCards.AddRange(bombCollection);

            if (filterType == FilterType.ONE_ACTION)
                _currentFilteredCards.RemoveAll(card => bombCollection.Contains(card));
            
           /* List<ICardInfo> cardInfos = allCardsList.FindAll(card => !card.IsBathCard() &&
                                                                     card.IsInUseForBattle() && ((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().FilterType == filterType);
            _currentFilteredCards.AddRange(cardInfos);*/
            return _currentFilteredCards;
        }
    
        private void ActivateFilterWithDublicates(FilterType filterType, Vector2 spacingGrid)
        {
            _filterType = filterType;
            SetFilter(_filterType);
            _listView.ItemsPadding = spacingGrid;
        }

        private void SetupButton(FilterType filterType, Vector2 spacing)
        {
            SetFilter(filterType);
            ChangeSprite(_filterType);
            Filter();
            _listView.ItemsPadding = spacing;
            _listView.OnItemsChanged();
        }

        private void SetFilter(FilterType type)
        {
            _filterType = type;
            if (_filterType == FilterType.ALL)
                GroupAllCards();
            else if (_filterType == FilterType.BATH)
                GroupBathCards();
            else
                GroupWeapon();
        }

        private void GroupAllCards()
        {
            _currentFilteredCards = allCardsList.FindAll(card =>!card.IsBathCard() && card.GetLevel() != cardMaxLevel);

            _currentFilteredCards.Sort(delegate(ICardInfo x, ICardInfo y)
            {
                if ((WeaponType)x.GetCardType() == WeaponType.NONE && (WeaponType)y.GetCardType() == WeaponType.NONE) return 0;
                else if ((WeaponType)x.GetCardType() == WeaponType.NONE) return -1;
                else if ((WeaponType)y.GetCardType() == WeaponType.NONE) return 1;
                else return x.GetCardType().CompareTo(y.GetCardType());
            });
            
            List<ICardInfo> tempFilteredWeaponCards = new List<ICardInfo>(_currentFilteredCards);
            List<ICardInfo> singlesCards = new List<ICardInfo>();
            _currentFilteredCards.Clear();
            
            foreach (ICardInfo card in tempFilteredWeaponCards)
            {
                if(_currentFilteredCards.Contains(card) || singlesCards.Contains(card))
                    continue;
                List<ICardInfo> tempList = tempFilteredWeaponCards.FindAll(c => c.GetCardType() == card.GetCardType());
                if (tempList.Count <= 1)
                {
                    singlesCards.AddRange(tempList);
                    continue;
                }
                tempList.Sort(delegate(ICardInfo x, ICardInfo y)
                {
                    if (x.GetLevel() == 0 && y.GetLevel() == 0) return 0;
                    else if (x.GetLevel() == 0) return -1;
                    else if (y.GetLevel() == 0) return 1;
                    else return x.GetLevel().CompareTo(y.GetLevel());
                });
                tempList.Reverse();
                foreach (var cardByLevel in tempList)
                {
                    var tempListByLevel = tempList.FindAll(c => c.GetLevel() == cardByLevel.GetLevel());
                    if (tempListByLevel.Count <= 1)
                    {
                        singlesCards.AddRange(tempListByLevel);
                        continue;
                    }
                }

                var withoutSinglesList = tempList.Except(singlesCards);
                
                _currentFilteredCards.AddRange(withoutSinglesList);
                tempList.Clear();
            }
            _currentFilteredCards.Reverse();
            _currentFilteredCards.AddRange(singlesCards);
            _currentFilteredCards.AddRange(allCardsList.FindAll(card => card.IsBathCard()));
            tempFilteredWeaponCards.Clear();
        }
        private void GroupWeapon()
        {
            _currentFilteredCards = allCardsList
                .FindAll(card => !card.IsBathCard() && ((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().FilterType == _filterType 
                                                    && card.GetLevel() != cardMaxLevel);

            if (_filterType == FilterType.LONG_DISTANCE)
            {
                _currentFilteredCards.AddRange(allCardsList.FindAll(card => !card.IsBathCard() &&
                                                                            (WeaponType)card.GetCardType() == WeaponType.MINA_BOMB || (WeaponType)card.GetCardType() == WeaponType.BOMB_ILLNESS));
            }

            if (_filterType == FilterType.ONE_ACTION)
            {
                _currentFilteredCards.RemoveAll(card =>
                    !card.IsBathCard() && (WeaponType)card.GetCardType() == WeaponType.MINA_BOMB || (WeaponType)card.GetCardType() == WeaponType.BOMB_ILLNESS);
            }
            
            _currentFilteredCards.Sort(delegate(ICardInfo x, ICardInfo y)
            {
                if ((WeaponType)x.GetCardType() == WeaponType.NONE && (WeaponType)y.GetCardType() == WeaponType.NONE) return 0;
                else if ((WeaponType)x.GetCardType() == WeaponType.NONE) return -1;
                else if ((WeaponType)y.GetCardType() == WeaponType.NONE) return 1;
                else return x.GetCardType().CompareTo(y.GetCardType());
            });
            
            List<ICardInfo> tempFilteredWeaponCards = new List<ICardInfo>(_currentFilteredCards);
            List<ICardInfo> singlesCards = new List<ICardInfo>();
            _currentFilteredCards.Clear();
            
            foreach (ICardInfo card in tempFilteredWeaponCards)
            {
                if(_currentFilteredCards.Contains(card) || singlesCards.Contains(card))
                    continue;
                List<ICardInfo> tempList = tempFilteredWeaponCards.FindAll(c => c.GetCardType() == card.GetCardType());
                if (tempList.Count <= 1)
                {
                    singlesCards.AddRange(tempList);
                    continue;
                }
                tempList.Sort(delegate(ICardInfo x, ICardInfo y)
                {
                    if (x.GetLevel() == 0 && y.GetLevel() == 0) return 0;
                    else if (x.GetLevel() == 0) return -1;
                    else if (y.GetLevel() == 0) return 1;
                    else return x.GetLevel().CompareTo(y.GetLevel());
                });
                tempList.Reverse();
                foreach (var cardByLevel in tempList)
                {
                    var tempListByLevel = tempList.FindAll(c => c.GetLevel() == cardByLevel.GetLevel());
                    if (tempListByLevel.Count <= 1)
                    {
                        singlesCards.AddRange(tempListByLevel);
                        continue;
                    }
                }

                var withoutSinglesList = tempList.Except(singlesCards);
                
                _currentFilteredCards.AddRange(withoutSinglesList);
                tempList.Clear();
            }
            _currentFilteredCards.Reverse();
            _currentFilteredCards.AddRange(singlesCards);
            tempFilteredWeaponCards.Clear();
        }
        private void GroupBathCards()
        {
            var maxLevelCards = allCardsList.FindAll(bathInCollectionData => 
                bathInCollectionData.IsBathCard() 
                && bathInCollectionData.GetLevel() == cardMaxLevel);
            _currentFilteredCards = new List<ICardInfo>(allCardsList.FindAll(local => local.IsBathCard()).ToList().Except(maxLevelCards));
            _currentFilteredCards.Sort(delegate(ICardInfo x, ICardInfo y)
            {
                if ((BathType)x.GetCardType() == BathType.NONE && (BathType)y.GetCardType() == BathType.NONE) return 0;
                else if ((BathType)x.GetCardType() == BathType.NONE) return -1;
                else if ((BathType)y.GetCardType() == BathType.NONE) return 1;
                else return x.GetCardType().CompareTo(y.GetCardType());
            });

            List<ICardInfo> tempFilteredBathData = new List<ICardInfo>(_currentFilteredCards);
            List<ICardInfo> singlesCards = new List<ICardInfo>();
            _currentFilteredCards.Clear();
            foreach (var card in tempFilteredBathData)
            {
                if (_currentFilteredCards.Contains(card) || singlesCards.Contains(card) ) 
                    continue;
                List<ICardInfo> tempList =
                    tempFilteredBathData.FindAll(c => c.GetCardType() == card.GetCardType());
                if (tempList.Count <= 1)
                {
                    singlesCards.AddRange(tempList);
                    continue;
                }
                tempList.Sort(delegate(ICardInfo x, ICardInfo y)
                {
                    if (x.GetLevel() == 0 && y.GetLevel() == 0) return 0;
                    else if (x.GetLevel() == 0) return -1;
                    else if (y.GetLevel() == 0) return 1;
                    else return x.GetLevel().CompareTo(y.GetLevel());
                });
                tempList.Reverse();
                foreach (var cardByLevel in tempList)
                {
                    var tempListByLevel = tempList.FindAll(c => c.GetLevel() == cardByLevel.GetLevel());
                    if (tempListByLevel.Count <= 1)
                    {
                        singlesCards.AddRange(tempListByLevel);
                        continue;
                    }
                }
                var withoutSinglesList = tempList.Except(singlesCards);
                _currentFilteredCards.AddRange(withoutSinglesList);
                tempList.Clear();
            }
            _currentFilteredCards.Reverse();
            _currentFilteredCards.AddRange(singlesCards);
            tempFilteredBathData.Clear();     
        }
    
    
        private void ChangeSprite(FilterType filterType)
        {
            allCards.ChangeSprite(FilterType.ALL == filterType);
            bath.ChangeSprite(FilterType.BATH == filterType);
            attack_long.ChangeSprite(FilterType.CLOSE_DISTANCE == filterType);
            defence.ChangeSprite(FilterType.DEFENCE == filterType);
            special.ChangeSprite(FilterType.ONE_ACTION == filterType);
            attack_close.ChangeSprite(FilterType.LONG_DISTANCE == filterType);
        
            _resultView.Setup(_filterType);
        }
    
        private void Filter()
        {
            _scrollRect.verticalNormalizedPosition = 1f;
        }
        
        public override void Hide()
        {
            if (Data.GetTutorialCurrentStep() == TutorialStepType.MERGE_POINTER_1)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
            }
            if (Data.GetTutorialCurrentStep() == TutorialStepType.MERGE_POINTER_2)
            {
                _signalBus.Fire<CompleteTutorialSignal>();
            }
           _signalBus.Fire<MergeAvailableSignal>();
            base.Hide();
        }

        private void UpdateFilteredCards(MergeFilterChangedSignal signal)
        {
            //if(_filterType != FilterType.ALL)
                _filterType = signal.FilterType;
            var leftCard = left.GetCardInfo();
            var rightCard = right.GetCardInfo();
            ICardInfo card = leftCard ?? rightCard;
            if (_filterType == FilterType.BATH)
            {
                _currentFilteredCards = allCardsList.FindAll(obj => obj.IsBathCard() && obj.GetLevel() == card.GetLevel() 
                                                                    && (BathType)obj.GetCardType() == (BathType)card.GetCardType()).ToList();
                ICardInfo cardLocal;
                if (leftCard != null)
                {
                    cardLocal =
                        _currentFilteredCards.Find(local =>
                            local.IsBathCard() && local.Equals(leftCard) && local.GetId() == leftCard.GetId());
                    if (cardLocal != null)
                        _currentFilteredCards.Remove(cardLocal);
                }

                if (rightCard != null)
                {
                    cardLocal =
                        _currentFilteredCards.Find(local =>
                            local.IsBathCard() && local.Equals(rightCard) && local.GetId() == rightCard.GetId());
                    if (cardLocal != null)
                        _currentFilteredCards.Remove(cardLocal);
                }
            }
            else
            {

                if (card != null)
                {
                    _currentFilteredCards.Clear();
                    _currentFilteredCards = allCardsList.FindAll(cardLocal => !cardLocal.IsBathCard() &&
                                                                              (WeaponType) cardLocal.GetCardType() ==
                                                                              (WeaponType) card.GetCardType() &&
                                                                              cardLocal.GetLevel() == card.GetLevel());

                    if (leftCard != null)
                    {
                        var cardLocal =
                            _currentFilteredCards.Find(local =>
                                !local.IsBathCard() && local.Equals(leftCard) && local.GetId() == leftCard.GetId());
                        if (cardLocal != null)
                            _currentFilteredCards.Remove(cardLocal);
                    }

                    if (rightCard != null)
                    {
                        var cardLocal =
                            _currentFilteredCards.Find(local =>
                                !local.IsBathCard() && local.Equals(rightCard) && local.GetId() == rightCard.GetId());
                        if (cardLocal != null)
                            _currentFilteredCards.Remove(cardLocal);
                    }
                }
                else
                {
                    _currentFilteredCards = allCardsList.FindAll(cardLocal =>!cardLocal.IsBathCard() && cardLocal.GetLevel() != cardMaxLevel);
                    InitFilter();
                    ChangeSprite(_filterType);
                }

            }

            _listView.OnItemsChanged();
        }

        public int GetItemsCount()
        {
            if (_filterType == FilterType.NONE) return 0;
            return _currentFilteredCards.Count;
        }

        public int GetViewTypeNumber(int position)
        {
            return 0;
        }

        public void SetupView(int position, int type, GameObject itemObject)
        {
            if (position == 0) firstElem = itemObject;
            switch (type)
            {
                case 0 :
                    ICardInfo card = _currentFilteredCards[position];
                    Card bathMergeCardView = itemObject.GetComponent<Card>();
                    itemObject.transform.GetChild(0).gameObject.SetActive(true);
                    bathMergeCardView.Setup( card);
                    bathMergeCardView.SetMergeDrag(left, right); 
                    itemObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                    itemObject.GetComponent<CanvasGroup>().alpha = 1;
                    if (position == _currentFilteredCards.Count - 1)
                        lastElem = itemObject;
                    break;
            }

        }

        private void Tutorial(TutorialSendTargetSignal signal)
        {
            bool isLeft = signal.FreeMergePlace;
            _signalBus.Fire(new TutorialPointerSignal()
            {
                PointerTarget = firstElem.transform, PointerEndTarget = isLeft ? left.transform : right.transform,
                IsMergeTutorial = true
            });
        }
    }

    public enum FilterType
    {
        NONE = 0,
        BATH = 1,
        CLOSE_DISTANCE = 2,
        LONG_DISTANCE = 3,
        DEFENCE = 4,
        ONE_ACTION = 5,
        ALL = 6,
        MOVEMENT = 7,
    }
}

public class FilterUniqItems
{
    public bool Dublicates(List<BathInCollectionData> list)
    {
        var set = new List<BathInCollectionData>();
        foreach (var item in list)
        {
            if (set.Find(card => card.Level == item.Level && card.CrocodileBathType == item.CrocodileBathType) != null)
            {
                set.Clear();
                return true;
            }
            set.Add(item);
        }
        set.Clear();
        return false;
    }
        
    public bool Dublicates(List<ICardInfo> list)
    {
        var set = new List<ICardInfo>();
        foreach(var item in list)
        {
            if (set.Find(card => card.GetLevel() == item.GetLevel() && (WeaponType)card.GetCardType() == (WeaponType)item.GetCardType()) != null)
            {
                set.Clear();
                return true;
            }
            set.Add(item);
        }
        set.Clear();
        return false;
    }
}