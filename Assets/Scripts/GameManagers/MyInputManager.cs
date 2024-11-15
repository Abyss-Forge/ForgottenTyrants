using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EInputAction
{
    MOVE,
    LOOK,
    JUMP,
    DASH,
    WEAPON_BASIC_ATTACK,
    WEAPON_ABILITY,
    CLASS_ABILITY_1,
    CLASS_ABILITY_2,
    CLASS_ABILITY_3,
    CLASS_ABILITY_4,
    PING,
    PAUSE,
    MENU,
}

public class MyInputManager : Singleton<MyInputManager>
{
    public delegate void OmniDelegate(InputAction.CallbackContext context);

    private Dictionary<EInputAction, OmniDelegate> _actionDelegates = new Dictionary<EInputAction, OmniDelegate>();
    //private Dictionary<EInputActions, (OmniDelegate Delegate, bool Enabled)> _actionDelegates = new();

    public void SubscribeToInput(EInputAction action, OmniDelegate function, bool subscribe = true)
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

    private void CallSubscribedFunction(EInputAction action, InputAction.CallbackContext context)
    {
        if (_actionDelegates.TryGetValue(action, out OmniDelegate actionDelegate))
        {
            actionDelegate?.Invoke(context);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.MOVE, context);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.LOOK, context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.JUMP, context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.DASH, context);
    }

    public void OnWeaponBasicAttack(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.WEAPON_BASIC_ATTACK, context);
    }

    public void OnWeaponAbility(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.WEAPON_ABILITY, context);
    }

    public void OnClassAbility1(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.CLASS_ABILITY_1, context);
    }

    public void OnClassAbility2(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.CLASS_ABILITY_2, context);
    }

    public void OnClassAbility3(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.CLASS_ABILITY_3, context);
    }

    public void OnClassAbility4(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.CLASS_ABILITY_4, context);
    }

    public void OnPing(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.PING, context);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.PAUSE, context);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        CallSubscribedFunction(EInputAction.MENU, context);
    }

}