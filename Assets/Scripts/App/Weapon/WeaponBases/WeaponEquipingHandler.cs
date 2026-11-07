using App.Crocodile.Fasteners;
using App.Crocodile.Implementations;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using Battle.Equipment;
using BattleStates.States;
using Library;
using Library.Utils;
using UnityEngine;
using Zenject;

namespace App.Weapon.WeaponBases
{
    public class WeaponEquipingHandler : MonoBehaviour
    {
        private OneActionEquipmentBase oneActionWeapon;
        private ICrocodileController _controller;
        private BathFastenersHandler bathFastenersHandler;
        private WeaponView _oneActionWeaponView;
        private bool isPlayer;
        private Card _currentBombCard;
        private WeaponView _currentBombView;
        private SignalBus _signalBus;
        private bool _isBombEquipped;

        public BathFastenersHandler GetBathFastenerHandler() => bathFastenersHandler;
        
        void Start()
        {
            _controller = GetComponent<CrocodileController>();
            bathFastenersHandler = _controller.GetCrocodileInfo().BathFastenersHandler;
        }
        
        public void UseOneActionWeapon(WeaponView view, int level, Card cardObject)
        {
            GameObject weaponGameObject = Instantiate(view.WeaponObject, bathFastenersHandler.BathRigidbody.transform);
            OneActionEquipmentBase oneActionEquipmentBase = weaponGameObject.GetComponent<OneActionEquipmentBase>();
            oneActionEquipmentBase.SetupCollisionBomb(isPlayer);
            AddEquipments(oneActionEquipmentBase, view, level);
            bathFastenersHandler.AddOneActionEquipment(oneActionEquipmentBase, cardObject);
            if (_currentBombCard != null && _currentBombView != null)
            {
                SwapBombWeapon();
            }
            
            if (view.Type == WeaponType.MINA_BOMB || view.Type == WeaponType.BOMB_ILLNESS)
            {
                _currentBombCard = cardObject;
                _currentBombView = view;
                _isBombEquipped = true;
            }
            _controller.GetCrocodileInfo().StatsHandler.CurrentManaPoint -=
                view.Mana;
        }

        public void ClearBombSlot()
        {
            _currentBombCard = null;
            _currentBombView = null;
            _isBombEquipped = false;
        }
        private void SwapBombWeapon()
        {
            _currentBombCard.gameObject.SetActive(true);
            _currentBombCard.GetComponent<Card>().CardStateHandlerProperty.SetCardUsed(false);
        }

        private void UseHealWeapon(WeaponView view, int level, Card cardObject)
        {
            _controller.GetCrocodileInfo().StatsHandler.CurrentManaPoint -=
                view.Mana;
            GameObject weaponGameObject = Instantiate(view.WeaponObject, bathFastenersHandler.GetFastenerByType(view.Type));
            weaponGameObject.layer = LayerMask.NameToLayer(isPlayer ? "Player" : "Enemy");
            weaponGameObject.transform.localScale = Vector3.one * 0.8f;
            IEquipment equipment = weaponGameObject.GetComponent<IEquipment>();
            equipment.SetupCollision(isPlayer);
            AddEquipments(equipment, view, level);
            bathFastenersHandler.AddOneActionEquipment(equipment, cardObject);
            _signalBus.Fire(new RemoveCardFromListSignal(){Card = cardObject});
        }
        
        public void AddEquipments(IEquipment equipment, WeaponView view, int level)
        {
            equipment.Setup(view, level, null);
            _controller.WeaponEquipped(equipment, true, level);
        }
        
        public Vector3 GetFastenerByWeaponActionType(WeaponActionType weaponActionType) =>
            bathFastenersHandler.GetFastenerTransformByWeaponActionType(weaponActionType).position;
        
        public void EquipWeapon(ICardInfo cardInfo, Card cardObject, IFastener target = null, int level = 1)
        {
            WeaponType weaponType = (WeaponType) cardInfo.GetCardType();
            WeaponView view = bathFastenersHandler.Setup.GetWeaponView(weaponType);
            
           // cardObject.WeaponEquipped = true;

           if (weaponType.GetAttribute<WeaponData>().IsArmor)
           {
               long armor = new CardCharactaristicsUtils().GetArmorValueByLevel(level, weaponType);
               _signalBus.Fire(new SwitchArmorSignal(){IsPlayer = isPlayer, IsActive = true, MaxArmor = armor});
               _controller.GetCrocodileInfo().StatsHandler.CurrentArmorProperty += armor;
           }
           
            if (weaponType.GetAttribute<WeaponData>().IsNeedFastener)
            {
                _controller.GetCrocodileInfo().StatsHandler.CurrentManaPoint -=
                    view.Mana;
                /*if (weaponType == WeaponType.UMBRELLA)
                    _controller.GetCrocodileInfo().LongDistanceImmun = true;
                if (weaponType == WeaponType.SNAKE_TUBE)
                    _controller.GetCrocodileInfo().CloseDistanceImmun = true;*/
                target.SetupWeapon(cardInfo, isPlayer, BattleViewController.roundIndex, cardObject);
            }
            else
            {
                if (weaponType.GetAttribute<WeaponData>().ActionType != WeaponActionType.ONE_ACTION_HEAL)
                    UseOneActionWeapon(view, level, cardObject);
                else UseHealWeapon(view, level, cardObject);
            }
            
        }
        
        public void SetupWeaponEquipping(bool isPlayer, SignalBus signalBus)
        {
            _signalBus = signalBus;
            this.isPlayer = isPlayer;
        }

        public bool IsBombEquipped()
        {
            return _isBombEquipped;
        }
    }
}