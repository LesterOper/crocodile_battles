using System.Collections.Generic;
using System.Linq;
using App.PrototypeBattle;
using Library.Utils;
using UnityEngine;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "weapon_setup", menuName = "Setup/WeaponSetup")]
    public class WeaponSetup : ScriptableObject
    {
        [SerializeField] private List<WeaponView> WeaponsContainer;

        public List<WeaponView> GetList() => WeaponsContainer;
        public GameObject GetWeaponObject(WeaponType type) => Get(type)?.WeaponObject;
        public WeaponView GetWeaponView(WeaponType type) => Get(type);
        public WeaponActionType GetWeaponActionType(WeaponType type) => Get(type).Type.GetAttribute<WeaponData>().ActionType;

        public Sprite GetWeaponSprite(WeaponType type) => Get(type)?.WeaponSprite;
        private WeaponView Get(WeaponType type) => WeaponsContainer.FirstOrDefault(v => v.Type == type);
    }


    public enum WeaponActionType
    {
        NONE = 0,
        ATTACK_LONG_DISTANCE = 2,
        DEFENCE = 3,
        ATTACK_CLOSE_DISTANCE = 4,
        ONE_ACTION = 5,
        ONE_ACTION_BOMB = 6,
        ONE_ACTION_HEAL = 7,
        ANTI_FREEZE = 8,
    }
}