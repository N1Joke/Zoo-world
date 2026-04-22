using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.StateMachine
{
    public abstract class StateMachineBase : IGameStateChanger
    {
        protected Dictionary<Type, IExitableState> _states;
        private IExitableState _currentState;

        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>();
            state?.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state?.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _currentState?.Exit();

            if (_states == null || !_states.TryGetValue(typeof(TState), out IExitableState state))
            {
                Debug.LogError($"[StateMachine] State of type {typeof(TState).Name} is not registered.");
                return null;
            }

            _currentState = state;
            return state as TState;
        }
    }
}
