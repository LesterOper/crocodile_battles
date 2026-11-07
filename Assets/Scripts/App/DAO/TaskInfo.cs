using System;
using App.Achievements;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class TaskInfo : IComparable<TaskInfo>
    {
        [PrimaryKey] [RequiredMember] 
        public TaskType type{ get; set; }

        [RequiredMember] 
        public TaskGroup taskGroup { get; set; }
        public TaskTimeSpan taskTimeSpan{ get; set; }
        [RequiredMember] 
        public bool isCompleted{ get; set; }
        [RequiredMember] 
        public int progress{ get; set; }
        [RequiredMember] 
        public PrizeType prizeType{ get; set; }
        [RequiredMember]
        public int prizeAmount { get; set; }
        [RequiredMember]
        public int completeValue{ get; set; }
        [RequiredMember] 
        public bool prizeTaken{ get; set; }
        [RequiredMember] 
        public long activateTime{ get; set; }

        [RequiredMember] public int currentIndex { get; set; }

        public int PrizeCoefficient { get; set; }
        public int CompleteValueCoefficient { get; set; }

        public bool Equals(TaskInfo task)
        {
            return task.isCompleted;
        }

        public int CompareTo(TaskInfo taskInfo)
        {
            if (ReferenceEquals(this, taskInfo)) return 0;
            if (ReferenceEquals(null, taskInfo)) return 1;

            var progressComparsion = progress.CompareTo(taskInfo.progress);
            if (progressComparsion != 0) return progressComparsion;
            
            return type.CompareTo(taskInfo.progress);
        }

        public void ClaimPrize()
        {
            progress = 0;
            SetNewCompleteValue();
        }

        private void SetNewCompleteValue()
        {
            completeValue += CompleteValueCoefficient;
            isCompleted = false;
            prizeAmount += PrizeCoefficient;
        }

        public void SetDailyTaskCompleted()
        {
            isCompleted = false;
            prizeTaken = true;
        }
    }

    
}