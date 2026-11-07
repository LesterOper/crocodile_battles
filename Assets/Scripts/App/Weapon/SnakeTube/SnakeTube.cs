using App.Audio;
using App.Crocodile.Implementations;
using App.Weapon.WeaponBases;
using UnityEngine;
using UnityEngine.Serialization;

namespace App.Weapon.SnakeTube
{
    public class SnakeTube : CloseDistanceWeaponBase
    {
        [Header("SnakeTube")] [SerializeField] private GameObject effect;
        [FormerlySerializedAs("collider")] [SerializeField] private Collider2D Сollider;
        public override void EnableCollider(bool enable)
        {
            Сollider.enabled = enable;
        }

        public override void SetupCollision(bool isPlayer)
        {
            base.SetupCollision(isPlayer);
            _soundPlayer.PlaySound(preStartAudioClip.name, AudioMixerController.MasterMixerGroup.SFX, 0.5f);
            gameObject.layer = isPlayer ? LayerMask.NameToLayer("PlayerDefence") : LayerMask.NameToLayer("EnemyDefence");
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            base.OnCollisionEnter2D(other);
            if (!isAnimAndEffectPlayed)
            {
                if (!other.gameObject.CompareTag("WaterSurface"))
                {
                    isAnimAndEffectPlayed = true;
                    _animation.Play(closeAnim.name);
                    GameObject effect = Instantiate(closeWeaponEffect.gameObject, transform);
                    Destroy(effect, 0.7f);
                    CrocodileController controller = other.collider.GetComponentInParent<CrocodileController>();
                    if (controller != null)
                    {
                        controller.GetCrocodileInfo().MoverHandler.StopMoveCoroutine(10, 0);
                    }
                
                    effect.SetActive(true);
                }
            }
        }

        public override void StartAction()
        {
            isAnimAndEffectPlayed = false;
        }

        public override void StopAction()
        {
        }

        #region Animation

        private void StartEffect() => effect.SetActive(true);
        private void StopEffect() => effect.SetActive(false);

        #endregion
    }
}