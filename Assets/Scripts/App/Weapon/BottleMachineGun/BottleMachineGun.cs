using System;
using System.Collections.Generic;
using App.Weapon.WeaponBases;
using Effects;
using UnityEngine;

namespace App.Weapon.BottleMachineGun
{
    public class BottleMachineGun : LongDistanceWeaponBase
    {
        [SerializeField] private Projectile bottle;
        [SerializeField] private List<Transform> parents;
        [SerializeField] private ParticleSystem _system;

        #region AnimationEvents

    
        public void ShootBottle(int index)
        {
            Projectile Bottle = EffectPool.Instance.GetBottle().Obtain().GetComponent<Projectile>();
            Bottle.gameObject.transform.position = parents[index].position;
            Bottle.gameObject.transform.rotation = bottle.transform.rotation;
            Bottle.Setup(new Vector3(2f, isPlayer ? 0.45f : -0.45f), isPlayer ? 8f : -8f, 26,GetDamage());
            PlayParticle();
            Bottle.transform.localScale = Vector3.one*0.65f;
            Bottle.gameObject.layer = LayerMask.NameToLayer(isPlayer ? "ProjectilePlayer" : "ProjectileEnemy");
        }

        private void PlayParticle()
        {
            _system.Play();
        }

        private void StopParticle() => _system.Stop();

        #endregion

        private long GetDamage()
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            long attack = utils.GetAttackValueByLevel(level, _view.Type);
            return (long)Math.Round((attack / 5d), MidpointRounding.AwayFromZero);
        }
    }
}
