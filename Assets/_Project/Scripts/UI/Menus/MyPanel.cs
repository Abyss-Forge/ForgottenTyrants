using System;
using UnityEngine;

public class MyPanel : MonoBehaviour
{
    [SerializeField] private bool _beginClosed;

    public event Action OnOpen, OnClose;

    void Awake()
    {
        if (_beginClosed) gameObject.SetActive(false);
    }

    public void Close()
    {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        OnOpen?.Invoke();
        gameObject.SetActive(true);
    }

}