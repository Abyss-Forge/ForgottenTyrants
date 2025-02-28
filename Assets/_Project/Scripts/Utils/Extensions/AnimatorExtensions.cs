using System;
using UnityEngine;

namespace Utils.Extensions
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Retrieves the corresponding <see cref="HumanBodyBones"/> enumeration value for a given <see cref="Transform"/> in a human <see cref="Animator"/>.
        /// </summary>
        /// <param name="animator">The <see cref="Animator"/> component to search within. Must be a human animator.</param>
        /// <param name="boneTransform">The <see cref="Transform"/> of the bone to find.</param>
        /// <returns>
        /// The <see cref="HumanBodyBones"/> value that corresponds to the given <see cref="Transform"/>, or <c>null</c> if the transform does not correspond to any human bone or if the animator is not human.
        /// </returns>
        public static HumanBodyBones GetBoneFromTransform(this Animator animator, Transform boneTransform)
        {
            if (!animator.isHuman) { throw new Exception("The animator is not humanoid"); }

            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) { continue; } // We skip LastBone, due to it not being a real bone

                Transform tr = animator.GetBoneTransform(bone);
                if (tr == boneTransform) { return bone; }
            }

            return HumanBodyBones.LastBone;
        }

    }
}