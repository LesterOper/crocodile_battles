using System.Collections.Generic;
using System.Linq;
using App;
using UnityEngine;

namespace Battle
{
    public class BattlePoints : Singleton<BattlePoints>
    {
        [SerializeField] private List<BattlePoint> battlePointsList;

        public Transform GetPointTransform(BattlePointType point)
        {
            return battlePointsList.FirstOrDefault(p => p.pointType == point).pointTransform;
        }
    }
}