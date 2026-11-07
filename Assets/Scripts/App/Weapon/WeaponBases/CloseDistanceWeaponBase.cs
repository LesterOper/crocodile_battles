using App.Crocodile.Interfaces;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace App.Weapon.WeaponBases
{
    public class CloseDistanceWeaponBase : EquipmentBase
    {
        [Header("CloseDistanceWeapon")]
        [SerializeField] protected ParticleSystem closeWeaponEffect;
        [SerializeField] protected AnimationClip closeAnim;
        protected bool isAnimAndEffectPlayed;
        private IDamagable _damagable;
        [Inject] private IGameInfoService _gameInfoService;
        public override void SetupCollision(bool isPlayer)
        {
            base.isPlayer = isPlayer;
            gameObject.layer = isPlayer ? LayerMask.NameToLayer("CloseWeaponPlayer") : LayerMask.NameToLayer("CloseWeaponEnemy");
            foreach (var componentsInChild in gameObject.GetComponentsInChildren<Collider2D>())
            {
                componentsInChild.gameObject.layer = gameObject.layer;
            }

            if (isPlayer || _gameInfoService.IsBossFight()) return;
            if (closeWeaponEffect != null)
            {
                Vector3 scale = closeWeaponEffect.transform.localScale;
                scale = new Vector3(scale.x * -1,scale.y, scale.z );
                closeWeaponEffect.transform.localScale = scale;
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            if (!isAnimAndEffectPlayed)
            {
                if (!other.gameObject.CompareTag("WaterSurface"))
                {
                    if (other.gameObject.TryGetComponent(out IDamagable damagable))
                        damagable.ApplyDamage(utils.GetAttackValueByLevel(level, _view.Type));
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            if (!isAnimAndEffectPlayed)
            {
                if (!other.gameObject.CompareTag("WaterSurface"))
                {
                    if (other.gameObject.TryGetComponent(out IDamagable damagable))
                        damagable.ApplyDamage(utils.GetAttackValueByLevel(level, _view.Type));
                }
            }
        }
        
        public override void ResetWeapons()
        {
            base.ResetWeapons();
            isAnimAndEffectPlayed = false;
        }

        public override void StartAction()
        {
            base.StartAction();
            _animation.Play(closeAnim.name);
        }
        
        public override void StopAction()
        {
            base.StopAction();
            isAnimAndEffectPlayed = false;
        }
    
        #region AnimationEvent

        private void Effect()
        {
       
        }

        #endregion
    }
}