using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CursorUtils
{
    public static bool IsCaptured => GetCaptured();

    private static bool GetCaptured() => Cursor.lockState == CursorLockMode.Locked;

    public static void Release() => Capture(false);
    public static void Capture(bool locked = true)
    {
        if (locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !locked;
    }

}