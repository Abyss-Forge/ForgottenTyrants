using UnityEngine;

namespace Utils
{
    public static class CursorHelper
    {
        static bool _isCaptured;
        public static bool IsCaptured => _isCaptured;

        public static void Toggle() => Capture(!IsCaptured);
        public static void Release() => Capture(false);
        public static void Capture(bool locked = true)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;

            _isCaptured = locked;
        }

    }
}