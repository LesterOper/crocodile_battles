
using UnityEngine;

namespace Effects
{
    public class DropsActivator : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect = null;

        private void OnEnable()
        {
            if(effect != null)
                effect.Play();
        }
    }
}
