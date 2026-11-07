using App.Audio;
using App.Crocodile.Fasteners;
using App.Crocodile.Interfaces;
using App.PrototypeBattle;
using App.Weapon.WeaponBases;
using DG.Tweening;
using Library;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace App.Weapon.MinaBomb
{
    public class MinaBomb : OneActionEquipmentBase
    {
        [Header("MinaBomb")] 
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject explosions;

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private PolygonCollider2D _collider2D;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject trail;
        private bool isEffectShown;
        private SkeletonAnimation _skeleton;
        
        private void OnDestroy()
        {
            _skeleton.state.Event -= ThrowBomb;
        }
        
        public override void StartAction()
        {
            base.StartAction();
            _skeleton.state.Event += ThrowBomb;
            gameObject.SetActive(true);
            transform.SetParent(null);
            gameObject.layer = LayerMask.NameToLayer(isPlayer ? "BombPlayer" : "BombEnemy");
            _skeleton.state.SetAnimation(0,"throw_bomb", false);
            _skeleton.state.AddAnimation(0, "idle", false,0);
        }
        
        private void ThrowBomb(TrackEntry entry, Spine.Event e)
        {
            bool match = (e.Data.Name.Equals("throw"));
            if (match)
            {
                rb.simulated = true;
                rb.AddForce(new Vector2(isPlayer ? 4.5f : -4.5f, 7f) * 45, ForceMode2D.Force);
                trail.SetActive(true);
                _skeleton.state.Event -= ThrowBomb;
            }
              
        }
        
        public override void SetupCollision(bool isPlayer) => this.isPlayer = isPlayer;

        public override void Setup(WeaponView view, int level, Fastener weaponFastener)
        {
            base.Setup(view, level, weaponFastener);
            _skeleton = _controller.GetCrocodileInfo().BathFastenersHandler.GetSkeletonAnimations();
            _controller.GetCrocodileInfo().BathFastenersHandler.TakeBombAnim();
            transform.localPosition = new Vector3(-0.41f, 1.106f, 0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("WaterSurface"))
            {
                GameObject efffect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(efffect, 0.3f);
                rb.bodyType = RigidbodyType2D.Static;
                transform.DOMoveY(transform.position.y + 0.1f, 1).SetEase(Ease.OutSine).OnComplete(() =>
                {
                }).SetLoops(-1, LoopType.Yoyo);
            }

            /*if (other.TryGetComponent(out SnakeTube.SnakeTube tube))
            {
                Explosion();
                _renderer.DOFade(0, 0.2f).OnComplete(()=>Destroy(gameObject));
            }*/
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Transform collisionGameObject;
            if (!isEffectShown)
            {
                IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
                Explosion();
                _renderer.DOFade(0, 0.2f);
                if (damagable != null)
                {
                    collisionGameObject = other.gameObject.transform;
                    CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
                    damagable.ApplyDamage(utils.GetAttackValueByLevel(level, _view.Type), false); 
                    other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2((isPlayer?2f:-3f), 2) * 20, ForceMode2D.Force);
                    collisionGameObject.transform
                        .DORotate(
                            new Vector3(0, 0,
                                20 *(isPlayer?-1:1)), 0.2f).SetEase(Ease.Flash).OnComplete(() =>
                        {
                            collisionGameObject.transform
                                .DORotate(
                                    new Vector3(0, 0,
                                        0), 0.1f).OnComplete(() => Destroy(gameObject));
                        });
                    return;
                }
                Destroy(gameObject);
            }
        }

        private void Explosion()
        {
            _soundPlayer.PlaySound(shootAudio.name, AudioMixerController.MasterMixerGroup.SFX, 0.8f);
            _collider2D.enabled = false;
            isEffectShown = true;
            GameObject efffect =
                Instantiate(explosions, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
            Destroy(efffect, 1.5f);
        }

        public override void StopAction() { }
    }
}
