using System;
using System.Collections.Generic;
using System.Linq;
using App.Cards;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using Battle.Equipment;
using DG.Tweening;
using Library;
using Library.Utils;
using UnityEngine;

namespace App.Crocodile.Fasteners
{
    [Serializable]
    public class Fastener : AbstractFastener
    {
        [Header("DefaultFasteners")]
        [SerializeField] private Transform parent;
        [SerializeField] private ParticleSystem effect;
        [Header("OnValidate")] 
        [SerializeField] private List<Icons> _iconses;

        [SerializeField] private GameObject fastenerBlocker;
        private SpriteRenderer parentSpriteRenderer;
        private BathFastenersHandler _handler;
        private Vector3 localScale;
        private ICardInfo _card;
        private bool botWillUseThisFastener;
        public ICardInfo Card => _card;

        public bool BotWillUseThisFastener
        {
            get => botWillUseThisFastener;
            set => botWillUseThisFastener = value;
        }
        

        private void Start()
        {
            parentSpriteRenderer = parent.GetComponent<SpriteRenderer>();
            localScale = transform.localScale;
            _handler = GetComponentInParent<BathFastenersHandler>();
            DoFadeParentFastener(0,0.5f);
        }

        private void ClearFasteners()
        {
            if (IsFreeFastener()) return;
            _card = null;
            _equipment = null;
            Destroy(weapon);
            DoFadeParentFastener(0,0.5f);
        }

        public void BackCardFromFastener()
        {
            if (IsFreeFastener()) return;
            SetFastenerFree();
            _card = null;
            CardObject = null;
            botWillUseThisFastener = false;
            Destroy(weapon);
            DoFadeParentFastener(0,0.5f);
        }

        public override void SetupWeapon(ICardInfo cardInfo, bool isPlayer, int round, Card cardObject,
            bool isGarage = false)
        {
            ActiveRoundCount = 0;
            WeaponType weaponType = (WeaponType)cardInfo.GetCardType();
            RoundsDuration = _setup.GetWeaponView(weaponType).RoundDuration;
            Transform parentLocal = parent;
            CardObject = cardObject;
            if (weaponType == WeaponType.UMBRELLA || weaponType == WeaponType.SNAKE_TUBE)
            {
                parentLocal = _handler.GetFastenerByType(weaponType);
            }

            weapon = Instantiate(
                _setup.GetWeaponObject(weaponType), parentLocal);
            if (weaponType == WeaponType.UMBRELLA)
            {
                weapon.GetComponentInChildren<MeshRenderer>().sortingLayerName = "Default";
            }
            if (weaponType == WeaponType.SHOWER)
            {
                weapon.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
            }
            _card = cardInfo;
            if (weaponType.GetAttribute<WeaponData>().ActionType == WeaponActionType.ATTACK_LONG_DISTANCE ||
                weaponType.GetAttribute<WeaponData>().ActionType == WeaponActionType.ATTACK_LONG_DISTANCE)
                DoFadeParentFastener(1, 0.5f);
            if (effect != null)
                effect.Play();
            _equipment = weapon.GetComponent<IEquipment>();
            _equipment.Setup(_setup.GetWeaponView(weaponType), cardInfo.GetLevel(), this);

            callback?.Invoke(_equipment, true, _card.GetLevel());

            base.SetupWeapon(_card, isPlayer, round, cardObject);
        }

        public void DoFadeParentFastener(float alpha = 0, float duration = 0.4f)
        {
            parentSpriteRenderer.DOFade(alpha, duration);
        }

        public void SetScaleToStartValue() => transform.DOScale(localScale, 0.2f);
        
        
        private void OnValidate()
        {
            if(_iconses != null)
                fastenerRenderer.sprite = _iconses.FirstOrDefault(obj => obj.ActionType == _actionType)?.renderer;
        }

        [Serializable]
        class Icons
        {
            public WeaponActionType ActionType;
            public Sprite renderer;
        }

        public void BlockFastener()
        {
            fastenerBlocker.SetActive(true);
            _isBlocked = true;
        }
    }
}