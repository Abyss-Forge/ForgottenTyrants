using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float _value;

    public enum EBuffApplyType
    {
        PERCENTUAL, ABSOLUTE
    }

    // Sobrescribir el operador implícito para acceder directamente al valor
    public static implicit operator float(Stat stat)
    {
        return stat._value;
    }

    public Stat(float value)
    {
        _value = value;
    }

    public void Buff(float value, EBuffApplyType type)
    {
        SetValue(value, false, type == EBuffApplyType.PERCENTUAL);
    }

    public void Debuff(float value, EBuffApplyType type)
    {
        SetValue(value, true, type == EBuffApplyType.PERCENTUAL);
    }

    private void SetValue(float value, bool isDebuff, bool isPercentual)
    {
        if (isPercentual) value *= _value / 100;

        if (isDebuff) value *= -1;

        _value += value;

        if (_value < 0) _value = 0;
    }
}