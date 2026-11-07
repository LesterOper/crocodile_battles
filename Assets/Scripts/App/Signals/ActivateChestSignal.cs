using App.Chests;

namespace App.Signals
{
    public struct ActivateChestSignal
    {
        public int SlotToPlace;
        public ChestType ChestType;
    }
}