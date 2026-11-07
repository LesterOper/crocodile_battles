using System.Collections;
using App.Crocodile.Interfaces;
using App.SetupsScript;
using CodeBase.Infrastructure;
using UnityEngine;

namespace App.Controllers
{
    public class FightingController
    {
        private ICrocodileController crocoPlayer;
        private ICrocodileController crocoEnemy;
        private ICoroutineRunner  _runner;
        private bool startRound;
        private bool isAntiFreezeWorked;
        private bool isCloseDistanceWeaponsWorked;

        public void SetupFightingController(ICrocodileController crocoPlayer, ICrocodileController crocoEnemy,
            ICoroutineRunner coroutineRunner)
        {
            this.crocoEnemy = crocoEnemy;
            this.crocoPlayer = crocoPlayer;
            _runner = coroutineRunner;
        }

        public void AttackStarted()
        {
            PreActivateCrocoWeapons();
            startRound = true;
            _runner.StartCoroutine(FightCoroutine());
        }

        private IEnumerator FightCoroutine()
        {
            crocoPlayer.CheckMove();
            crocoEnemy.CheckMove();
            ActivateCrocoWeapons(ActivationState.BEFORE_TIMER);
            ActivateCrocoWeapons(ActivationState.UNTIL_MOVING);
            ActivateCrocoWeapons(ActivationState.BEFORE_MOVING);
            ActivateCrocoOneActionWeapons();
            yield return new WaitForSeconds(2);
            ActivateCrocoWeapons(ActivationState.CLOSE_DISTANCE);
            yield return new WaitForSeconds(1.5f);
            CustomEventManager.InvokeCompleteFightState();
        }

        public void AttackStopped()
        {
            crocoPlayer.StopAttack();
            crocoEnemy.StopAttack();
            DeActivateCrocoWeapons(WeaponActionType.ATTACK_LONG_DISTANCE);
            DeActivateCrocoWeapons( WeaponActionType.ATTACK_CLOSE_DISTANCE);
        }

        private void ActivateCrocoOneActionWeapons()
        {
            crocoPlayer.GetCrocodileInfo()
                .BathFastenersHandler
                .ActivateOneActionWeapons();
            crocoEnemy.GetCrocodileInfo()
                .BathFastenersHandler
                .ActivateOneActionWeapons();
        }
        
        private void ActivateCrocoWeapons(ActivationState activationState)
        {
            crocoPlayer.GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .ForEach(obj =>
                {
                    obj.ActivateWeapon(activationState);
                });
            crocoEnemy.GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .ForEach(obj =>
                {
                    obj.ActivateWeapon(activationState);
                });
        }
        private void DeActivateCrocoWeapons(WeaponActionType weaponActionType)
        {
            crocoPlayer.GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .ForEach(obj => obj.DeactivateWeapon(weaponActionType));
            crocoEnemy.GetCrocodileInfo()
                .BathFastenersHandler
                .GetFasteners()
                .ForEach(obj => obj.DeactivateWeapon(weaponActionType));
        }
    
        private void PreActivateCrocoWeapons()
        {
            CustomEventManager.InvokePreparingWeaponEvent();
        }
    }

    public enum ActivationState
    {
        NONE = 0,
        BEFORE_TIMER = 1,
        BEFORE_MOVING = 2,
        UNTIL_MOVING = 3,
        ANTI_FREEZE = 4,
        CLOSE_DISTANCE = 5,
        AFTER_CLOSE_DISTANCE = 6
    }
}