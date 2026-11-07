using System;
using App.DAO;
using CodeBase.Infrastructure.Services;

namespace App.Utils
{
    public static class DailyPrizeUtils
    {
        private const string LAST_COLLECT_TIME_PREF = "PrizeUtils_last_collect_time";
        
        private static bool exist = false;

        private static void Validate()
        {
            if(exist) return;
            
            exist = true;
        }
        
        public static void OnPrizeCollected(IDataService dataService)
        {
            //Validate();
            dataService.SetString(LAST_COLLECT_TIME_PREF, DateTime.UtcNow.Ticks.ToString());
        }

        private static long GetDaysDistance(long time1, long time2)
        {
            return time1 - time2;
        }
    }
}