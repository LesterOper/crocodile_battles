using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
  public class AssetProvider : IAssetProvider
  {
    public GameObject Instantiate(string path)
    {
      GameObject heroPrefab = Resources.Load<GameObject>(path);
      return Object.Instantiate(heroPrefab);
    }

    public GameObject Instantiate(string path, Vector3 spawnPoint)
    {
      GameObject heroPrefab = Resources.Load<GameObject>(path);
      return Object.Instantiate(heroPrefab, spawnPoint, Quaternion.identity);
    }
  }
}