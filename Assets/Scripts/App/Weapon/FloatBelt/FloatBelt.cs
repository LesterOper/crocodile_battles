using App.Weapon.WeaponBases;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Weapon.FloatBelt
{
    public class FloatBelt : EquipmentBase
    {
        public Vector3 pos;
        public Vector3 scale;

        private void Start()
        {
            if(SceneManager.GetActiveScene().name != "Main")
                transform.SetParent(_controller.GetCrocodileInfo().BathFastenersHandler.GetParentForBelt());
            else
            {
                BathFastenersHandler handler = GetComponentInParent<BathFastenersHandler>();
                transform.SetParent(handler.GetParentForBelt());
            }
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one*0.65f;
        }
    }
}
