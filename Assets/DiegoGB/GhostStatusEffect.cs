using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostStatusEffect : StatusEffect
{
    //TODO invulnerable effect
    public override void ApplyEffect(Player player)
    {
        Debug.Log("Ghost applied to player.");
        /*MyInputManager.Instance.DisableAction(EInputActions.WeaponBasicAttack);
        MyInputManager.Instance.DisableAction(EInputActions.WeaponAbility);
        MyInputManager.Instance.DisableAction(EInputActions.ClassAbility1);
        MyInputManager.Instance.DisableAction(EInputActions.ClassAbility2);
        MyInputManager.Instance.DisableAction(EInputActions.ClassAbility3);
        MyInputManager.Instance.DisableAction(EInputActions.ClassAbility4);*/
    }

    public override void RemoveEffect(Player player)
    {
        Debug.Log("Ghost removed from player.");
        /*MyInputManager.Instance.EnableAction(EInputActions.WeaponBasicAttack);
        MyInputManager.Instance.EnableAction(EInputActions.WeaponAbility);
        MyInputManager.Instance.EnableAction(EInputActions.ClassAbility1);
        MyInputManager.Instance.EnableAction(EInputActions.ClassAbility2);
        MyInputManager.Instance.EnableAction(EInputActions.ClassAbility3);
        MyInputManager.Instance.EnableAction(EInputActions.ClassAbility4);*/

    }
}
