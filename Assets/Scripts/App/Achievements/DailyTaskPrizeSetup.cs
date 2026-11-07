using System.Collections.Generic;
using UnityEngine;

namespace App.Achievements
{
    [CreateAssetMenu(fileName = "daily_task_prize_setup", menuName = "Setup/DailyTaskPrizeSetup")]
    public class DailyTaskPrizeSetup : ScriptableObject
    {
        [SerializeField] private List<TaskPrizeInformation> taskPrizes;
        public List<TaskPrizeInformation> GetTaskPrizes() => taskPrizes;
        
    }
}