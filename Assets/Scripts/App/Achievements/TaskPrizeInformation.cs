using UnityEngine;

namespace App.Achievements
{
    [System.Serializable]
    public class TaskPrizeInformation
    {
        [SerializeField] private PrizeType prizeType;
        [SerializeField] private int prizeAmount;
        [SerializeField] private Sprite prizeIcon;
        public PrizeType GetPrizeType() => prizeType;
        public int GetPrizeAmout() => prizeAmount;
        public Sprite GetPrizeIcon() => prizeIcon;
    }
}