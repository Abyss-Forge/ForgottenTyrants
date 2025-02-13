using System.Collections.Generic;
using Systems.SingletonPattern;
using UnityEngine.InputSystem;

namespace Systems.GameManagers
{
    public enum EInputAction
    {
        ANY,
        MOVE, LOOK,
        JUMP, DASH,
        WEAPON_BASIC_ATTACK, WEAPON_ABILITY,
        CLASS_ABILITY_1, CLASS_ABILITY_2, CLASS_ABILITY_3, CLASS_ABILITY_4,
        PING, PAUSE, MENU,
    }

    public class MyInputManager : Singleton<MyInputManager>
    {
        public delegate void InputHandlerDelegate(InputAction.CallbackContext context);
        private Dictionary<EInputAction, (InputHandlerDelegate Delegate, bool Enabled)> _actionDelegates = new();

        public void Unsubscribe(EInputAction action, InputHandlerDelegate function) => Subscribe(action, function, false);
        public void Subscribe(EInputAction action, InputHandlerDelegate function, bool subscribe = true)
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

        public void Disable(EInputAction action) => Enable(action, false);
        public void Enable(EInputAction action, bool enable = true)
        {
            if (_actionDelegates.ContainsKey(action))
            {
                var (actionDelegate, _) = _actionDelegates[action];
                _actionDelegates[action] = (actionDelegate, enable);
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

            // Always call ANY if enabled.
            if (_actionDelegates.TryGetValue(EInputAction.ANY, out var anyActionTuple))
            {
                var (anyDelegate, anyEnabled) = anyActionTuple;

                if (anyEnabled)
                {
                    anyDelegate?.Invoke(context);
                }
            }
        }

        //esto deberia estar publico para poder asignarlo en el inspector, pero una vez asignado no se borra aunque lo pongas privado asi que XD
        public void OnMove(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.MOVE, context);

        public void OnLook(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.LOOK, context);

        public void OnJump(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.JUMP, context);

        public void OnDash(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.DASH, context);

        public void OnWeaponBasicAttack(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.WEAPON_BASIC_ATTACK, context);

        public void OnWeaponAbility(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.WEAPON_ABILITY, context);

        public void OnClassAbility1(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_1, context);

        public void OnClassAbility2(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_2, context);

        public void OnClassAbility3(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_3, context);

        public void OnClassAbility4(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.CLASS_ABILITY_4, context);

        public void OnPing(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.PING, context);

        public void OnPause(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.PAUSE, context);

        public void OnMenu(InputAction.CallbackContext context) => CallSubscribedFunction(EInputAction.MENU, context);

    }
}