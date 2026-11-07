using App.Audio;
using App.Crocodile.Fasteners;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using App.Signals;
using App.Weapon.WeaponBases;
using DG.Tweening;
using Library;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace App.Weapon.Bomb
{
    public class Bomb : OneActionEquipmentBase
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject waterEffect;
        [SerializeField] private GameObject trail;
        private int layer;
        private Tween _takeBombTween;
        private SkeletonAnimation _skeleton;

        private void OnDestroy()
        {
            _skeleton.state.Event -= ThrowBomb;
        }

        public override void Setup(WeaponView view, int levelParam, Fastener weaponFastener)
        {
            base.Setup(view, levelParam, weaponFastener);
            _skeleton = _controller.GetCrocodileInfo().BathFastenersHandler.GetSkeletonAnimations();
            gameObject.layer = LayerMask.NameToLayer(isPlayer ? "BombPlayer" : "BombEnemy");
            _controller.GetCrocodileInfo().BathFastenersHandler.TakeBombAnim();
            _takeBombTween = transform.DOLocalMove(new Vector3(-0.41f, 1.106f, 0f), 0.5f);
            _skeleton.state.Event += ThrowBomb;
        }
        
        public override void StartAction()
        {
            base.StartAction();
            gameObject.SetActive(true);
            
            Shoot();
        }

        private void Shoot()
        {
            _skeleton.state.SetAnimation(0,"throw_bomb", false);
            _skeleton.state.AddAnimation(0, "idle", false,0);
        }

        private AnimationState.TrackEntryDelegate stateOnComplete()
        {
            return entry =>
            {
                _skeleton.state.SetAnimation(0, "idle", false);
                _skeleton.state.Complete -= stateOnComplete();
            };
        }

        private void ThrowBomb(TrackEntry entry, Spine.Event e)
        {
            bool match = (e.Data.Name.Equals("throw"));
            if (match)
            {
                _takeBombTween.Kill();
                transform.SetParent(null);
                rb.simulated = true;
                rb.AddForce(new Vector2(isPlayer ? 3.3f : -3.3f, 3.1f) * 40, ForceMode2D.Force);
                trail.SetActive(true);
                _skeleton.state.Event -= ThrowBomb;
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
            {
                if (damagable == null)
                {
                    GameObject efffect = Instantiate(waterEffect, transform.position, Quaternion.identity);
                    Destroy(effect, 0.7f);
                }
                else
                {
                    if (!_controller.GetCrocodileInfo().LongDistanceImmun)
                    {
                        _soundPlayer.PlaySound(preStartAudioClip.name, AudioMixerController.MasterMixerGroup.SFX, 0.8f);
                        BombCollision();
                        damagable.ApplyDamage(10, true, false);
                        damagable.IsBombDebufActive = true;
                    }
                }
                Destroy(gameObject);
            }
            else
            {
                if (other.name == "umbrella(Clone)")
                {
                    Destroy(gameObject);
                }
                else if (other.CompareTag("Bath"))
                {
                    IDamagable damag = other.GetComponentInParent<IDamagable>();
                    if (!_controller.GetCrocodileInfo().LongDistanceImmun)
                    {
                        if (damag != null)
                        {
                            damag.IsBombDebufActive = true;
                            damag.ApplyDamage(10, true, false);
                        }

                        _soundPlayer.PlaySound(preStartAudioClip.name, AudioMixerController.MasterMixerGroup.SFX, 1);
                        BombCollision();
                    }

                    Destroy(gameObject);
                }
            }
        }
        private void BombCollision()
        {
            GetComponent<CircleCollider2D>().enabled = false;
            rb.simulated = false;
            effect.GetComponent<ParticleSystem>().Play();
            _signalBus.Fire(new BathEffectSignal(){IsPlayer = !isPlayer, Effect = EffectType.BOMB});
        }
    }
}