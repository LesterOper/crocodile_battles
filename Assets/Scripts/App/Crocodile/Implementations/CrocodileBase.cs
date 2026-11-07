using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.Counters;
using App.Crocodile.Fasteners;
using App.Crocodile.Handlers;
using App.Crocodile.Interfaces;
using App.DAO;
using App.Dialogs.Merge;
using App.PrototypeBattle;
using App.SetupsScript;
using Battle;
using Battle.Equipment;
using CodeBase.Infrastructure.Services;
using DG.Tweening;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Crocodile.Implementations
{
    public abstract class CrocodileBase : ICrocodile
    {
        protected CrocodileStatsHandler _statsHandler;
        protected CrocodileController _crocodileController;
        protected List<Fastener> _fasteners;
        protected bool isPlayer;
        protected SignalBus _signalBus;
        protected IDataService _dataService;
        protected IGameInfoService _gameInfoService;

        protected CrocodileBase(bool isPlayer, SignalBus signalBus)
        {
            this.isPlayer = isPlayer;
            _signalBus = signalBus;
        } 
        
        public virtual void RandomizeCardsForCurrentRound() { }

        public virtual void Initialize(CounterUtility counterUtility, IDataService dataService, IGameInfoService gameInfoService)
        {
            _dataService = dataService;
            _gameInfoService = gameInfoService;
            BathData bathData = dataService.GetBathData();

            WeightsRandomBathSettings weightsRandomBathSettings =
                Resources.Load<WeightsRandomBathSettings>("WeightsSettings");
            WeightsRandomBathSettings.Modificator mod = weightsRandomBathSettings.GetModificator(counterUtility);
            if(isPlayer)
                _statsHandler = new CrocodileStatsHandler(0,  new CardCharactaristicsUtils().GetBathHPValueByLevel(
                    bathData.Level, bathData.CrocodileBathType, isPlayer), GetStartMana(isPlayer) ,0,isPlayer, _signalBus, "Player", dataService );
            else
            {
                _statsHandler = new CrocodileStatsHandler(0, new CardCharactaristicsUtils().GetBathHPValueByLevel(
                        EnemyGenerator.GetBathLevel(dataService, _gameInfoService)
                        , EnemyGenerator.GetEnemyBathType(dataService), isPlayer), GetStartMana(isPlayer), 0, isPlayer,
                    _signalBus, _crocodileController.GetName(),dataService);
            }
        }

        public virtual void HideOpenedCards() { }

        private int GetStartMana(bool isPlayerParam) => 
            isPlayerParam ? _crocodileController.GetBattleSetup().StartManaPlayer 
                : _crocodileController.GetBattleSetup().StartManaEnemy;

        public CrocodileStatsHandler GetStatsHandler() => _statsHandler;

        public bool CanMove() => 
            _fasteners.Find(obj => obj.GetCurrentEquipment()?.GetWeaponType().GetAttribute<WeaponData>().FilterType == FilterType.CLOSE_DISTANCE) != null;
        public virtual void Setup(CrocodileController controllerParam) => _crocodileController = controllerParam;
        
        public virtual void SetZeroRotation(Transform transform) =>
            transform.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
        

        public virtual void ListenToFasteners(CrocodileController crocodileControllerParam) => 
            _fasteners = crocodileControllerParam.GetCrocodileInfo().BathFastenersHandler.GetFasteners();

        public virtual void WeaponEquipped(IEquipment view, bool hasFastener, int level)
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            if (hasFastener)
            {
                SetupCollision(view.GetWeaponType());

                _statsHandler.CurrentHealthPointProperty +=
                    utils.GetHealthPointValueByLevel(level, view.GetWeaponType());
                _statsHandler.MaxHealthPointProperty += utils.GetHealthPointValueByLevel(level, view.GetWeaponType());
                _statsHandler.CurrentAttackPointProperty +=
                    utils.GetAttackValueByLevel(level, view.GetWeaponType());
            }
        }

        public bool IsPlayer() => isPlayer;

        private void SetupCollision(WeaponType type) => _crocodileController
            .GetCrocodileInfo().BathFastenersHandler
            .GetFasteners()
            .FirstOrDefault(obj => obj.GetCurrentEquipment()?.GetWeaponType() == type && obj.GetCurrentEquipment()?.isUsed == false)
            ?.GetCurrentEquipment()
            .SetupCollision(isPlayer);


        public virtual Transform GetBattlePoint() => 
            BattlePoints.Instance
                .GetPointTransform(isPlayer ? BattlePointType.PLAYER_BATTLE_POINT : BattlePointType.ENEMY_BATTLE_POINT);

        public Transform GetEndBattlePoint() => 
            BattlePoints.Instance.GetPointTransform(isPlayer ? BattlePointType.PLAYER_ZERO_POINT : BattlePointType.ENEMY_ZERO_POINT);

        public virtual Transform GetStartPoint() => 
            BattlePoints.Instance.GetPointTransform(isPlayer ? BattlePointType.PLAYER_START_POINT : BattlePointType.ENEMY_START_POINT);

        public abstract void SetCardsHandler(ICardsHandler cardsHandler);
        public abstract void AutoEquippingCrocodile();
    }
}