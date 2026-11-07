using App.Achievements;
using App.Analytic;
using App.Chests;
using App.DAO;
using CodeBase.Infrastructure.Services;
using Zenject;

public interface IAchievementObserver : IService
{
    void Setup(IDataService dataService, SignalBus signalBus, TaskSetup taskSetup, IAnalytics analytics);
}