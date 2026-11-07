namespace App.Signals
{
    public struct HealthPointsChangeSignal
    {
        public bool IsPlayer;
        public float CurrentHealthPoint;
        public float HPChangedPercent;
    }
}