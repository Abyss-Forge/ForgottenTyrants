using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CursorUtils
{
    public bool IsCaptured => GetCaptured();

    private bool GetCaptured() => Cursor.lockState == CursorLockMode.Locked;

    public void Release() => Capture(false);
    public void Capture(bool locked = true)
    {
        if (locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !locked;
    }

}