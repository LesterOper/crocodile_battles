using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using CodeBase.Infrastructure.Services;
using Library.Utils;
using Library.View;
using TMPro;
using UnityEngine;
using Zenject;

public class DeckController : MonoBehaviour, RecycleListView.IListAdapter
{
    [SerializeField] private RecycleListView recycleListView;
    [Inject]private WeaponSetup _setup;
    [SerializeField] private TextMeshProUGUI deckCardsCount;
    [SerializeField] private TextMeshProUGUI deckCardsCountMax;
    private List<DeckCard> _cards = new List<DeckCard>();
    private DeckUtils _deckUtils;
    [Inject]private IDataService _dataService;
    private List<BattleCardInfo> _currentFilteredBattleCards;
    public DeckUtils DeckUtils => _deckUtils;
    
    private void Awake()
    {
    }

    private void Start()
    {
        _deckUtils = new DeckUtils(_dataService);
        recycleListView.SetAdapter(this);
        recycleListView.OnItemsChanged();
    }
    
    public int GetItemsCount()
    {
        deckCardsCount.text = _deckUtils.GetDeckSize().ToString();
        deckCardsCountMax.text ="/" + DeckUtils.CardLimit;
        return _currentFilteredBattleCards.Count; //_deckUtils.GetDeckSize();
    }

    public void SetWeaponSetup(WeaponSetup weaponSetup)
    {
        //_setup = weaponSetup;
    }

    public void GroupWeapons(FilterType filterType)
    {
        if (filterType == FilterType.ALL)
        {
            _currentFilteredBattleCards = _dataService.GetBattleCardInfoData().CardInfos;
        }
        else
        {
            _currentFilteredBattleCards = _dataService.GetBattleCardInfoData().CardInfos
                .FindAll(card => card.WeaponType.GetAttribute<WeaponData>().FilterType == filterType);
        }
        
        if (filterType == FilterType.LONG_DISTANCE)
            _currentFilteredBattleCards.AddRange(_dataService.GetBattleCardInfoData().CardInfos.FindAll(card =>
                card.WeaponType == WeaponType.MINA_BOMB || card.WeaponType == WeaponType.BOMB_ILLNESS));

        if (filterType == FilterType.ONE_ACTION)
            _currentFilteredBattleCards.RemoveAll(card =>
                card.WeaponType == WeaponType.MINA_BOMB || card.WeaponType == WeaponType.BOMB_ILLNESS);
        
        _currentFilteredBattleCards.Sort(delegate(BattleCardInfo x, BattleCardInfo y)
        {
            if (x.WeaponType == WeaponType.NONE && y.WeaponType == WeaponType.NONE) return 0;
            else if (x.WeaponType == WeaponType.NONE) return -1;
            else if (y.WeaponType == WeaponType.NONE) return 1;
            else return x.WeaponType.CompareTo(y.WeaponType);
        });
        
        List<BattleCardInfo> tempFilteredWeaponCards = new List<BattleCardInfo>(_currentFilteredBattleCards);
        List<BattleCardInfo> singlesCards = new List<BattleCardInfo>();
        _currentFilteredBattleCards.Clear();
        
        foreach (BattleCardInfo card in tempFilteredWeaponCards)
        {
            if(_currentFilteredBattleCards.Contains(card) || singlesCards.Contains(card))
                continue;
            List<BattleCardInfo> tempList = tempFilteredWeaponCards.FindAll(c => c.WeaponType == card.WeaponType);
            if (tempList.Count <= 1)
            {
                singlesCards.AddRange(tempList);
                continue;
            }
            tempList.Sort(delegate(BattleCardInfo x, BattleCardInfo y)
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
                
            _currentFilteredBattleCards.AddRange(withoutSinglesList);
            tempList.Clear();
        }
        _currentFilteredBattleCards.Reverse();
        _currentFilteredBattleCards.AddRange(singlesCards);
        tempFilteredWeaponCards.Clear();
    }
    
    public int GetViewTypeNumber(int position) => 0;

    public void SetupView(int position, int type, GameObject itemObject)
    {
        BattleCardInfo cardInfo = _currentFilteredBattleCards[position];
        DeckCard weaponCardView = itemObject.GetComponent<DeckCard>();
        weaponCardView.Setup(_setup, new BattleCardInfo(cardInfo), gameObject.transform, true, UpdateItems);
        //weaponCardView.ActivateNewCardIcon();
        itemObject.transform.localScale = new Vector3(0.8f, 0.8f, 0);
        itemObject.GetComponent<CanvasGroup>().alpha = 1;
    }
    
    private void UpdateItems() => recycleListView.OnItemsChanged();
}

public class DeckUtils
{
    public const int CardLimit = 50;
    private IDataService _dataService;
    public DeckUtils(IDataService dataService) => _dataService = dataService;
    public int GetDeckSize() => GetDeckCards() == null ? 0 : GetDeckCards().Count;
    
    public List<BattleCardInfo> GetDeckCards() =>
        _dataService.GetBattleCardInfoData().CardInfos;
}