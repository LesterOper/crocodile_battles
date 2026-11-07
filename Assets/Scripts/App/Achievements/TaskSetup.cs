using System.Collections.Generic;
using System.Linq;
using App.DAO;
using Library.Collections;
using UnityEngine;

namespace App.Achievements
{
    [CreateAssetMenu(fileName = "task_setup", menuName = "Setup/TaskSetup")]
    public class TaskSetup : ScriptableObject
    {
        [SerializeField] private List<TaskInformation> tasks;
        public TaskInformation GetTaskByType(TaskType type) => tasks.FirstOrDefault(task => task.GetTaskType == type);

        public List<TaskInformation> GetEternalTasks() =>
            tasks.FindAll(task => task.GetTaskTimeSpan == TaskTimeSpan.ETERNAL);

        private int Count(float rand, List<TaskInformation> list)
        {
            float sumBuf = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sumBuf += list[i].TaskPropability;
                if (rand < sumBuf)
                    return i;
            }
            return 0;
        }
        
        public List<TaskInformation> GetDailyTasks()
        {
           var taskToChoose = tasks.FindAll(task => task.GetTaskTimeSpan == TaskTimeSpan.DAY);
           List<TaskInformation> chosenTasks = new List<TaskInformation>();

           for (int i = 0; i < 3; i++)
           {
               float sum = taskToChoose.Sum(task => task.TaskPropability);
               float randomNum = Random.Range(0, sum);
               int index = Count(randomNum, taskToChoose);
               TaskInformation task = taskToChoose[index];
               chosenTasks.Add(task);
               taskToChoose.Remove(task);
           }
           return chosenTasks;
        }
            
    }


    [System.Serializable]
    public class TaskInformation
    {
        [SerializeField] private TaskGroup _taskGroup;
        [SerializeField] private TaskType _type;
        [SerializeField] private TaskTimeSpan _timeSpan;
        [SerializeField] private List<PrizeInfo> _defaultPrize;
        [SerializeField] private int _startCompleteValue;
        [SerializeField] private string _descriptionKey;
        [SerializeField] private Sprite _taskIcon;
        [SerializeField] private float _taskPropability;
        [SerializeField] private int _completeValueCoefficient;
        [SerializeField] private int _prizeAmountCoefficient;
        public TaskGroup GetTaskGroup => _taskGroup;
        public TaskType GetTaskType => _type;
        public TaskTimeSpan GetTaskTimeSpan => _timeSpan;
        public List<PrizeInfo> GetPrizeInfo => _defaultPrize;
        public int GetStartCompleteTaskValue => _startCompleteValue;
        public string GetDescriptionKey => _descriptionKey;
        public Sprite GetTaskIcon => _taskIcon;
        public float TaskPropability => _taskPropability;
        public int CompleteValueCoefficient => _completeValueCoefficient;
        public int PrizeAmountCoefficient => _prizeAmountCoefficient;
    }

    [System.Serializable]
    public class PrizeInfo
    {
        [SerializeField] private PrizeType _prize;
        [SerializeField] private int _amount;
        [SerializeField] private Sprite _prizeIcon;
        public PrizeType GetPrizeType => _prize;
        public int GetPrizeAmount => _amount;
        public Sprite GetPrizeIcon => _prizeIcon;
    }
}