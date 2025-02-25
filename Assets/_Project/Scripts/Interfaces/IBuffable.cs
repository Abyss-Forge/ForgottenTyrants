using System;

public interface IBuffable
{
    // Property to access the stats of the object.
    public Stats CurrentStats { get; }

    // Method to apply buff to the object.
    void ApplyBuff(EStat stat, float value, float duration = -1, bool isPercentual = true, bool isDebuff = false);

    // Optional: Method to reset the object buff.
    //Task ResetBuff(CancellationToken token, EStat stat, float value, float duration);

    // Optional: Event to notify whenever the object gets buffed (provides buff duration in seconds + buffed stat)
    event Action<float, EStat> OnBuff, OnDebuff;
}