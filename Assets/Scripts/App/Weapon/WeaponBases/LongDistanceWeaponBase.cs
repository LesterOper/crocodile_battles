using App.Crocodile.Fasteners;
using App.PrototypeBattle;

namespace App.Weapon.WeaponBases
{
    public class LongDistanceWeaponBase : EquipmentBase
    {
        public override void SetupCollision(bool isPlayer)
        {
            base.SetupCollision(isPlayer);
            this.isPlayer = isPlayer;
        }

        public override void Setup(WeaponView view, int level, Fastener weaponFastener)
        {
            base.Setup(view, level, weaponFastener);
        }
    }
}
