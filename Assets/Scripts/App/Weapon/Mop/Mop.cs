using App.Crocodile.Implementations;
using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Mop
{
    public class Mop : CloseDistanceWeaponBase
    {
        protected override void Awake()
        {
            base.Awake();
            transform.localScale = Vector3.one;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            _animation.Play(closeAnim.name);
            closeWeaponEffect.Play();
            other.GetComponentInParent<CrocodileController>()?.GetCrocodileInfo().MoverHandler.StopMoveCoroutine(15, 0);
        }

        public override void StartAction()
        {
        
        }

        public override void StopAction()
        {
        
        }
    }
}
