using UnityEngine;

namespace CodeBase.Data
{
  [SerializeField]
  public class PlayerProgress
  {
    public WorldData WorldData;
    public PlayerProgress(string initialLevel)
    {
      WorldData = new WorldData(initialLevel);
    }
  }
}