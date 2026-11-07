using System.Collections.Generic;
using App.Audio;
using App.Controllers;
using App.Crocodile.Fasteners;
using App.Crocodile.Implementations;
using App.PrototypeBattle;
using App.SetupsScript;
using App.View;
using Battle.Equipment;
using Library;
using Library.Common;
using UnityEngine;
using Zenject;

namespace App.Weapon.WeaponBases
{
    public class EquipmentBase : MonoBehaviour, IEquipment
    {
        [Header("Base")]
        [SerializeField] protected WeaponType type;
        [SerializeField] protected ActivationState _state;
        [SerializeField] protected WeaponActionType _actionType;
        [SerializeField] protected Animation _animation;
        [SerializeField] protected AnimationClip preStartAnimation;
        [SerializeField] protected AnimationClip resettingAnimation;
        [SerializeField] protected AnimationClip shoot, reload;
        [SerializeField] protected AudioClip preStartAudioClip;
        [SerializeField] protected AudioClip resettingAudioClip;
        [SerializeField] protected AudioClip shootAudio, reloadAudio;
        protected WeaponView _view;
        protected CrocodileController _controller;
        protected int level;
        protected bool isPlayer;
        protected bool isWeaponFinishedWork;
        protected CrocoItemsDOFader _fader;
        protected SignalBus _signalBus;
        protected SoundPlayer _soundPlayer;
        private float _lastTimeClick;

        [Inject]
        public void Construct(SignalBus signalBus, SoundPlayer soundPlayer)
        {
            _signalBus = signalBus;
            _soundPlayer = soundPlayer;
        }
        protected virtual void Awake()
        {
            _controller = GetComponentInParent<CrocodileController>();
            _fader = GetComponent<CrocoItemsDOFader>();
        }
        
        public GameObject GetObject() => gameObject;

        public virtual void UseWeapon(bool isAttack)
        {
            if(isAttack)
                StartAction();
            else StopAction();
        }

        public bool isUsed { get; set; }

        public virtual void PreStartAction()
        {
            if(preStartAnimation != null)
            {
                _animation.Play(preStartAnimation.name);
            }

            if (preStartAudioClip != null)
            {
                _soundPlayer.PlaySound(preStartAudioClip.name, AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            }
        }
        
        public virtual void StartAction()
        {
            if(shoot != null)
            {
                _animation.Play(shoot.name);
            }

            if (shootAudio != null)
            {
                _soundPlayer.PlaySound(shootAudio.name, AudioMixerController.MasterMixerGroup.SFX, 0.8f);
            }
        }

        public virtual void StopAction()
        {
            if (reload != null)
            {
                _animation.Play(reload.name);
            }

            if (reloadAudio != null)
            {
                //_soundPlayer.PlaySound(reloadAudio.name, AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            }
            isWeaponFinishedWork = false;
        }

        public virtual void StopAnimation()
        {
            if(_animation != null)
                _animation.Stop();
            if (_fader == null) _fader = GetComponent<CrocoItemsDOFader>();
            _fader.DOFadeCroco(0, 1, 0);
        }

        public virtual void ResetWeapons()
        {
            if (resettingAnimation != null)
            {
                _animation.Play(resettingAnimation.name);
            }

            if (resettingAudioClip != null)
            {
                _soundPlayer.PlaySound(resettingAudioClip.name, AudioMixerController.MasterMixerGroup.SFX, 0.3f);
            }
        }

        public WeaponType GetWeaponType() => type;
        public ActivationState GetActivationState() => _state;

        public WeaponActionType GetWeaponActionType() => _actionType;

        public virtual void SetupCollision(bool isPlayer)
        {
            this.isPlayer = isPlayer;
           
            gameObject.layer = isPlayer ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");
        }

        public void SetupCollisionBomb(bool isPlayer) => this.isPlayer = isPlayer;
        

        public List<WeaponType> GetContrWeaponType()
        {
            return null;
        }

        public virtual void Setup(WeaponView view, int lvl, Fastener weaponFastener)
        {
            _view = view;
            this.level = lvl;
            CustomEventManager.PreparingWeaponEvent.AddListener(PreStartAction);
            CustomEventManager.ResettingWeaponEvent.AddListener(ResetWeapons);
        }

        public virtual void Heal() { }

        public WeaponView GetWeaponView() => _view;
        public bool IsWeaponFinishedWork() => isWeaponFinishedWork;
        public void DoFadeWeapon(float alpha, float duration)
        {
            _fader.DOFadeCroco(alpha, duration, 0);
        }

        public virtual void EnableCollider(bool enable)
        {
            
        }

        public bool IsPlayerWeapon() => isPlayer;

        #region Animation

        private void SetWeaponFinishedWork() => isWeaponFinishedWork = true;

        #endregion

        
    }
}