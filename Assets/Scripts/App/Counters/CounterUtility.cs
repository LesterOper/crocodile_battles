using App.Chests;
using App.DAO;
using App.Signals;
using CodeBase.Infrastructure.Services;
using Library;
using Zenject;

namespace App.Counters
{
    public class CounterUtility
    {
        public CounterUtility()
        {
            
        }
        public CounterUtility(IDataService dataService, SignalBus signalBus)
        {
            _dataService = dataService;
            _signalBus = signalBus;
        }
        private const string PREFIX = "CounterUtility_";
        private const string NEXTCHEST = "NextChest";

        [Inject]private IDataService _dataService;
        [Inject] private SignalBus _signalBus;
        private float  GetExactMoney(CounterType type) => _dataService.GetLong(PREFIX + type);

        private float  GetDefaultMoney() => _dataService.GetCrocodileData().DefaultMoney;

        private long GetBattlesCountWin() => _dataService.GetCrocodileData().BattlesCountWin;
        private long GetBossBattlesCountWin() => _dataService.GetCrocodileData().BossBattlesWinCount;
        private long GetBattlesCountLoose() => _dataService.GetCrocodileData().BattlesCountLoose;

        public float GetMoney(CounterType counterType)
        {
            switch (counterType)
            {
                case CounterType.DEFAULT_MONEY : return GetDefaultMoney(); 
                case CounterType.PREMIUM_MONEY : return GetPremMoney(); 
                case CounterType.BATTLES_WIN : return GetBattlesCountWin(); 
                case CounterType.BATTLES_LOOSE : return GetBattlesCountLoose(); 
                case CounterType.POINTS : return GetExactMoney(counterType);
                case CounterType.BOSS_BATTLES_WIN : return GetBossBattlesCountWin();
                default: return 0;
            }
        }

        private void AppendDefaultMoney(float money)
        {
            _dataService.GetCrocodileData().DefaultMoney += (long)money;
            _dataService.UpdateCrocodileData();
        }

        public void AppendMoney(CounterType type, float money)
        {
            switch (type)
            {
                case CounterType.DEFAULT_MONEY : AppendDefaultMoney(money); break;
                case CounterType.PREMIUM_MONEY : AppendPremMoney(money); break;
                case CounterType.POINTS : AppendExactMoney(type, money); break;
                case CounterType.BATTLES_WIN : AppendBattlesCountWin(money); break;
                case CounterType.BATTLES_LOOSE : AppendBattlesCountLoose(money); break;
                case CounterType.BOSS_BATTLES_WIN : AppendBossBattlesCountWin(money); break;
            }
            _signalBus.Fire<ChangeMoneySignal>();
        }

        private void AppendBattlesCountWin(float money = 1)
        {
            _dataService.GetCrocodileData().BattlesCountWin+=(long)money;
            _dataService.UpdateCrocodileData();
        }

        private void AppendBossBattlesCountWin(float money = 1)
        {
            _dataService.GetCrocodileData().BossBattlesWinCount += (long) money;
            _dataService.UpdateCrocodileData();
        }
    
        private void AppendBattlesCountLoose(float money = 1)
        {
            _dataService.GetCrocodileData().BattlesCountLoose+=(long) money;
            _dataService.UpdateCrocodileData();
        }
    
        private void AppendPremMoney(float money)
        {
            _dataService.GetCrocodileData().PremMoney += (long)money;
            _dataService.UpdateCrocodileData();
        }

        private float GetPremMoney() => _dataService.GetCrocodileData().PremMoney;

        private void AppendExactMoney(CounterType type, float money)
        {
            var updated = GetMoney(type) + money;
            _dataService.SetLong(PREFIX+  type, (long)updated);
        }

        public void ClearExactMoney(CounterType type) => _dataService.SetLong(PREFIX+ type, 0);

        public ChestType GetNextChest()
        {
            int index = (int)_dataService.GetLong(NEXTCHEST);
            return (ChestType)(index <= 0 ? 1 : index);
        }

        public ChestType GetPrevChest()
        {
            int index = (int)_dataService.GetLong(NEXTCHEST) - 1;
            return (ChestType)(index);
        }

        public void SetNextChest()
        {
            int index = (int)_dataService.GetLong(NEXTCHEST);
            
            int currentIndex = index <= 0 ? 1 : index;
            currentIndex++;
            if (currentIndex > 4)
                currentIndex = 1;
            
            _dataService.SetLong(NEXTCHEST, currentIndex);
        }

        public void ResetChest()
        {
            _dataService.SetLong(NEXTCHEST, 1);
        }
    }

    public enum CounterType
    {
        NONE = 0,
        DEFAULT_MONEY = 1,
        PREMIUM_MONEY = 2,
        POINTS = 3,
        BATTLES_WIN = 4,
        BATTLES_LOOSE = 5,
        BOSS_BATTLES_WIN = 6
    }
}