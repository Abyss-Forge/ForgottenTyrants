using System;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class RangoAttribute : PropertyAttribute
{
    public int Min { get; }
    public int Max { get; }

    public RangoAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }

}