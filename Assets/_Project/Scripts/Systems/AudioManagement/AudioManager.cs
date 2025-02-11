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

        private AudioHelper _audioHelper;

        private List<AudioSource> AllSceneAudioSources;

        protected override void OnAwake()
        {
            _audioHelper = new(_audioMixer);
            _audioHelper.LoadPrefs();

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
            AllSceneAudioSources = new();
            AllSceneAudioSources.AddRange(FindObjectsOfType<AudioSource>());
        }

        public void PauseAllAudio()
        {
            foreach (AudioSource audio in AllSceneAudioSources)
            {
                audio.Pause();
            }
        }

        public void ResumeAllAudio()
        {
            foreach (AudioSource audio in AllSceneAudioSources)
            {
                audio.UnPause();
            }
        }

    }
}