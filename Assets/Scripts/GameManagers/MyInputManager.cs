using System.Collections;
using System.Collections.Generic;
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
    private Dictionary<EInputAction, (OmniDelegate Delegate, bool Enabled)> _actionDelegates = new();

    public void Unsubscribe(EInputAction action, OmniDelegate function) => Subscribe(action, function, false);
    public void Subscribe(EInputAction action, OmniDelegate function, bool subscribe = true)
    {
        if (!_actionDelegates.ContainsKey(action))
        {
            _actionDelegates[action] = (null, true);
        }

        var (actionDelegate, enabled) = _actionDelegates[action];

        if (subscribe)
        {
            actionDelegate += function;
        }
        else
        {
            actionDelegate -= function;
        }

        _actionDelegates[action] = (actionDelegate, enabled);
    }

    public void SetActionEnabled(EInputAction action, bool enabled)
    {
        if (_actionDelegates.ContainsKey(action))
        {
            var (actionDelegate, _) = _actionDelegates[action];
            _actionDelegates[action] = (actionDelegate, enabled);
        }
    }

    private void CallSubscribedFunction(EInputAction action, InputAction.CallbackContext context)
    {
        if (_actionDelegates.TryGetValue(action, out var actionTuple))
        {
            var (actionDelegate, enabled) = actionTuple;

            if (enabled)
            {
                actionDelegate?.Invoke(context);
            }
        }
    }

    //esto deberia estar publico para poder asignarlo en el inspector, pero una vez asignado no se borra aunque lo pongas privado asi que XD
    private void OnMove(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.MOVE, context);

    private void OnLook(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.LOOK, context);

    private void OnJump(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.JUMP, context);

    private void OnDash(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.DASH, context);

    private void OnWeaponBasicAttack(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.WEAPON_BASIC_ATTACK, context);

    private void OnWeaponAbility(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.WEAPON_ABILITY, context);

    private void OnClassAbility1(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_1, context);

    private void OnClassAbility2(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_2, context);

    private void OnClassAbility3(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_3, context);

    private void OnClassAbility4(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_4, context);

    private void OnPing(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.PING, context);

    private void OnPause(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.PAUSE, context);

    private void OnMenu(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.MENU, context);

}