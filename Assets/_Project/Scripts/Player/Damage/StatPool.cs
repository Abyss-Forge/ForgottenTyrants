using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.GameManagers;
using UnityEngine;
using Utils;

//TODO implement

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

    [Serializable]
    public class Stat//<T> where T : struct, IComparable, IConvertible
    {
        /*[SerializeField] private Type _type;
        public Type Type => _type;*/

        [SerializeField] private float _defaultValue, _maximumValue, _minimumValue;

        public float Value { get; private set; }    //"Baked" value, modified at runtime by buffs and debuffs

        public Stat(float defaultValue = 0, float maximumValue = float.PositiveInfinity, float minimumValue = 0)
        {
            Preconditions.CheckState(defaultValue >= minimumValue);
            Preconditions.CheckState(defaultValue <= maximumValue);

            _defaultValue = defaultValue;
            _maximumValue = maximumValue;
            _minimumValue = minimumValue;
        }

        public void ApplyBuffFromInfo(BuffInfo info) => ApplyBuff(info.Value, info.Duration, info.IsPercentual, info.IsDebuff);

        public void ApplyBuff(float value, float duration = -1, bool isPercentual = true, bool isDebuff = false)
        {
            if (isPercentual) value *= _defaultValue / 100;

            if (isDebuff) value *= -1;

            if (Value + value < _minimumValue) value = _minimumValue - Value;
            if (Value + value > _maximumValue) value = _maximumValue - Value;

            Value += value;

            if (duration > 0) CoroutineManager.Instance.StartCoroutine(ResetBuff(value, duration));

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