using App.Cards;
using App.Counters;
using App.Crocodile.Handlers;
using App.Crocodile.Implementations;
using App.DAO;
using Battle.Equipment;
using UnityEngine;

namespace App.Crocodile.Interfaces
{
    public interface ICrocodile
    {
        CrocodileStatsHandler GetStatsHandler();
    
        Transform GetEndBattlePoint();
        Transform GetBattlePoint();
        Transform GetStartPoint();
    
        void Setup(CrocodileController controller);
        void ListenToFasteners(CrocodileController crocodileController);
        void WeaponEquipped(IEquipment view, bool hasFastener, int level);
        void SetCardsHandler(ICardsHandler cardsHandler); 
        void SetZeroRotation(Transform transform);
        void AutoEquippingCrocodile();

        void RandomizeCardsForCurrentRound();
        void Initialize(CounterUtility counterUtility, IDataService dataService, IGameInfoService gameInfoService);

        void HideOpenedCards();
    
        bool CanMove();
        bool IsPlayer();
    }
}
