using UnityEngine;
using Random = UnityEngine.Random;

namespace Effects
{
    public class WeatherSetup : MonoBehaviour
    {
        [SerializeField] private RainController rain;
        [SerializeField] private SnowController snow;

        private void Awake()
        {
            if (Random.Range(0, 10) > 5)
            {
                rain.gameObject.SetActive(true);
                rain.OnMasterChanged(1);
                rain.OnLightningChanged(1);
                rain.OnRainChanged(Random.Range(0f,1f));
                rain.OnWindChanged(Random.Range(0f,1f));
                rain.OnFogChanged(Random.Range(0f,1f));
            }
            /*else
        {
            snow.gameObject.SetActive(true);
            snow.OnMasterChanged(1);
            snow.OnFogChanged(Random.Range(0f,0.5f));
            snow.OnSnowChanged(Random.Range(0f,1f));
            snow.OnSnowLevelChanged(Random.Range(0f,7f));
            snow.OnWindChanged(Random.Range(0f,1f));
        }
        */
        }
    }
}
