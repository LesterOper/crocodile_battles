using App.SetupsScript;
using App.Weapon.WeaponBases;

namespace App.Crocodile.Fasteners
{
    public interface IStater
    {
        void SetActivateDrag(bool state);
        void SetMana(bool state);
        void SetTargets(WeaponEquipingHandler handler);
        void SetWeaponActionType(WeaponActionType type);
    }
}
