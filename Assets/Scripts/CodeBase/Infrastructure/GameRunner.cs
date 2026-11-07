using App.DAO;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure
{
  public class GameRunner : MonoBehaviour
  {
    public GameBootstrapper GameBootstrapper;
    private IDataService _dataService;
    private void Awake()
    {
      Application.targetFrameRate = 60;
      var bootstrapper = FindObjectOfType<GameBootstrapper>();
      if (bootstrapper == null)
        Instantiate(GameBootstrapper);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
      if (pauseStatus)
      {
        _dataService.SetLastSeesionTicks();
      }
    }
  }
}