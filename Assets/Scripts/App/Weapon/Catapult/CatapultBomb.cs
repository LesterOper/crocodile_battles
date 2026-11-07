using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Catapult
{
    public class CatapultBomb : Projectile
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name == "umbrella(Clone)")
            {
                rb.AddForce(new Vector2(10,25)*50, ForceMode2D.Force);
            }
            else base.OnTriggerEnter2D(other);
        }

        protected override void Effect(Vector2 col, GameObject effect, bool isWater = false) => base.Effect(col, effect);
    }
}
