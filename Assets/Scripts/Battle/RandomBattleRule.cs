using System;
using System.Collections.Generic;
using BattleStates.States;

namespace Battle
{
    public class RandomBattleRule : BattleRule
    {
        private bool _isDoubleCard;
        public RandomBattleRule(bool isDoubleCard = false)
        {
            _isDoubleCard = isDoubleCard;
            _stateQueue = new List<Type>
            {
                typeof(PreBattleState),
                typeof(PlayerPreparingState),
                typeof(FightingState),
            };
        }
        
        
    }
}