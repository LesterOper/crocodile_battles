using System;
using App.Cards;
using App.Counters;
using App.DAO;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using Battle.Equipment;
using CodeBase.Infrastructure;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Crocodile.Interfaces
{
    public interface ICrocodileController
    {
        Rigidbody2D GetCrocoBathRigidbody();
        CrocodileInfo GetCrocodileInfo();
        ICrocodileController GetEnemy();
        void DestroyObject();
        BattleSetup GetBattleSetup();
        void StartCrocodile(Transform zeroPoint, AnimationCurve curve, AnimationCurve curve1, ICrocodileController enemy, ICoroutineRunner runner, bool move, SignalBus signalBus);
        void SetOneActionTarget(Transform target, ICrocodileController enemy);
        void SetWeaponEquippingHandler(WeaponEquipingHandler equipingHandler);
        void SetBathFastenerHandler(BathFastenersHandler bathFastenersHandler);
        void WeaponEquipped(IEquipment equipment, bool hasFastener, int level);
        void SetCardsHandler(ICardsHandler cardsHandler);
        void SetupCrocodile(IDataService dataService,CounterUtility counterUtility, ICrocodile crocodile, IGameInfoService gameInfoService);
        void StopAttack(Action action = null);
        void Disappearing( Action callback);
        void CheckMove(Action action = null);
        void AutoEquippingCrocodile();

        void RandomizeCardsForCurrentRound();

        void HideOpenedCards();
        
        void SetZeroRotation();
        void MoveToStart();
        void VictoryAnim();
        void ReWriteEnemy(CrocodileInfo enemy);
        bool CanMove();
    }
}