using App.AddressablesTest;
using UnityEngine;

namespace App.AddressablesManager.Addressables
{
    [RequireComponent(typeof(SetupHelper))]
    [RequireComponent(typeof(ParticleSpawner))]
    public class AddressableManager : Singleton<AddressableManager>, IAddressableManager
    {
        public SetupHelper SetupHelper { get; private set; }

        public ParticleSpawner ParticleSpawner { get; private set; }

        public bool AnyLoading()
        {
            return
                SetupHelper.AnyLoading() ||
                ParticleSpawner.AnyLoading();
        }
        private void Awake()
        {
            SetupHelper = GetComponent<SetupHelper>();
            ParticleSpawner = GetComponent<ParticleSpawner>();
        }
    }
}