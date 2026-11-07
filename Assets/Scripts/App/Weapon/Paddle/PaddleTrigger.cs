using App.Weapon.WeaponBases;
using UnityEngine;

namespace App.Weapon.Paddle
{
    public class PaddleTrigger : MonoBehaviour
    {
        [SerializeField] private CloseDistanceWeaponBase _base;
        private bool triggered;
        private int index = 0;
        private void OnTriggerEnter2D(Collider2D other)
        {
            index++;
            _base.StartAction();
        }
    }
}
