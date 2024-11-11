using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCursorManager : Singleton<MyCursorManager>
{
    [SerializeField] private GameObject _crosshairParent;
    public bool IsCaptured => GetCaptured();

    void Start()
    {
        Release();
        Capture();
    }

    private bool GetCaptured()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void Capture()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Release()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableCrosshair()
    {
        _crosshairParent.SetActive(true);
    }

    public void DisableCrosshair()
    {
        _crosshairParent.SetActive(false);
    }

}
