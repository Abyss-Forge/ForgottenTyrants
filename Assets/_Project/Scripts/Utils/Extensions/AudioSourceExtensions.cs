using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class AudioSourceExtensions
    {

        /// <summary>
        /// Plays the audio clip attached to the AudioSource after an optional delay and returns a Task that completes when the audio clip finishes playing.
        /// </summary>
        /// <param name="audio">The AudioSource to play the audio clip from.</param>
        /// <param name="delay">The optional delay in seconds before playing the audio clip. Default is 0 seconds.</param>
        /// <returns>A Task that completes when the audio clip finishes playing.</returns>
        public static Task PlayAndAwaitFinish(this AudioSource audio, float delay = 0f)
        {
            if (delay > 0) Task.Delay(TimeSpan.FromSeconds(delay));
            audio.Play();
            return Task.Delay(TimeSpan.FromSeconds(audio.clip.length));
        }

        /// <summary>
        /// Fades in the audio over a specified duration.
        /// </summary>
        /// <param name="audio">The AudioSource to fade in.</param>
        /// <param name="fadeTime">The duration of the fade in seconds. Default is 1 second.</param>
        /// <returns>An IEnumerator that can be used in a coroutine to fade in the audio.</returns>
        public static IEnumerator FadeIn(this AudioSource audio, float fadeTime = 1) => Fade(audio, true, fadeTime);

        /// <summary>
        /// Fades out the audio over a specified duration.
        /// </summary>
        /// <param name="audio">The AudioSource to fade out.</param>
        /// <param name="fadeTime">The duration of the fade in seconds. Default is 1 second.</param>
        /// <returns>An IEnumerator that can be used in a coroutine to fade out the audio.</returns>
        public static IEnumerator FadeOut(this AudioSource audio, float fadeTime = 1) => Fade(audio, false, fadeTime);

        /// <summary>
        /// Fades the audio in or out over a specified duration.
        /// </summary>
        /// <param name="audio">The AudioSource to fade.</param>
        /// <param name="fadeIn">If true, fades in the audio; if false, fades out the audio.</param>
        /// <param name="fadeTime">The duration of the fade in seconds.</param>
        /// <returns>An IEnumerator that can be used in a coroutine to fade the audio.</returns>
        public static IEnumerator Fade(AudioSource audio, bool fadeIn, float fadeTime)
        {
            float currentTime = 0f, originTime = 1f, targetTime = 0f;   //fadeOut by default

            if (fadeIn) ExtensionMethods.Swap<float>(ref originTime, ref targetTime);

            while (currentTime < fadeTime)
            {
                currentTime += Time.deltaTime;

                audio.volume = Mathf.Lerp(originTime, targetTime, currentTime / fadeTime);

                yield return null;
            }
        }

    }
}