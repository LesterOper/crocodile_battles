using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class StateMachine<T> where T : class, IExitableState 
    {
        protected Dictionary<Type, T> states;
        protected T activeState;

        public virtual void Enter<TState>() where TState : class, T, IState 
        {
            Debug.Log($"{GetType().Name} enter state : <color=green> {typeof(TState)} </color>");
            TState state = ChangeState<TState>();
            state.Enter();
        }

        public virtual void Enter(Type type)
        {
            Debug.Log($"{GetType().Name} enter state :<color=green>  {type.Name} </color>");
            IState state = ChangeState(type);
            state.Enter();
        }

        public virtual void Enter<TState, TPayload>(TPayload payload) where TState : class, T, IPayloadedState<TPayload>
        {
            Debug.Log($"{GetType().Name} enter state : <color=green> {typeof(TState)} </color>");
            IPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(payload);
        }

        protected virtual TState ChangeState<TState>() where TState : class, T 
        {
            activeState?.Exit();
            Debug.Log($"{GetType().Name} Exit state :<color=red> {typeof(TState)} </color>");
            TState state = GetState<TState>();
            activeState = state;
      
            return state;
        }

        protected virtual IState ChangeState(Type type)
        {
            activeState?.Exit();
            Debug.Log($"{GetType().Name} enter state :<color=red>  {type.Name} </color>");
            T state = GetState(type);
            activeState = state;
            return (IState) state;
        }

        protected virtual TState GetState<TState>() where TState : class, IExitableState => 
            states[typeof(TState)] as TState;

        protected virtual T GetState(Type type) => states[type];
    }
}