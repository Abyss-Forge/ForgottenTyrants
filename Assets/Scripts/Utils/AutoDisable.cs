using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AutoDisable : MonoBehaviour
{
    void OnValidate()
    {
        gameObject.SetActive(false);
    }
}