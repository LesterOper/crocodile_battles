using System;
using System.Collections.Generic;
using App.Weapon.WeaponBases;
using Effects;
using UnityEngine;

namespace App.Weapon.TubeGun
{
    public class TubeGun : LongDistanceWeaponBase
    {
        [SerializeField] private Projectile _bottle;
        [SerializeField] private List<Transform> parents;
        private Projectile Bottle;
    
        #region AnimationEvent

        private void ShootBottle(int index)
        {
            Bottle = EffectPool.Instance.GetBottle().Obtain().GetComponent<Projectile>();
            Bottle.gameObject.transform.position = parents[index].position;
            Bottle.gameObject.transform.rotation = _bottle.transform.localRotation;
            float changeCoefficient = ChangingForceXValue(index);

            float forceX = isPlayer ? changeCoefficient : -changeCoefficient;
            float forceY = 1.15f;
            Vector3 force = new Vector3(forceX, forceY, 0);
       
            Bottle.Setup(force, 6.5f ,   0,  GetDamage());
            Bottle.gameObject.layer = LayerMask.NameToLayer(isPlayer ? "ProjectilePlayer" : "ProjectileEnemy");
        }

        private float ChangingForceXValue(int index) => 1.5f - (0.1f * index);

        #endregion
        
        private long GetDamage()
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            long attack = utils.GetAttackValueByLevel(level, _view.Type);
            return (long) Math.Round( attack / 5d, MidpointRounding.AwayFromZero);
        }
    }
}
