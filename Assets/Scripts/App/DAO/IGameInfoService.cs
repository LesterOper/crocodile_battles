using System.Collections.Generic;
using App.DAO;
using App.Dialogs.Merge;
using CodeBase.Infrastructure.Services;
using Zenject;

public interface IGameInfoService : IService
{
    bool DeckIsEmpty { get; set; }
    bool IsEnemyCardsOpenByAds { get; set; }
    void SetNewCardsInfo(SignalBus signalBus, List<ICardInfo> cards);
    List<ICardInfo> GetNewCardsInfo();
    void ClearByFilterType(FilterType filterType);
    void ClearAllFilteredCards();
    void Clear();
    bool IsBossFight();
    bool IsDoubleCards();
    void SetIsBossFight(bool isBossFight);
    void SetDoubleCardsInRound(bool doubleCards);
    void SetNewBathsCardInfo(SignalBus signalBus, List<BathInCollectionData> bathInCollectionDatas);
    List<BathInCollectionData> GetNewBathsCardInfo();
    void ClearBaths();

    bool IsGarage();
    void SetIsGarage(bool isGarage);
}