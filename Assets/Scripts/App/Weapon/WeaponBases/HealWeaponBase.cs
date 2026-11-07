using App.Crocodile.Handlers;
using UnityEngine;

namespace App.Weapon.WeaponBases
{
    public class HealWeaponBase : WaterUsedWeaponBase
    {
        public override void StartAction()
        {
            base.StartAction();
            gameObject.SetActive(true);
            Heal();
        }

        public override void Heal()
        {
            CardCharactaristicsUtils utils = new CardCharactaristicsUtils();
            CrocodileStatsHandler handler = _controller.GetCrocodileInfo().StatsHandler;
            float health = handler.CurrentHealthPointProperty;
            if (health + utils.GetHealingPoint(level, _view.Type) >
                handler.MaxHealthPointProperty)
                handler.CurrentHealthPointProperty =
                    handler.MaxHealthPointProperty;
            else
                handler.CurrentHealthPointProperty += utils.GetHealingPoint(level, _view.Type); 
        
            Destroy(gameObject, 2.5f);
        }

        public override void StopAction()
        {
            base.StopAction();
            if(gameObject != null)
                DestroyImmediate(gameObject);
        }

        public override void SetupCollision(bool isPlayer)
        {
            base.SetupCollision(isPlayer);
            waterSpawner.ForEach(obj => obj.DropObject.GetComponent<SpriteRenderer>().sortingOrder = 0);
        }

        #region AnimationEvent

        private void AnimationEventInvoke()
        {
            waterSpawner.ForEach(obj => obj.Spawn());
        }

        #endregion
    }
}