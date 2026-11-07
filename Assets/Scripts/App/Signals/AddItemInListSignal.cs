using App.Achievements;

namespace App.Signals
{
    public struct AddItemInListSignal
    {
        public bool IsAdd;
    }

    public struct ActivateBoosterSignal
    {
        public BoosterType BoosterType;
        public bool IsUsed;
    }
}