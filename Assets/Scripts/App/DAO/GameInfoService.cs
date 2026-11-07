using System.Collections.Generic;
using App;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.Signals;
using Library;
using Library.Utils;
using Zenject;

public class GameInfoService : IGameInfoService
{
    private List<ICardInfo> cards = new List<ICardInfo>();
    private List<BathInCollectionData> bathCards = new List<BathInCollectionData>();
    private bool isBossFight;
    private bool isDoubleCards;
    private bool isGarage;
    private long lastSessionTicks = 0;
    private bool showPassiveDialog;
    [Inject]private SignalBus _signalBus;
    public bool DeckIsEmpty { get; set; }
    public bool IsEnemyCardsOpenByAds { get; set; }
    
    public void SetNewCardsInfo(SignalBus signalBus, List<ICardInfo> cardsToAdd)
    {
        cards.AddRange(cardsToAdd);
        signalBus.Fire<NewCardsSignal>();
    }

    public List<ICardInfo> GetNewCardsInfo() => cards;
    public void ClearByFilterType(FilterType filterType) => cards.RemoveAll(card => ((WeaponType)card.GetCardType()).GetAttribute<WeaponData>().FilterType == filterType);
    public void ClearAllFilteredCards() => cards.Clear();

    public void Clear() => cards.Clear();

    bool IGameInfoService.IsBossFight() => isBossFight;

    bool IGameInfoService.IsDoubleCards() => isDoubleCards;
    public void SetIsBossFight(bool isBoss) => isBossFight = isBoss;
    public void SetDoubleCardsInRound(bool doubleCards) => isDoubleCards = doubleCards;

    public void SetNewBathsCardInfo(SignalBus signalBus, List<BathInCollectionData> bathInCollectionDatas)
    {
        bathCards.AddRange(bathInCollectionDatas);
        signalBus.Fire<NewCardsSignal>();
    }

    public List<BathInCollectionData> GetNewBathsCardInfo() => bathCards;
    public void ClearBaths() => bathCards.Clear();

    public bool IsGarage() => isGarage;
    public void SetIsGarage(bool isGarage) => this.isGarage = isGarage;
}