using System.Collections.Generic;
using App.Controllers;
using App.Crocodile.Fasteners;
using App.PrototypeBattle;
using App.SetupsScript;
using UnityEngine;

namespace Battle.Equipment
{
    public interface IEquipment
    {
        GameObject GetObject();
        void UseWeapon(bool isAttack);
        bool isUsed { get; set; }
        void PreStartAction();
        void StartAction();
        void StopAction();
        void StopAnimation();
        void ResetWeapons();
        WeaponType GetWeaponType();
        ActivationState GetActivationState();
        WeaponActionType GetWeaponActionType();

        void SetupCollision(bool isPlayer);
        void SetupCollisionBomb(bool isPlayer);
        
        List<WeaponType> GetContrWeaponType();
        
        void Setup(WeaponView view, int level, Fastener weaponFastener);

        void Heal();

        WeaponView GetWeaponView();

        bool IsWeaponFinishedWork();

        void DoFadeWeapon(float alpha, float duration);
        void EnableCollider(bool enable);
    }
}