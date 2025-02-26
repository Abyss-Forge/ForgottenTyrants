using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IInitializable
{
    bool IsInitialized { get; }

    void Initialize();
}

public class BuffableBehaviour : MonoBehaviour, IBuffable, IInitializable
{
    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private Stats _baseStats = new();
    private Stats _modifiedStats = new();
    public Stats CurrentStats => _modifiedStats;
    public event Action<float, EStat> OnBuff, OnDebuff;

    [SerializeField] private bool _canBeReinitialized = true;
    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;

    private CancellationTokenSource _tokenSource = new();

    public void Initialize() => Initialize(_baseStats);
    public void Initialize(Stats defaultStats)
    {
        if (_isInitialized && !_canBeReinitialized) return;
        _isInitialized = true;

        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _tokenSource = new();

        _baseStats = new Stats(defaultStats);
        _modifiedStats = new Stats(defaultStats);
    }

    public void ApplyBuffFromData(BuffData info) => ApplyBuff(info.Stat, info.Value, info.Duration, info.IsPercentual, info.IsDebuff);

    public void ApplyBuff(EStat stat, float value, float duration = -1, bool isPercentual = true, bool isDebuff = false)
    {
        float baseValue = _baseStats.Get(stat);
        float bakedValue = _modifiedStats.Get(stat);

        if (isPercentual) value *= baseValue / 100;

        if (isDebuff) value *= -1;

        bakedValue += value;
        _modifiedStats.Set(stat, bakedValue);

        if (duration > 0) Task.Run(() => ResetBuff(_tokenSource.Token, stat, value, duration), _tokenSource.Token);

        if (isDebuff) OnDebuff?.Invoke(duration, stat);
        else OnBuff?.Invoke(duration, stat);

        Debug.Log("Buff apply");
    }

    private async Task ResetBuff(CancellationToken token, EStat stat, float value, float duration)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(duration), token);

            float currentValue = _modifiedStats.Get(stat);
            currentValue -= value;
            _modifiedStats.Set(stat, currentValue);

            Debug.Log("Buff reset");
        }
        catch (TaskCanceledException) { Debug.Log("Buff reset cancelled"); }
    }

}