using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Achievements
{
    [CreateAssetMenu(fileName = "merge_cost_setup", menuName = "Setup/MergeCostSetup")]
    public class MergeCostSetup : ScriptableObject
    {
        [SerializeField] private List<MergeCost> mergeCostValues;

        public long GetMergeCostByLevel(int level)
        {
            return mergeCostValues.FirstOrDefault(cost => cost.GetCardLevel == level).GetCostValue;
        }
    }
    [System.Serializable]
    public class MergeCost
    {
        [SerializeField] private int _cardLevel;
        [SerializeField] private long _costValue;
        public int GetCardLevel => _cardLevel;
        public long GetCostValue => _costValue;
    }
}