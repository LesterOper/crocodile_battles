using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using App.Achievements;
using App.Cards;
using App.PrototypeBattle;
using App.SetupsScript;
using App.Signals;
using App.Tutorial;
using Library.Data;
using Zenject;

namespace App.DAO
{
    public class SqlDataService : IDataService
    {
        private const string WON_BOSS = "WON_BOSS";
        private const string INTER = "INTER_LAST_SHOW_TICKS";
        private const string SAVE = "SAVE";
        private const string X2_REWARD_TIME = "X2_REWARD_TIME";

        private CrocoDataHelper _dataHelper;
        private BathData BathData = new BathData();
        private BattleCardInfoData BattleCardInfoData = new BattleCardInfoData();
        private ChestCollectionData ChestCollectionData = new ChestCollectionData();
        private ChestSlotsData ChestSlotsData = new ChestSlotsData();
        private CardCollectionData CardCollectionData = new CardCollectionData();
        private CrocodileData CrocodileData = new CrocodileData();
        private BathCollection _bathCollection = new BathCollection();
        private TaskCollectionData _taskCollectionData = new TaskCollectionData();
        private BoosterCollectionData _boosterCollectionData = new BoosterCollectionData();
        private SignalBus _signalBus;

        public void Initialize(DataHelper dataHelper, SignalBus signalBus)
        {
            _dataHelper = (CrocoDataHelper) dataHelper;
            _signalBus = signalBus;
            GetData();
        }

        public void SetTutorialSetOfEquipment()
        {
            if (!GetTutorialStepComplete(TutorialStepType.BATTLE_TUTORIAL_END))
            {
                BattleCardInfoData.CardInfos.Clear();
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.BOTTLE_GUN, type = WeaponActionType.ATTACK_LONG_DISTANCE, Level = 7});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.BRUSH, type = WeaponActionType.ATTACK_CLOSE_DISTANCE, Level = 9});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.MINA_BOMB, type = WeaponActionType.ONE_ACTION_BOMB, Level = 6});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.TEST_TUBE_GUN, type = WeaponActionType.ATTACK_LONG_DISTANCE, Level = 10});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.BOMB_ILLNESS, type = WeaponActionType.ATTACK_CLOSE_DISTANCE, Level = 4});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.UMBRELLA, type = WeaponActionType.DEFENCE, Level = 1});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.LIFEBUOY, type = WeaponActionType.DEFENCE, Level = 10});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.RIFLE, type = WeaponActionType.ATTACK_LONG_DISTANCE, Level = 9});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.SHOWER, type = WeaponActionType.ATTACK_CLOSE_DISTANCE, Level = 10});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.BOW, type = WeaponActionType.ATTACK_LONG_DISTANCE, Level = 8});
                BattleCardInfoData.CardInfos.Add(new BattleCardInfo()
                    {WeaponType = WeaponType.PADDLE, type = WeaponActionType.ATTACK_CLOSE_DISTANCE, Level = 6});
                BathData.CrocodileBathType = BathType.BATH_TYPE_4;
                BathData.Level = 7;
                SetTutorialCurrentStep(TutorialStepType.TUTORIAL_BATTLE);
            }
        }
        
        public Task SetCrocodileData()
        {
            UpdateBathData();
            UpdateCrocodileData();
            UpdateCardCollectionData();
            UpdateBattleCardInfoData();
            UpdateChestCollectionData();
            UpdateChestSlotsData();
            UpdateOfflineTacticsCardsData();
            return Task.CompletedTask;
        }

        public BathType GetCurrentBoss()
        {
            if ((BathType) _dataHelper.GetLong(WON_BOSS) == BathType.NONE)
                return BathType.BOSS_1;
            return (BathType)_dataHelper.GetLong(WON_BOSS);
        }
        public CrocodileData GetCrocodileData() => CrocodileData;

        public BattleCardInfoData GetBattleCardInfoData() => BattleCardInfoData;

        public CardCollectionData GetCardCollectionData() => CardCollectionData;
        
        public ChestCollectionData GetChestCollectionData() => ChestCollectionData;
        public BoosterCollectionData GetBoosterCollectionData() => _boosterCollectionData;
        public TaskCollectionData GetTasks() => _taskCollectionData;
        public ChestSlotsData GetChestSlotsData() => ChestSlotsData;
        public BathData GetBathData() => BathData;
        public BathCollection GetBathCollection() => _bathCollection;

        public long GetLastInterTicks() => _dataHelper.GetLong(INTER);

        public Task UpdateCrocodileData()
        {
            CrocodileData ??= new CrocodileData();
            _dataHelper.SetCrocodileData(CrocodileData);
            return Task.CompletedTask;
        }
        
        public DateTime GetDateTime(string prefName)
        {
            var stringTime = GetString(prefName);
            if(string.IsNullOrEmpty(stringTime)) return DateTime.MinValue;
            return DateTime.ParseExact(stringTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        public void SetDateTime(string prefName, DateTime value)
        {
            SetString(prefName, value.ToString(CultureInfo.InvariantCulture));
        }

        
        public void UpdateBattleCardInfoData()
        {
            UpdateCrocodileData();
            _dataHelper.SetBattleCardData(BattleCardInfoData.CardInfos);
        }

        public void UpdateCardCollectionData()
        {
            UpdateCrocodileData();
            _dataHelper.SetCardCollectionData(CardCollectionData.Collection);
        }

        public void UpdateOfflineTacticsCardsData()
        {
            UpdateCrocodileData();
        }

        public void UpdateChestCollectionData()
        {
            UpdateCrocodileData();
            _dataHelper.SetChestCollectionData(ChestCollectionData.ChestCollection);
        }

        public void UpdateBoosterCollectionData()
        {
            UpdateCrocodileData();
            _dataHelper.SetBoostersCollectionData(_boosterCollectionData.BoosterCollection);
        }

        public void UpdateTaskCollectionData()
        {
            UpdateCrocodileData();
            _dataHelper.SetTasksCollectionData(_taskCollectionData.TaskCollection);
        }

        public Task UpdateChestSlotsData()
        {
            UpdateCrocodileData();
            _dataHelper.SetChestSlotsData(ChestSlotsData.Slots);
            return Task.CompletedTask;
        }

        public Task UpdateBathData()
        {
            UpdateCrocodileData();
            _dataHelper.SetBathData(BathData);
            return Task.CompletedTask;
        }

        public Task UpdateBathCollection()
        {
            UpdateCrocodileData();
            _dataHelper.SetBathCollectionData(_bathCollection.BathDatas);
            return Task.CompletedTask;
        }
        
        public void SetLastSavedTime() => _dataHelper.SetLong(SAVE, DateTime.UtcNow.ToLocalTime().Ticks);

        public DateTime GetLastSavedTime() => new DateTime(_dataHelper.GetLong(SAVE));
        public void SetX2Reward(long ticks) => _dataHelper.SetLong(X2_REWARD_TIME, ticks);

        public long GetX2Reward() => _dataHelper.GetLong(X2_REWARD_TIME);
        public void SetX2RewardState(bool active) => _dataHelper.SetLong(X2_REWARD_TIME + "_STATE", active ? 1 : 0);

        public bool GetX2RewardState() => _dataHelper.GetLong(X2_REWARD_TIME + "_STATE") == 1;
        
        public void SetString(string key, string value) => _dataHelper.SetString(key, value);

        public string GetString(string key) => _dataHelper.GetString(key);
        public void SetLong(string key, long value) => _dataHelper.SetLong(key, value);

        public long GetLong(string key) => _dataHelper.GetLong(key);

        public void SetTutorialStepComplete(TutorialStepType stepType) => _dataHelper.SetLong(stepType.ToString(), 1);

        public bool GetTutorialStepComplete(TutorialStepType stepType) => _dataHelper.GetLong(stepType.ToString()) == 1;
        public TutorialStepType GetTutorialCurrentStep() => (TutorialStepType) _dataHelper.GetLong("Current_");
        public void SetTutorialCurrentStep(TutorialStepType stepType) => _dataHelper.SetLong("Current_", (int)stepType);

        public void SetPlayerCrocodileCards(BattleCardInfo cardInfo, int index)
        {
            BattleCardInfoData.CardInfos.RemoveAt(index);
            BattleCardInfoData.CardInfos.Insert(index, cardInfo);
        }

        public void SetCurrentBoss(BathType bossType)
        {
            _dataHelper.SetLong(WON_BOSS, (int)bossType);
        }

        public void SetLastInterTicks()
        {
            _dataHelper.SetLong(INTER, DateTime.UtcNow.Ticks);
        }

        public void SetLastSeesionTicks() => _dataHelper.SetLastSessionTicks();

        public long GetLastSessionTicks() => _dataHelper.GetLastSessionTicks();

        public void SetExtensionUnlock(Extension extension, bool unlock)
        {
            _dataHelper.SetLong(extension.ToString(), unlock ? 1 : 0);
            _signalBus.Fire<TutorialButtonSignal>();
        }

        public void ApplyData(Dictionary<Type, object[]>  data)
        {
            _dataHelper.ApplyData(data);
            GetData();
        }

        public Dictionary<Type, object[]> GetAllData()
        {
           return _dataHelper.GetData();
        }

        public bool GetExtensionUnlock(Extension extension) => _dataHelper.GetLong(extension.ToString()) == 1;
        
        public void GetData()
        {
            ChestCollectionData.ChestCollection = _dataHelper.GetChestCollectionData();
            _taskCollectionData.TaskCollection = _dataHelper.GetTaskCollectionData();
            _boosterCollectionData.BoosterCollection = _dataHelper.GetBoosterCollectionData();
            if (_boosterCollectionData.BoosterCollection.Count == 0)
            {
                InitBoosters();
            }
            ChestSlotsData = _dataHelper.GetChestSlotsData();
            CrocodileData = _dataHelper.GetCrocodileData();
            BathData = _dataHelper.GetBathData();
            CardCollectionData = _dataHelper.GetCardCollectionData();
            BattleCardInfoData = _dataHelper.GetBattleCardData();
            _bathCollection.BathDatas = _dataHelper.GetBathCollection();
        }

        private void InitBoosters()
        {
            foreach (BoosterType booster in Enum.GetValues(typeof(BoosterType)))
            {
                if(booster == BoosterType.NONE)
                    continue;
                _boosterCollectionData.BoosterCollection.Add(new BoosterInfo(){BoosterType = booster, BoosterAmount = 0, isChosen = false});
            }
            UpdateBoosterCollectionData();
        }

        public void Clear()
        {
            _dataHelper.Clear();
        }
    }
}