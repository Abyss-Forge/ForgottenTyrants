using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EInputActions
{
    Move,
    Look,
    Jump,
    Dash,
}

public class MyInputManager : Singleton<MyInputManager>
{

    public delegate void OmniDelegate(InputAction.CallbackContext context);

    private Dictionary<EInputActions, OmniDelegate> _actionDelegates = new Dictionary<EInputActions, OmniDelegate>();

    public void SubscribeToInput(EInputActions action, OmniDelegate function, bool subscribe = true)
    {
        if (!_actionDelegates.ContainsKey(action))
        {
            _actionDelegates[action] = null;
        }

        if (subscribe)
        {
            _actionDelegates[action] += function;
        }
        else
        {
            _actionDelegates[action] -= function;
        }
    }

    private void CallSubscribedFunction(EInputActions action, InputAction.CallbackContext context)
    {
        if (_actionDelegates.TryGetValue(action, out OmniDelegate actionDelegate))
        {
            actionDelegate?.Invoke(context);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Move, context);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Look, context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Jump, context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Dash, context);
    }
}
