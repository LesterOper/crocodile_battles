using System.Collections.Generic;
using App.DAO;
using App.SetupsScript;
using App.Tutorial;
using CodeBase.Infrastructure.Services;

namespace App.Utils
{
    public static class FightUtils
    {
        private static bool exist = false;

        private static void Validate()
        {
            if(exist) return;
            exist = true;
        }
        
        public static Dictionary<BathType, int> FIGHTS_NEED_TO_OPEN = new Dictionary<BathType, int>()
        {
            [BathType.BOSS_1] = 3,
            [BathType.BOSS_2] = 5,
            [BathType.BOSS_3] = 10,
            [BathType.BOSS_4] = 20,
            [BathType.BOSS_5] = 40,
            [BathType.BOSS_6] = 100,
        };
        private const string BATTLES_COUNT = "BATTLES_COUNT";

        public static int GetNeedFightsToOpen(IDataService dataService)
        {
            BathType bath = dataService.GetCurrentBoss();
            if (bath == BathType.NONE)
            {
                dataService.SetCurrentBoss(BathType.BOSS_1);
                bath = BathType.BOSS_1;
            }
            return FIGHTS_NEED_TO_OPEN[bath];
        }
    
        public static void OnCompleteBattle(IDataService dataService)
        {
            if (dataService.GetTutorialCurrentStep() !=
                TutorialStepType.BATTLE_TUTORIAL_END)
            {
                dataService.SetLong(BATTLES_COUNT, dataService.GetLong(BATTLES_COUNT) + 1);
            }
        }
    
        public static void OnCompleteBossBattle(IDataService dataService)
        {
            
            dataService.SetLong(BATTLES_COUNT,0);
        }

        public static bool BossBattleIsOpen(IDataService dataService)
        {
            BathType bath = dataService.GetCurrentBoss();
            if (bath == BathType.NONE)
            {
                dataService.SetCurrentBoss(BathType.BOSS_1);
                bath = BathType.BOSS_1;
            }
            return dataService.GetLong(BATTLES_COUNT) >=
                   FIGHTS_NEED_TO_OPEN[bath];
        }
    
        public static int BattlesCount(IDataService dataService)
        {
            return (int)dataService.GetLong(BATTLES_COUNT);
        }
    
        public static float ProgressToOpen(IDataService dataService)
        {
            int count = (int)dataService.GetLong(BATTLES_COUNT);
            if (count != 0)
            {
                return (float) count / FIGHTS_NEED_TO_OPEN[dataService.GetCurrentBoss()];
            }
            else
            {
                return 0;
            }
        
        }

    }
}

