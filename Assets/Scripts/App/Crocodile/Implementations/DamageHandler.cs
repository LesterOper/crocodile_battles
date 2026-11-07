using System;
using System.Collections.Generic;
using App.Achievements;
using App.Crocodile.Interfaces;
using App.Signals;
using Library;
using Library.Collections;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Crocodile.Implementations
{
    public class DamageHandler : MonoBehaviour, IDamagable
    {
        private ICrocodileController _controller;
        private SignalBus _signalBus;
        private bool isDoubleDamage;
        private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> takeDamageSFX;
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        public bool IsBombDebufActive { get; set; }
        

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<DebufDamageSignal>(ApplyDebufDamage);
            _signalBus.TryUnsubscribe<GameOverSignal>(DisableDamage);
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _controller = GetComponentInParent<ICrocodileController>();
            this.IsBombDebufActive = false;
            _signalBus.Subscribe<GameOverSignal>(DisableDamage);
            _signalBus.Subscribe<DebufDamageSignal>(ApplyDebufDamage);
            _signalBus.Subscribe<ActivateBoosterSignal>(ActivateBooster);
        }

        private void ActivateBooster(ActivateBoosterSignal signal)
        {
            if(signal.BoosterType != BoosterType.DOUBLE_DAMAGE)
                return;
            if (!_controller.GetCrocodileInfo().Crocodile.IsPlayer() &&
                !signal.IsUsed)
                isDoubleDamage = true;
            else
                isDoubleDamage = false;
        }

        public void ApplyDamage(float damage, bool needShake = true, bool closeDamage = true)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(takeDamageSFX.Random(), 0.8f);
            }
            if (isDoubleDamage)
                damage *= 2;
            if (closeDamage && _controller.GetCrocodileInfo().CloseDistanceImmun)
                return;
            if (!closeDamage && _controller.GetCrocodileInfo().LongDistanceImmun)
                return;
            if (_controller.GetCrocodileInfo().StatsHandler.IsArmorActive())
                _controller.GetCrocodileInfo().StatsHandler.CurrentArmorProperty -= damage;
            else
                _controller.GetCrocodileInfo().StatsHandler.CurrentHealthPointProperty -= damage;
            _controller.GetCrocodileInfo().BathFastenersHandler.HitEffect(needShake);
        }

        private void ApplyDebufDamage()
        {
            if (this.IsBombDebufActive)
            {
                _signalBus.Fire(new BathEffectSignal(){IsPlayer = gameObject.layer == LayerMask.NameToLayer("Player"), Effect = EffectType.BOMB});
                ApplyDamage(10);
            }
        }

        private void DisableDamage(GameOverSignal signal) => Destroy(this);
    }
}