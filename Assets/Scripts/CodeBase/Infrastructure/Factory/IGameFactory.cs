using System.Threading.Tasks;
using App.Crocodile.Implementations;
using Battle;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
  public interface IGameFactory : IService
  {
    void Cleanup();

    Task<CrocodileController> CreateCrocodile(IBattleController battleController, Vector3 at, string name,
      bool isPlayer);
  }
}