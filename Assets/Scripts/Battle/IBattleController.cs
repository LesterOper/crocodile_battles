using System.Threading.Tasks;
using App.Counters;
using App.DAO;
using BattleStates;
using CodeBase.Infrastructure.Factory;
using Zenject;

namespace Battle
{
    public interface IBattleController
    {
        void Init(IGameFactory gameFactory, SignalBus signalBus, CounterUtility counterUtility,
            IDataService dataService, DiContainer diContainer);
        void InitializeCards();
        Task SetupBattle(BattleStateInfoHandler infoHandler, SignalBus signalBus);
        IBattleRule BattleRule { get; }
    }
}