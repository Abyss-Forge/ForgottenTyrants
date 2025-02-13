using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ParticleSystemExtensions
    {

        /// <summary>
        /// Plays the particles with a delay specified in seconds. 
        /// </summary>
        /// <param name="particles">The ParticleSystem to await.</param>
        /// <param name="delay"> Delay time specified in seconds.</param>
        public static void PlayDelayed(this ParticleSystem particles, float delay)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                particles.Play();
            });
        }

        /// <summary>
        /// Awaits the completion of the particle system's playback, with an optional delay before starting.
        /// </summary>
        /// <param name="particles">The ParticleSystem to await.</param>
        /// <param name="delay">Optional delay time specified in seconds before starting the particle system. Default is 0 seconds.</param>
        /// <returns>A Task that completes after the particle system has finished playing.</returns>
        public static Task PlayAndAwaitFinish(this ParticleSystem particles, float delay = 0f)
        {
            if (delay > 0) Task.Delay(TimeSpan.FromSeconds(delay));
            particles.Play();
            return Task.Delay(TimeSpan.FromSeconds(particles.main.duration));
        }

    }
}