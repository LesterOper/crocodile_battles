using System;
using App.DAO;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Weapon.WeaponBases;
using CodeBase.Infrastructure.Services;
using Library;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Dialogs.Merge
{
    public abstract class AbstractDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler
    {
        protected Camera _camera;
        protected float mZCoord;
        private Transform parentGrid;
        protected Vector3 mOffset;
        protected Transform parentWhileDragging;
        protected bool dragBlocked;
        protected BathSetup _bathSetup;
        protected WeaponSetup _weaponSetup;
        public void BlockedDrag(bool blocked) => dragBlocked = blocked;
    
        protected virtual void Start()
        {
            parentGrid = transform.parent;
            parentWhileDragging = parentGrid.parent.parent.parent;
            _camera = Camera.main;
        }

        public void SetupSetups(BathSetup bathSetup, WeaponSetup weaponSetup)
        {
            _bathSetup = bathSetup;
            _weaponSetup = weaponSetup;
        }

        /// <summary>
        /// For merge dialog
        /// </summary>
        public virtual void Setup(ICardInfo cardInfo, MergePlace left, MergePlace right){}
        /// <summary>
        /// For choosing cards for battle
        /// </summary>
        public virtual void Setup(ICardInfo info, Transform target, Action onDragCompleteSuccessfully){}
        /// <summary>
        /// For bath cards
        /// </summary>
        public virtual void Setup(ICardInfo data, bool clone, Transform target){}
        /// <summary>
        /// For battle cards
        /// </summary>
        public virtual void Setup(WeaponView weaponView, ICardInfo cardInfo, WeaponEquipingHandler equipingHandler){}

        public virtual void SetMana(bool mana)
        {
        }
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            mZCoord = _camera.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = transform.position - GetMouseWorldPos();
        }

        public abstract void OnPointerUp(PointerEventData eventData);

        public virtual void OnDrag(PointerEventData eventData)
        {
            if(!dragBlocked)
                transform.position = GetMouseWorldPos() + mOffset;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if(!dragBlocked)
                transform.SetParent(parentWhileDragging);
        }

        protected virtual  Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = mZCoord;
            return _camera.ScreenToWorldPoint(mousePoint);
        }

        protected abstract void ResetCard();
    }
}