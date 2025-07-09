using System;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler<T> where T : class
{
    private T owner;
    private BaseState<T> currentState;
    private BaseState<T> previousState;
    private BaseState<T> globalState;
    private Dictionary<Type, BaseState<T>> states;
    public BaseState<T> CurrentState => currentState;
    public StateHandler(T owner)
    {
        this.owner = owner;
        states = new Dictionary<Type, BaseState<T>>();
    }

    public void RegisterState(BaseState<T> state)
    {
        var stateType = state.GetType();
        if (!states.ContainsKey(stateType))
        {
            states[stateType] = state;
        }
    }

    public void ChangeState(Type stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            //Debug.LogError($"ChangeState : stateType Type Error");
            return;
        }

        previousState = currentState;
        currentState?.Exit(owner);
        currentState = states[stateType];
        currentState.Enter(owner);
    }

    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            var prevState = previousState;
            previousState = currentState;
            currentState = prevState;
            currentState.Enter(owner);
        }
    }

    public bool IsInState<TState>() where TState : BaseState<T>
    {
        return currentState?.GetType() == typeof(TState);
    }

    public void SetGlobalState(BaseState<T> state)
    {
        globalState = state;
    }

    public void Update()
    {
        globalState?.Update(owner);
        currentState?.Update(owner);
    }

    public Type GetPreviousStateType()
    {
        return previousState?.GetType() ?? typeof(WarriorIdleState);
    }
}