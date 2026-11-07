using App.Achievements;

namespace App.Signals
{
    public struct AttackChangedSignal
    {
        public bool IsPlayer;
        public long AttackPoints;
    }

    public struct LoadBattleSceneSignal
    {
        
    }

    public struct BoosterAmountChanged
    {
        public BoosterType BoosterType;
    }
}