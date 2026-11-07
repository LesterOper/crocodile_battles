using System;
using System.Collections.Generic;

namespace Battle
{
    public abstract class BattleRule : IBattleRule
    {
        protected List<Type> _stateQueue;
        private IEnumerator<Type> nextState;

        public Type GetNextState()
        {
            nextState ??= NextState();
            if (nextState.MoveNext())
            {
                return nextState.Current;
            }
            else
            {
                nextState = NextState();
                return GetNextState();
            }
        } 
        
        private IEnumerator<Type> NextState()
        {
            foreach (var state in _stateQueue)
            {
                yield return state;
            }            
        }

        public void RemoveState(Type state)
        {
            _stateQueue.Remove(state);
        }
    }
}