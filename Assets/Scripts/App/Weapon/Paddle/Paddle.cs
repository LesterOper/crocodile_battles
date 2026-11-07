using App.Weapon.WeaponBases;
using DG.Tweening;
using UnityEngine;

namespace App.Weapon.Paddle
{
    public class Paddle : CloseDistanceWeaponBase
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!isAnimAndEffectPlayed)
            {
                if (!other.gameObject.CompareTag("WaterSurface") && other.gameObject.layer !=
                                                                 LayerMask.NameToLayer("BombPlayer")
                                                                 && other.gameObject.layer !=
                                                                 LayerMask.NameToLayer("BombEnemy"))
                {
                    isAnimAndEffectPlayed = true;
                    _animation.Stop();
                    transform.DOLocalRotate(new Vector3(0, 0, 60), 0.35f);
                    other.transform.DOMoveY(other.transform.position.y - 0.2f, 0.2f);
                    GameObject particle = Instantiate(closeWeaponEffect.gameObject,
                        GetComponent<Collider2D>().ClosestPoint(other.transform.position),
                        closeWeaponEffect.transform.rotation);
                    particle.transform.SetParent(other.transform);
                    Destroy(particle, 0.3f);
                }
            }
        }
    }
}
