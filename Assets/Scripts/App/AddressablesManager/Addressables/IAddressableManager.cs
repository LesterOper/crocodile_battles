using App.AddressablesTest;

namespace App.AddressablesManager.Addressables
{
    public interface IAddressableManager
    {
        SetupHelper SetupHelper { get; }

        ParticleSpawner ParticleSpawner { get; }
        bool AnyLoading();
    }
}