using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float _value;
    public float Value => _value;

    public Stat()
    {
        _value = 0;
    }

    public Stat(float value)
    {
        _value = value;
    }

    public float Buff(float value, bool isPercentual = true, float duration = -1)
    {
        return SetValue(value, false, isPercentual, duration);
    }

    public float Debuff(float value, bool isPercentual = true, float duration = -1)
    {
        return SetValue(value, true, isPercentual, duration);
    }

    private float SetValue(float value, bool isDebuff, bool isPercentual, float duration)
    {
        if (isPercentual) value *= _value / 100;

        if (isDebuff) value *= -1;

        _value += value;

        if (_value < 0) _value = 0;

        if (duration > 0) ResetBuff(value, duration);//corrutina

        Debug.Log("Buff apply ");
        return value;
    }

    private IEnumerator ResetBuff(float value, float duration)
    {
        yield return new WaitForSeconds(duration);

        _value += -value;
        Debug.Log("Buff reset");

        yield return null;
    }

}