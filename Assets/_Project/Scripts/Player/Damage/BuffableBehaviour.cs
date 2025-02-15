using System;
using System.Collections;
using UnityEngine;

public class BuffableBehaviour : MonoBehaviour//, IBuffable
{
    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private Stats _baseStats = new();
    private Stats _modifiedStats = new();
    public Stats CurrentStats => _modifiedStats;

    public event Action<bool> OnBuff;

    public void Initialize(Stats defaultStats)
    {
        _baseStats = new(defaultStats);
        _modifiedStats = new(defaultStats);
    }

    public void ApplyBuffFromInfo(BuffInfo info) => ApplyBuff(info.Stat, info.Value, info.Duration, info.IsPercentual, info.IsDebuff);

    public void ApplyBuff(EStat stat, float value, float duration = -1, bool isPercentual = true, bool isDebuff = false)
    {
        float baseValue = _baseStats.Get(stat);
        float bakedValue = _modifiedStats.Get(stat);

        if (isPercentual) value *= baseValue / 100;

        if (isDebuff) value *= -1;

        bakedValue += value;
        _modifiedStats.Set(stat, bakedValue);

        if (duration > 0) StartCoroutine(ResetBuff(stat, value, duration));

        OnBuff?.Invoke(isDebuff);
        Debug.Log("Buff apply");
    }

    private IEnumerator ResetBuff(EStat stat, float value, float duration)
    {
        yield return new WaitForSeconds(duration);

        float currentValue = _modifiedStats.Get(stat);
        currentValue -= value;
        _modifiedStats.Set(stat, currentValue);

        Debug.Log("Buff reset");
        yield return null;
    }

}