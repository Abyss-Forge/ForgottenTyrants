using System.Collections;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ParticleSystemExtensions
    {

        public static IEnumerator PlayAndDestroy(this ParticleSystem particles, bool disableInsteadOfDestroy = false)
        {
            particles.Play();
            yield return new WaitForSeconds(particles.main.duration);

            if (disableInsteadOfDestroy)
            {
                particles.gameObject.SetActive(false);
            }
            else
            {
                particles.gameObject.Destroy();
            }
        }

    }
}