using UnityEngine;

namespace App.SetupsScript
{
    [CreateAssetMenu(fileName = "battle_setup", menuName = "Setup/BattleSetups")]
    public class BattleSetup : ScriptableObject
    {
        [Header("Player")]
        public int StartManaPlayer = 0;
        
        [Header("Enemy")]
        public int StartManaEnemy = 0;

        [Header("X2BonusDuration")] public long X2BonusDuration;

    }
}