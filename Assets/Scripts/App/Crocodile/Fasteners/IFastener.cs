using System;
using App.Controllers;
using App.PrototypeBattle;
using App.SetupsScript;
using Battle.Equipment;
using UnityEngine;

namespace App.Crocodile.Fasteners
{
    public interface IFastener
    {
        IFastener GetFastener();
        Transform GetFastenerTransform();
        IEquipment GetCurrentEquipment();
        bool IsFreeFastener(WeaponType type = WeaponType.NONE);
        void SetupWeapon(ICardInfo cardInfo, bool isPlayer, int round, Card cardObject, bool isGarage = false);
        WeaponActionType GetActionType();
        GameObject GetWeaponObject();
        void AddListener(Action<IEquipment , bool, int> callback);
        void ActivateWeapon(ActivationState activationState);
        void ActivateWeapon(WeaponType type);
        void DeactivateWeapon(WeaponActionType type);
    }
}