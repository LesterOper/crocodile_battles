using App.Crocodile;
using Battle;
using UnityEngine;

namespace BattleStates.States
{
    public interface IBattleViewController
    {
        int RoundCount { get; set; }
        RandomBattleController BattleController { get; set; }
        void IncreaseRoundCounter();
        void SetPreparingText();
        AnimationCurve Curve(AnimationType type);
        void ChangeReadyButtonState(bool active);
    }
}