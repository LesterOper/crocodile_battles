using App.Crocodile.Interfaces;
using App.Weapon.Catapult;
using DG.Tweening;
using Effects;
using UnityEngine;

namespace App.Weapon.WeaponBases
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected GameObject effectHitCrocodile;
        [SerializeField] protected GameObject effectCollideWaterSurface;
        [SerializeField] protected GameObject bigSplashWithStones;
        [SerializeField] protected Collider2D colliderTr;
        protected Rigidbody2D rb;
        protected SpriteRenderer _renderer;

        protected float throwSpeed = 1.5f;

        protected bool isActive = false;
        protected Vector3 force;

        protected long damage;
        protected TrailRenderer _trail;
    
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _trail = GetComponentInChildren<TrailRenderer>();
        }

        public virtual void Setup(Vector3 force, float speed, float angle,  long damage)
        {
            this.damage = damage;
            this.force = force;
            throwSpeed = speed;
            isActive = true;
            _trail.Clear();
            gameObject.SetActive(true);
            AddForce();
        }
    
        protected virtual void AddForce()
        {
            rb.simulated = true;
            rb.AddForce(force * throwSpeed, ForceMode2D.Impulse);
        }

        private void Update()
        {
            if (isActive)
            {
                transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            bool isWater = false;
            Vector2 col = other.contacts[0].point;
            GameObject effect;
            if (other.gameObject.CompareTag("WaterSurface"))
            {
                effect = EffectPool.Instance.GetEffectSolidWithWater().Obtain();
                isWater = true;
            }
            else
            {
                effect = EffectPool.Instance.GetEffectSolidWithCroco().Obtain();
                isWater = false;
            }


            Effect(col, effect, isWater);
            TryGetComponentFromCollision(other.gameObject);
        }

        protected virtual void TryGetComponentFromCollision(GameObject target)
        {
            if (!target.CompareTag("WaterSurface"))
            {
                if (target.TryGetComponent(out IDamagable damageHandler))
                {
                    damageHandler.ApplyDamage(damage, true, false);
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            bool isWater = false;
            Vector2 point = other.ClosestPoint(transform.position);
            GameObject effect;
            if (other.gameObject.CompareTag("WaterSurface"))
            {
                effect = EffectPool.Instance.GetEffectSolidWithWater().Obtain();
                isWater = true;
            }
            else
            {
                effect = EffectPool.Instance.GetEffectSolidWithCroco().Obtain();
                isWater = false;
            }
            Effect(point, effect, isWater);
            TryGetComponentFromCollision(other.gameObject);

        }
        protected virtual void Effect(Vector2 col, GameObject effect, bool isWater = false)
        {
            effect.transform.position = col;
            effect.transform.rotation = Quaternion.identity;
            effect.SetActive(true);
            _renderer.DOFade(0, 0);
            colliderTr.isTrigger = true;
            rb.simulated = false;
            DOVirtual.DelayedCall(1f, () => { 
                effect.SetActive(false);
                if(!isWater)
                    EffectPool.Instance.GetEffectSolidWithCroco().Free(effect);
                else
                    EffectPool.Instance.GetEffectSolidWithWater().Free(effect);
            });

            if (this is CatapultBomb)
            {
                Destroy(gameObject);
                return;
            }
            
            gameObject.SetActive(false);
            _renderer.DOFade(1, 0);
            EffectPool.Instance.GetBottle().Free(gameObject);
        }

    }
}
