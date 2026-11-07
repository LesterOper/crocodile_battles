using System;
using App.Cards;
using App.Counters;
using App.Crocodile.Handlers;
using App.Crocodile.Interfaces;
using App.DAO;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using Battle.Equipment;
using CodeBase.Infrastructure;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Crocodile.Implementations
{
    public class CrocodileController : MonoBehaviour, ICrocodileController
    {
        private CrocodileInfo _crocodileInfo;
        private AnimationCurve _curve;
        private Rigidbody2D _rigidbody;
        private ICrocodileController enemy;
        private String _name;
        private BattleSetup _battleSetup;
        private void Awake() => _crocodileInfo = new CrocodileInfo();
        
        public CrocodileInfo GetCrocodileInfo() => _crocodileInfo;
        public Rigidbody2D GetCrocoBathRigidbody() => _rigidbody;
        public ICrocodileController GetEnemy() => enemy;
        public String GetName() => _name;
        public void DestroyObject() => Destroy(this.gameObject);
        public BattleSetup GetBattleSetup() => _battleSetup;
        public void SetupCrocodile(IDataService dataService,CounterUtility counterUtility, ICrocodile crocodile, IGameInfoService gameInfoService)
        {
            _crocodileInfo.Crocodile = crocodile;
            _crocodileInfo.Crocodile.Setup(this);
            _crocodileInfo.Crocodile.Initialize(counterUtility, dataService, gameInfoService);
        }

        public void ReWriteEnemy(CrocodileInfo enemyRigid) => _crocodileInfo.MoverHandler.ReWriteEnemy(enemyRigid);

        public void StartCrocodile(Transform zeroPoint, AnimationCurve curve, AnimationCurve curve1, ICrocodileController enemy, ICoroutineRunner runner, bool move, SignalBus signalBus)
        {
            this.enemy = enemy;
            _curve = curve;
            _crocodileInfo.StatsHandler = _crocodileInfo.Crocodile.GetStatsHandler();
            _crocodileInfo.BathFastenersHandler.SetForOneActionBomb(enemy.GetCrocodileInfo().BathFastenersHandler.BathRigidbody.transform);
            
            _crocodileInfo.MoverHandler = new CrocodileMoverHandler(_crocodileInfo.BathFastenersHandler.BathRigidbody, _curve, curve1, 
                _crocodileInfo, this.enemy.GetCrocodileInfo(), runner, signalBus);
            
            _crocodileInfo.Crocodile.ListenToFasteners(this);
            
            if(move)
                MoveToStart();
        }

        public void SetOneActionTarget(Transform target, ICrocodileController enemy)
        {
            this.enemy = enemy;
            _crocodileInfo.BathFastenersHandler
                .SetForOneActionBomb(this.enemy.GetCrocodileInfo().BathFastenersHandler.BathRigidbody.transform);
        }

        public void SetBathFastenerHandler(BathFastenersHandler bathFastenersHandler) => _crocodileInfo.BathFastenersHandler = bathFastenersHandler;
        public void SetWeaponEquippingHandler(WeaponEquipingHandler equipingHandler) => _crocodileInfo.EquipingHandler = equipingHandler;
        public void SetPlayerName(String name) => _name = name;
        public void SetBattleSetup(BattleSetup setup) => _battleSetup = setup;
        public void WeaponEquipped(IEquipment equipment, bool hasFastener, int level) => _crocodileInfo.Crocodile.WeaponEquipped(equipment, hasFastener, level);
        public void SetCardsHandler(ICardsHandler cardsHandler)
        {
            _crocodileInfo.Crocodile.SetCardsHandler(cardsHandler);
            if (_crocodileInfo.Crocodile.IsPlayer())
                cardsHandler.SetWeaponEquippingHandler(_crocodileInfo.EquipingHandler);
        }
        public void AutoEquippingCrocodile() => _crocodileInfo.Crocodile.AutoEquippingCrocodile();
        public void RandomizeCardsForCurrentRound() =>
            _crocodileInfo
            .Crocodile
            .RandomizeCardsForCurrentRound();
        
        public void HideOpenedCards() => 
            _crocodileInfo
            .Crocodile
            .HideOpenedCards();

        public void Disappearing(Action callback) => _crocodileInfo.MoverHandler.EndBattle(callback);
        public void CheckMove(Action action) => _crocodileInfo.MoverHandler.GoToCroco();
        public void StopAttack(Action action) => _crocodileInfo.MoverHandler.GoToStart(action);
        public void SetZeroRotation() => _crocodileInfo.Crocodile.SetZeroRotation(_rigidbody.transform);
        public void MoveToStart() => _crocodileInfo.MoverHandler.StartCrocodile();
        public void VictoryAnim() => _crocodileInfo.BathFastenersHandler.VictoryAnim();
        
        public bool CanMove() => _crocodileInfo.Crocodile.CanMove();
        
    }
}