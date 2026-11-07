using System;
using App.Controllers;
using App.PrototypeBattle;
using App.SetupsScript;
using Battle.Equipment;
using Library.Utils;
using UnityEngine;

namespace App.Crocodile.Fasteners
{
    public abstract class AbstractFastener : MonoBehaviour, IFastener
    {
        [SerializeField] protected WeaponSetup _setup;
        [SerializeField] protected SpriteRenderer fastenerRenderer;
        [SerializeField] protected WeaponActionType _actionType;
        protected bool _isBlocked;
        protected IEquipment _equipment;
        protected GameObject weapon;
        protected Action<IEquipment, bool, int> callback;
        public int ActiveRoundCount = 0;
        public int RoundsDuration = 0;
        public GameObject Weapon => weapon;
        public Card CardObject { get; set; }
        public IFastener GetFastener() => this;
    
        public Transform GetFastenerTransform() => transform;

        public IEquipment GetCurrentEquipment() => _equipment;

        public virtual bool IsFreeFastener(WeaponType type = WeaponType.NONE)
        {
            if (_isBlocked)
                return false;
           return _equipment == null;
        }

        protected void SetFastenerFree() => _equipment = null;

        public virtual WeaponActionType GetActionType() => _actionType;
        public GameObject GetWeaponObject() => weapon;

        public void AddListener(Action<IEquipment, bool, int > callbackLocal) => callback = callbackLocal;
        public void ActivateWeapon(ActivationState activationState)
        {
            if (_equipment?.GetActivationState() == activationState)
            {
                ActiveRoundCount++;
                if(GetCurrentEquipment()?.GetWeaponType() != WeaponType.PADDLE)
                    _equipment?.StartAction();
            }
        }

        public void ActivateWeapon(WeaponType type)
        {
            if(_equipment?.GetWeaponType() == type)
                _equipment?.StartAction();
        }

        public void DeactivateWeapon(WeaponActionType type)
        {
            if(_equipment?.GetWeaponView().Type.GetAttribute<WeaponData>().ActionType == type)
                _equipment?.StopAction();
        }
    
        public virtual void SetupWeapon(ICardInfo cardInfo ,bool isPlayer, int round, Card cardObject, bool isGarage = false) { }
    }
}
