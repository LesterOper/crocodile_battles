using App.Audio;
using App.Signals;
using DG.Tweening;
using Library;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Crocodile
{
    public class BathEffectController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;
        [SerializeField] private ParticleSystem effectHealing;
        [SerializeField] private SpriteRenderer plesen;
        private SignalBus _signalBus;
        private bool _isPlayer;
        private SoundPlayer _soundPlayer;
        [Inject]
        public void Costruct(SignalBus signalBus, SoundPlayer soundPlayer)
        {
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
        }
        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<BathEffectSignal>(PlayEffect);
            _signalBus.TryUnsubscribe<BathEffectSignal>(PlayEffect);
        }

        private void Start()
        {
            _isPlayer = transform.parent.parent.gameObject.layer == LayerMask.NameToLayer("Player");
            _signalBus.Subscribe<BathEffectSignal>(PlayEffect);
        }

        private void PlayEffect(BathEffectSignal signal)
        {
            if (signal.IsPlayer == _isPlayer)
            {
                switch (signal.Effect)
                {
                    case EffectType.BOMB:
                        StartEffect();
                        break;
                    case EffectType.HEAL:
                        StartHealEffect();
                        break;
                }
            }
        }
        
        private void StartHealEffect()
        {
            _soundPlayer.PlaySound("card_health_01", AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            effectHealing.Play();
        }
        
        private void StartEffect()
        {
            effect.Play();
            plesen.DOFade(1, 0.25f);
        }
    }
}
