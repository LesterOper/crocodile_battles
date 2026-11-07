using App.Achievements;
using SimpleSQL;
using UnityEngine.Scripting;

namespace App.DAO
{
    public class BoosterInfo
    {
        [PrimaryKey] [RequiredMember] public BoosterType BoosterType { get; set; }
        [RequiredMember] public int BoosterAmount { get; set; }
        [RequiredMember] public bool isChosen { get; set; }
        
    }
}