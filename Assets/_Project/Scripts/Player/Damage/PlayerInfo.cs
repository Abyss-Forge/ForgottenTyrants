using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private Stats _baseStats = new(), _modifiedStats = new();
    public Stats Stats => _modifiedStats;

    public ClientData Data { get; private set; }

    void Awake()
    {
        Data = HostManager.Instance.GetMyClientData();
        InitializeBasicBuildData();
    }

    private void InitializeBasicBuildData()
    {
        _baseStats.Add(Data.Race.Stats);
        _baseStats.Add(Data.Class.Stats);
        _baseStats.Add(Data.Armor.Stats);
        _baseStats.Add(Data.Trinket.Stats);
    }

    public void ApplyBuff(EStat stat, float value, float duration = -1, bool isPercentual = true, bool isDebuff = false)
    {
        float baseValue = _baseStats.Get(stat);
        float bakedValue = _modifiedStats.Get(stat);

        if (isPercentual) value *= baseValue / 100;

        if (isDebuff) value *= -1;

        bakedValue += value;
        _modifiedStats.Set(stat, bakedValue);

        if (duration > 0) StartCoroutine(ResetBuff(stat, value, duration));

        Debug.Log("Buff apply ");
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