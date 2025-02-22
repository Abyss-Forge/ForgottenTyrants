using UnityEngine;
using UnityEngine.Audio;

namespace Systems.AudioManagement
{
    public class AudioVolumeHelper
    {
        private AudioMixer _audioMixer;

        public AudioVolumeHelper(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
        }

        public void LoadPrefs()
        {
            foreach (string key in VolumeKeyList)
            {
                float value = PlayerPrefs.GetFloat(key, 0.5f);
                SetVolume(key, value);
            }
        }

        public string[] VolumeKeyList
        {
            get
            {
                string[] keyList = new string[]
                {
                PlayerPrefsKeys.VOLUME_MASTER,
                PlayerPrefsKeys.VOLUME_UI,
                PlayerPrefsKeys.VOLUME_MUSIC,
                PlayerPrefsKeys.VOLUME_SFX,
                PlayerPrefsKeys.VOLUME_CINEMATICS,
                PlayerPrefsKeys.VOLUME_VOICE_CHAT
                };

                return keyList;
            }
        }

        //  Mute & Unmute
        public void ToggleVolume(string key)
        {
            string tempKey = $"Temp{key}";
            float currentVolume = GetVolume(key);

            if (currentVolume <= 0)
            {
                SetVolume(key, GetVolume(tempKey));

                PlayerPrefs.DeleteKey(tempKey);
            }
            else
            {
                SetVolume(key, 0);

                PlayerPrefs.SetFloat(tempKey, currentVolume);
            }
        }

        public void SetVolume(string key, float volume)
        {
            _audioMixer.SetFloat(key, ToDecibels(volume));

            PlayerPrefs.SetFloat(key, volume);
        }

        public float GetVolume(string key)
        {
            _audioMixer.GetFloat(key, out float volume);
            volume = FromDecibels(volume);

            return PlayerPrefs.GetFloat(key, 0.5f);
        }

        /// <summary>
        /// Converts a linear value (0 to 1) to decibels (-80 to 20).
        /// </summary>
        /// <param name="value">The input linear value (0 to 1).</param>
        /// <returns>The corresponding value in decibels (-80 to 20).</returns>
        private float ToDecibels(float value)
        {
            value = Mathf.Clamp01(value);

            if (value <= 0.0001f) return -80f;
            return Mathf.Log10(value) * 20f + Mathf.Lerp(0, 20, value); //esto no deberia estar asi
        }

        /// <summary>
        /// Converts a decibel value  (-80 to 20) back to a linear scale (0 to 1).
        /// </summary>
        /// <param name="value">The input value in decibels (-80 to 20).</param>
        /// <returns>The corresponding linear value between (0 to 1).</returns>
        private float FromDecibels(float value)
        {
            value = Mathf.Clamp(value, -80f, 20f);

            float minLin = Mathf.Pow(10f, -80f / 20f);
            float maxLin = Mathf.Pow(10f, 20f / 20f);

            return (Mathf.Pow(10f, value / 20f) - minLin) / (maxLin - minLin);
        }

    }
}