using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Achievements
{
    [CreateAssetMenu(fileName = "booster_setup", menuName = "Setup/BoosterSetup")]
    public class BoosterSetup : ScriptableObject
    {
        [SerializeField] private int _winsToUnlockBoosters;
        [SerializeField] private List<BoosterInfoSetup> boosters;

        public int WinsToUnlockBoosters => _winsToUnlockBoosters;
        public BoosterInfoSetup GetBoosterInfoByType(BoosterType type) =>
            boosters.FirstOrDefault(booster => booster.BoosterType == type);
        public long GetBoosterCost(BoosterType boosterType)
        {
            return boosters.FirstOrDefault(booster => booster.BoosterType == boosterType).BoosterCost;
        }
    }

    [System.Serializable]
    public class BoosterInfoSetup
    {
        [SerializeField] private BoosterType boosterType;
        [SerializeField] private long boosterCost;
        [SerializeField] private Sprite boosterIcon;
        [SerializeField] private String boosterTitle;
        public BoosterType BoosterType => boosterType;
        public long BoosterCost => boosterCost;
        public Sprite BoosterIcon => boosterIcon;
        public String BoosterTitle => boosterTitle;
    }

    public enum BoosterType
    {
        NONE = 0,
        DOUBLE_DAMAGE = 1,
        FREEZE = 2,
        BLOCK_FASTENER = 3
    }
}