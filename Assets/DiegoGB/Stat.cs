using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat<T> where T : int or T : float
{
    [SerializeField] T _value;

    private enum EBuffApplyType
    {
        PERCENTUAL, ABSOLUTE
    }

    public void Buff(T value, EBuffApplyType type)
    {
        if (type == EBuffApplyType.PERCENTUAL)
        {
            value *= (_value / 100);
        }
        SetValue(value, true)
    }

    public void Debuff(T value, EBuffApplyType type)
    {
        if (type == EBuffApplyType.PERCENTUAL)
        {
            value *= (_value / 100);
        }
        SetValue(value, false)
    }

    private void SetValue(T value, bool buff)
    {
        if (!buff)
        {
            value *= -1;
        }

        _value += value

        if (_value < 0)
        {
            _value = 0;
        }


    }
}