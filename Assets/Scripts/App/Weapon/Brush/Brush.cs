using App.Crocodile.Implementations;
using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Brush
{
    public class Brush : CloseDistanceWeaponBase
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!isAnimAndEffectPlayed)
            {
                if (!other.gameObject.CompareTag("WaterSurface"))
                {
                    isAnimAndEffectPlayed = true;
                    _animation.Play(closeAnim.name);
                    closeWeaponEffect.Play();
                    other.GetComponentInParent<CrocodileController>()?.GetCrocodileInfo().MoverHandler.StopMoveCoroutine(8, 0.5f);
                }
            }
        }

        public override void StartAction()
        {
        
        }
    }
}
