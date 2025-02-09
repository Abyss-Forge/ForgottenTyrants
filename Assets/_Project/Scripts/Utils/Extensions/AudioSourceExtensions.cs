using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class AudioSourceExtensions
    {

        public static Task PlayAndAwaitFinish(this AudioSource audio)
        {
            audio.Play();
            return Task.Delay(TimeSpan.FromSeconds(audio.clip.length));
        }

    }
}