using System.Threading.Tasks;
using AddressablesManager.Addressables;
using App.AddressablesManager.Addressables;
using App.AddressablesTest;
using UnityEngine;
using UnityEngine.UI;

public class ParticleSpawnerButton : MonoBehaviour
{
    [SerializeField] private ParticleType _particleType;
    [SerializeField] private int _spawnCount = 1;

    public async Task RequestSpawnParticle(ParticleType particleType)
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            var pos = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5,5), 0);
            var prop = new Properties(pos, Quaternion.identity);
            var particle = await AddressableManager.Instance.ParticleSpawner.GetParticle(particleType, prop);
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(async () => await RequestSpawnParticle(_particleType));
    }

    private void OnValidate()
    {
    }

}
