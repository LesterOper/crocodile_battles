using App.Audio;
using App.Crocodile.Interfaces;
using Library;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Weapon.WeaponBases
{
    public class AntiFreezeWeaponBase : EquipmentBase
    {
        [Header("AntiFreeze")] 
        [SerializeField] private AnimationClip cantWork;

        [SerializeField] private GameObject effect;
        [SerializeField] private bool moveToCrocoOrNot;
        
        private ICrocodileController anotherCroco;

        protected override void Awake()
        {
            base.Awake();
            CustomEventManager.AntiFreezeState.AddListener(StartAction);
        }
        private void Start()
        {
            if(SceneManager.GetActiveScene().name != "Main")
                anotherCroco = _controller.GetEnemy();
        }

        public override void UseWeapon(bool isAttack)
        {
            if(isAttack)
                StartAction();
            else StopAction();
        }

        public override void StartAction()
        {
            _animation.Play(shoot.name);
            _soundPlayer.PlaySound(shootAudio.name, AudioMixerController.MasterMixerGroup.SFX, 1f);
            ICrocodileController crocodile;
            crocodile = moveToCrocoOrNot ? _controller : anotherCroco;
            crocodile.GetCrocodileInfo().MoverHandler.GoToCroco();
        }

        public override void StopAction()
        {
            _animation.Play(cantWork.name);
        }

        #region AnimationsEvents

        protected virtual void ShowEffect() => effect.SetActive(true);
        protected virtual void StopEffect() => effect.SetActive(false);

        #endregion
    }
}
