using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressablesManager.Addressables;
using App.AddressablesTest;
using UnityEngine;

namespace App.AddressablesManager.Addressables
{
    public class ParticleSpawner : PrefabsHelper
    {
        [SerializeField] private List<ParticleAddressable> _particles;

        public async Task<GameObject> GetParticle(ParticleType particleType, Properties prop)
        {
            var particle = _particles.First(p => p.Type == particleType);
            return await SpawnPrefab(particle.AssetReference, prop);
        }
    }
}