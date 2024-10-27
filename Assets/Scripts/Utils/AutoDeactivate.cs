using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutoDeactivate : MonoBehaviour
{

    private void Awake()
    {
        OnAwake();
        gameObject.SetActive(false);
    }

    // Virtual method to allow implementations to use Awake
    protected virtual void OnAwake() { }
}
