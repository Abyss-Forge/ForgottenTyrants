using System.Collections.Generic;
using Systems.SingletonPattern;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Systems.AudioManagement
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioMixer _audioMixer;

        public AudioHelper Helper;

        private List<AudioSource> _allSceneAudioSources;

        protected override void OnAwake()
        {
            Helper = new(_audioMixer);
            Helper.LoadPrefs();

            RefreshAudioSources();
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshAudioSources();
            //PlayMusicForScene(SceneUtils.Current);
        }

        private void RefreshAudioSources()
        {
            _allSceneAudioSources = new();
            _allSceneAudioSources.AddRange(FindObjectsOfType<AudioSource>());
        }

        public void PauseAllAudio()
        {
            foreach (AudioSource audio in _allSceneAudioSources)
            {
                audio.Pause();
            }
        }

        public void ResumeAllAudio()
        {
            foreach (AudioSource audio in _allSceneAudioSources)
            {
                audio.UnPause();
            }
        }

    }
}