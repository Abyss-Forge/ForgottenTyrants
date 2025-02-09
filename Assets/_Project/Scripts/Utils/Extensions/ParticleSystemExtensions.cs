using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ParticleSystemExtensions
    {

        public static Task PlayAndAwaitFinish(this ParticleSystem particles)
        {
            particles.Play();
            return Task.Delay(TimeSpan.FromSeconds(particles.main.duration));
        }

    }
}