using UnityEngine;

namespace Utils
{
    public static class CursorHelper
    {
        public static bool IsCaptured => GetCaptured();

        private static bool GetCaptured()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public static void Toggle() => Capture(!IsCaptured);
        public static void Release() => Capture(false);
        public static void Capture(bool locked = true)
        {
            if (locked) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;

            Cursor.visible = !locked;
        }

    }
}