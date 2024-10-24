using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        // If there is already an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = (T)this;
        OnAwake();  // base.Awake();
    }

    // Virtual method to allow implementations to use Awake: protected override void OnAwake() { }
    protected virtual void OnAwake() { }
}