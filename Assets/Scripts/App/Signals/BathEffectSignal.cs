namespace App.Signals
{
    public struct BathEffectSignal
    {
        public bool IsPlayer;
        public EffectType Effect;
    }

    public enum EffectType
    {
        NONE= 0,
        BOMB = 1,
        HEAL = 2,
    }
}