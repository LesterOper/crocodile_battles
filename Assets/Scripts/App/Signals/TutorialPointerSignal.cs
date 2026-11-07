using App.PrototypeBattle;
using UnityEngine;

namespace App.Signals
{
    public struct TutorialPointerSignal
    {
        public Transform PointerTarget;
        public Transform PointerEndTarget;
        public bool IsMergeTutorial;
        public WeaponType WeaponType;
    }
}