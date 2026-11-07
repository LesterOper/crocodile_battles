using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Cards;
using App.SetupsScript;
using App.Tutorial;
using CodeBase.Infrastructure.Services;
using Library.Data;
using Zenject;

namespace App.DAO
{
    public interface IDataService : IService
    {
        void Initialize(DataHelper dataHelper, SignalBus signalBus);
        void ApplyData(Dictionary<Type, object[]>  data);
        Task SetCrocodileData();
        void SetTutorialStepComplete(TutorialStepType stepType);
        TutorialStepType GetTutorialCurrentStep();
        void SetTutorialCurrentStep(TutorialStepType stepType);
        void SetPlayerCrocodileCards(BattleCardInfo cardInfo, int index);
        void SetCurrentBoss(BathType bossType);
        void SetLastInterTicks();
        void SetTutorialSetOfEquipment();
        void GetData();
        Dictionary<Type, object[]> GetAllData();
        void SetExtensionUnlock(Extension extension, bool unlock);
        bool GetExtensionUnlock(Extension extension);
        BathType GetCurrentBoss();
        CrocodileData GetCrocodileData();
        BattleCardInfoData GetBattleCardInfoData();
        CardCollectionData GetCardCollectionData();
        ChestCollectionData GetChestCollectionData();
        TaskCollectionData GetTasks();
        ChestSlotsData GetChestSlotsData();
        BathData GetBathData();
        BathCollection GetBathCollection();
        bool GetTutorialStepComplete(TutorialStepType stepType);
        void SetLastSeesionTicks();
        long GetLastSessionTicks();

        long GetLastInterTicks();

        Task UpdateCrocodileData();
        void UpdateBattleCardInfoData();
        void UpdateCardCollectionData();
        void UpdateOfflineTacticsCardsData();
        void UpdateChestCollectionData();
        void UpdateTaskCollectionData();
        Task UpdateChestSlotsData();
        Task UpdateBathData();
        Task UpdateBathCollection();
        void SetLastSavedTime();
        DateTime GetLastSavedTime();
        void SetX2Reward(long ticks);
        long GetX2Reward();

        void SetX2RewardState(bool active);
        bool GetX2RewardState();
        void SetString(string key, string value);
        string GetString(string key);

        void SetLong(string key, long value);
        long GetLong(string key);
        void Clear();
        void UpdateBoosterCollectionData();
        BoosterCollectionData GetBoosterCollectionData();
         DateTime GetDateTime(string prefName);
         void SetDateTime(string prefName, DateTime value);

    }
}