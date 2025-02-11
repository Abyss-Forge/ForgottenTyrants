using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ParticleSystemExtensions
    {

        /// <summary>
        /// Plays the particle system and awaits until it finishes playing.
        /// </summary>
        /// <param name="particles">The ParticleSystem to play.</param>
        /// <returns>A Task that completes when the particles finish playing.</returns>
        public static Task PlayAndAwaitFinish(this ParticleSystem particles)
        {
            particles.Play();
            return Task.Delay(TimeSpan.FromSeconds(particles.main.duration));
        }

    }
}