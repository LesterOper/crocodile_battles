using UnityEngine;

namespace App.Weapon.Bobber
{
    public class BobberEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ripple;
        private bool isOnTriggerStay;

        private void Update()
        {
            ripple.transform.position = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("WaterSurface"))
                ripple.Play();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isOnTriggerStay) return;
            var main = ripple.main;
            main.loop = true;
            ripple.Play();
            isOnTriggerStay = true;
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isOnTriggerStay) return;
            var main = ripple.main;
            main.loop = false;
            ripple.Play();
            isOnTriggerStay = false;
        }
    }
}