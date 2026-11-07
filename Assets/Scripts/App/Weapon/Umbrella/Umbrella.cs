using App.Crocodile.Fasteners;
using App.PrototypeBattle;
using App.Weapon.WeaponBases;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace App.Weapon.Umbrella
{
    public class Umbrella : EquipmentBase
    {
        [Header("Child")] [SerializeField] private SkeletonAnimation _skeletonAnimation;
        [SerializeField] private ParticleSystem _effect;
        [SerializeField] private ParticleSystem smoke;
        [SerializeField] private Collider2D _collider2D;
        private bool isUmbrellaOpen;

        public override void EnableCollider(bool enable)
        {
            _collider2D.enabled = enable;
        }

        private void Start()
        {
            transform.position += new Vector3(0, 0.1f, 0);
            gameObject.layer = isPlayer ? LayerMask.NameToLayer("PlayerDefence") : LayerMask.NameToLayer("EnemyDefence");
            PreStartAction();
        }

        private void BeforeRound()
        {
            base.PreStartAction();
            if (isUmbrellaOpen) return;
            {
                _collider2D.enabled = true;
                _skeletonAnimation.AnimationState.ClearTracks();
                _skeletonAnimation.state.SetAnimation(0, "open_umbrella", false);
                smoke.Play();
                isUmbrellaOpen = true;
            }
        }
    
        public override void ResetWeapons()
        {
            base.ResetWeapons();
            if (!isUmbrellaOpen) return;
            _collider2D.enabled = false;
            _skeletonAnimation.AnimationState.ClearTracks();
            _effect.gameObject.SetActive(true);

            transform.DOShakeRotation(0.2f, new Vector3(0, 0, 17), 2).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                _skeletonAnimation.state.ClearTracks();
                _skeletonAnimation.state.SetAnimation(0, "close_umbrella", false);
                _effect.gameObject.SetActive(false);
            });
            isUmbrellaOpen = false;
        }
    
        protected void OnTriggerEnter2D(Collider2D c)
        {
            if (c.gameObject.name != "catapult_bomb(Clone)") return;
            _skeletonAnimation.state.ClearTracks();
            _skeletonAnimation.state.SetAnimation(0, "deformation", false);
        }

        public override void SetupCollision(bool isPlayer)
        {
            base.isPlayer = isPlayer;
        }

        public override void StopAnimation()
        {
            _skeletonAnimation.state.ClearTracks();
            _fader.DOFadeCroco(0, 1, 0);
        }

        public override void Setup(WeaponView view, int level, Fastener weaponFastener)
        {
            _view = view;
            base.level = level;
            CustomEventManager.BeforePreparingState.AddListener(BeforeRound);
            CustomEventManager.ResettingWeaponEvent.AddListener(ResetWeapons);
        }
    }
}