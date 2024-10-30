using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EInputActions
{
    Move,
    Look,
    Jump,
    Dash,
    WeaponBasicAttack,
    WeaponAbility,
    ClassAbility1,
    ClassAbility2,
    ClassAbility3,
    ClassAbility4,
    Ping,
    Pause,
    Menu,
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

    public void OnWeaponBasicAttack(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.WeaponBasicAttack, context);
    }

    public void OnWeaponAbility(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.WeaponAbility, context);
    }

    public void OnClassAbility1(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.ClassAbility1, context);
    }

    public void OnClassAbility2(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.ClassAbility2, context);
    }

    public void OnClassAbility3(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.ClassAbility3, context);
    }

    public void OnClassAbility4(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.ClassAbility4, context);
    }

    public void OnPing(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Ping, context);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Pause, context);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputActions.Menu, context);
    }

}
