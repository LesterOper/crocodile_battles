using System.Collections.Generic;
using App.Crocodile.Fasteners;
using App.PrototypeBattle;
using Library.Utils;
using UnityEngine;
using Water2D;

namespace App.Weapon.WeaponBases
{
    public class OneActionEquipmentBase : EquipmentBase
    {
        [SerializeField] protected List<Water2D_Spawner> waterSpawner;


        public override void PreStartAction()
        {
            if(preStartAnimation != null)
            {
                _animation.Play(preStartAnimation.name);
            }
        }

        public override void StartAction()
        {
            if(shoot != null)
            {
                _animation.Play(shoot.name);
            }
            if (_view != null)
            {
                if (_view.Type.GetAttribute<WeaponData>().IsNeedFastener)
                {
                    List<Fastener> fasteners = _controller.GetCrocodileInfo().BathFastenersHandler.GetFasteners();
                    IFastener fastener = fasteners.Find(obj => obj.IsFreeFastener());
                    if (fastener == null)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        gameObject.SetActive(true);
                        transform.position = fastener.GetFastenerTransform().position;
                    }
                }
            }
        }

        public override void SetupCollision(bool isPlayer)
        {
            base.SetupCollision(isPlayer);
            waterSpawner?.ForEach(obj => obj.gameObject.layer = isPlayer ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy"));
        }
    
        public override void Setup(WeaponView view, int level, Fastener weaponFastener)
        {
            base.Setup(view, level, weaponFastener);
        }

        public override void StopAction()
        {
            if(gameObject != null)
                DestroyImmediate(gameObject);
        }
    }
}
