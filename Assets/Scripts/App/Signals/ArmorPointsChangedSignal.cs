namespace App.Signals
{
    public struct ArmorPointsChangedSignal
    {
        public bool IsPlayer;
        public float CurrentArmorPoint;
        public float ArmorChangedPercent;
    }
}