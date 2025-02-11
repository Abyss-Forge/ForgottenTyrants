using UnityEngine;

namespace Utils.Extensions
{
    public static class PlayerPrefsExtensions
    {

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            string value = PlayerPrefs.GetString(key, null);
            if (bool.TryParse(value, out bool result)) return result;
            return defaultValue;
        }

    }
}