using System;
using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Glue
{
    public class Glue : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            BathFastenersHandler enemyHandler = other.GetComponent<BathFastenersHandler>();
            enemyHandler.ActivateBlockFastenerBooster();
            Destroy(gameObject);
        }
    }
}