using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.GameManagers;
using UnityEngine;

public class StatPool : MonoBehaviour
{
    [SerializeField] Stat _health;
    [SerializeField] Stat _physicalDamage;
    [SerializeField] Stat _magicalDamage;
    [SerializeField] Stat _physicalDefense;
    [SerializeField] Stat _magicalDefense;
    [SerializeField] Stat _movementSpeed;
    [SerializeField] Stat _attackSpeed;
    [SerializeField] Stat _cooldownReduction;

    void nidf()
    {
        _health.Buff(50, duration: 3f);
    }

    [Serializable]
    public class Stat//<T> where T : struct, IComparable, IConvertible
    {
        /*[SerializeField] private Type _type;
        public Type Type => _type;*/

        [SerializeField] private float _defaultValue, _maximumValue, _minimumValue;

        public float Value { get; private set; }    //"Baked" value, modified by buffs, and debuffs

        public Stat(float defaultValue = 0, float maximumValue = float.PositiveInfinity, float minimumValue = 0)
        {
            Preconditions.CheckState(defaultValue >= minimumValue);
            Preconditions.CheckState(defaultValue <= maximumValue);

            _defaultValue = defaultValue;
            _maximumValue = maximumValue;
            _minimumValue = minimumValue;
        }

        public void Buff(float value, bool isPercentual = true, float duration = -1)
        {
            SetValue(value, false, isPercentual, duration);
        }

        public void Debuff(float value, bool isPercentual = true, float duration = -1)
        {
            SetValue(value, true, isPercentual, duration);
        }

        private void SetValue(float value, bool isDebuff, bool isPercentual, float duration)//15, false, false, -1
        {
            if (isPercentual) value *= _defaultValue / 100;

            if (isDebuff) value *= -1;

            if (Value + value < _minimumValue) value = _minimumValue - Value;
            if (Value + value > _maximumValue) value = _maximumValue - Value;

            Value += value;

            if (duration > 0) CoroutineManager.Instance.StartCoroutine(ResetBuff(value, duration));   //corrutina

            Debug.Log("Buff apply ");
        }

        private IEnumerator ResetBuff(float value, float duration)
        {
            yield return new WaitForSeconds(duration);

            Value += -value;
            Debug.Log("Buff reset");

            yield return null;
        }

    }
}