using System.Collections.Generic;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class TaskCollectionData
    {
        public TaskCollectionData()
        {
        }

        [RequiredMember] public List<TaskInfo> TaskCollection { get; set; }
    }
}