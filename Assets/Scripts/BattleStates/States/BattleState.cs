using System;
using UnityEngine;

namespace BattleStates.States
{
    public abstract class BattleState : IBattleState
    {
        protected readonly BattleStateMachine battleStateMachine;
        protected readonly IBattleViewController battleViewController;

        public BattleState(BattleStateMachine battleStateMachine, IBattleViewController battleViewController)
        {
            this.battleStateMachine = battleStateMachine;
            this.battleViewController = battleViewController;
        }
        public event Action Complete;

        public abstract void Exit();

        public abstract void Enter();
        
        protected virtual void OnComplete()
        {
            Debug.Log($"{GetType().Name} - OnComplete");
            Complete?.Invoke();
        }
    }
}