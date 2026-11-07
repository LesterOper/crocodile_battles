using System.Collections.Generic;
using App.Audio;
using Library;
using UnityEngine;
using Water2D;

namespace App.Weapon.WeaponBases
{
    public class WaterUsedWeaponBase : EquipmentBase
    {
        [Header("WaterWeaponSettings")]
        [SerializeField] protected List<Water2D_Spawner> waterSpawner;

        [SerializeField] private List<GameObject> shootEffect;
    
        [EasyButtons.Button]
        public override void UseWeapon(bool isAttack)
        {
            base.UseWeapon(true);
        }
        
        public override void StopAction()
        {
            base.StopAction();
            waterSpawner.ForEach(obj =>
            {
                obj.StopSpawning();
            });
            StopEffect();
        }

        public override void SetupCollision(bool isPlayer)
        {
            base.SetupCollision(isPlayer);
            waterSpawner.ForEach(obj => obj.gameObject.layer = isPlayer ? LayerMask.NameToLayer("ProjectilePlayer") : LayerMask.NameToLayer("ProjectileEnemy"));
        }


        public override void StartAction()
        {
            if(shoot != null)
            {
                _animation.Play(shoot.name);
            }
        }

        #region AnimationEvents

        public void Shoot(int index)
        {
            if (shootAudio != null)
            {
                _soundPlayer.PlaySound(shootAudio.name, AudioMixerController.MasterMixerGroup.SFX, 0.8f);
            }
            if (index < 0)
            {
                waterSpawner.ForEach(obj =>
                {
                    obj.Spawn();
                
                });
                shootEffect.ForEach(obj =>
                {
                    obj.SetActive(true);
                    
                });
            
            }
            else
            {
                waterSpawner[index].Spawn();
                shootEffect[0].SetActive(true);
            }
        }

        private void StopEffect()
        {
            shootEffect?.ForEach(obj => obj.SetActive(false));
        }

        #endregion

        public float GetOneWaterSpawnerDamage()
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            return (float)utils.GetAttackValueByLevel(level, type)/(float)waterSpawner.Count;
        }
    }
}
