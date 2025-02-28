using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RagdollBuilder
{
    private class BoneInfo
    {
        public string name;
        public Transform anchor;
        public CharacterJoint joint;
        public BoneInfo parent;
        public float minLimit;
        public float maxLimit;
        public float swingLimit;
        public Vector3 axis;
        public Vector3 normalAxis;
        public float radiusScale;
        public Type colliderType;
        public List<BoneInfo> children = new List<BoneInfo>();
        public float density;
        public float summedMass;
    }

    public static void BuildRagdoll(
        Transform pelvis, Transform leftHips, Transform leftKnee, Transform leftFoot,
        Transform rightHips, Transform rightKnee, Transform rightFoot,
        Transform leftArm, Transform leftElbow, Transform rightArm, Transform rightElbow,
        Transform middleSpine, Transform head, float totalMass = 20f, bool flipForward = false)
    {
        if (pelvis == null || head == null)
        {
            Debug.LogError("Pelvis and Head must be assigned");
            return;
        }

        List<BoneInfo> bones = new List<BoneInfo>();
        BoneInfo rootBone = new BoneInfo { name = "Pelvis", anchor = pelvis, density = 2.5f };
        bones.Add(rootBone);

        AddMirroredJoint("Hips", leftHips, rightHips, rootBone, bones, Vector3.right, Vector3.forward, -20f, 70f, 30f, typeof(CapsuleCollider), 0.3f, 1.5f);
        AddMirroredJoint("Knee", leftKnee, rightKnee, FindBone("Hips", bones), bones, Vector3.right, Vector3.forward, -80f, 0f, 0f, typeof(CapsuleCollider), 0.25f, 1.5f);
        AddJoint("Middle Spine", middleSpine, rootBone, bones, Vector3.right, Vector3.forward, -20f, 20f, 10f, null, 1f, 2.5f);
        AddMirroredJoint("Arm", leftArm, rightArm, FindBone("Middle Spine", bones), bones, Vector3.up, Vector3.forward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
        AddMirroredJoint("Elbow", leftElbow, rightElbow, FindBone("Arm", bones), bones, Vector3.forward, Vector3.up, -90f, 0f, 0f, typeof(CapsuleCollider), 0.2f, 1f);
        AddJoint("Head", head, FindBone("Middle Spine", bones), bones, Vector3.right, Vector3.forward, -40f, 25f, 25f, null, 1f, 1f);

        BuildRagdollComponents(bones, totalMass);
    }

    private static void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, BoneInfo parent, List<BoneInfo> bones, Vector3 twistAxis, Vector3 swingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
    {
        AddJoint("Left " + name, leftAnchor, parent, bones, twistAxis, swingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
        AddJoint("Right " + name, rightAnchor, parent, bones, twistAxis, swingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
    }

    private static void AddJoint(string name, Transform anchor, BoneInfo parent, List<BoneInfo> bones, Vector3 twistAxis, Vector3 swingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
    {
        if (anchor == null) return;
        BoneInfo bone = new BoneInfo { name = name, anchor = anchor, axis = twistAxis, normalAxis = swingAxis, minLimit = minLimit, maxLimit = maxLimit, swingLimit = swingLimit, density = density, colliderType = colliderType, radiusScale = radiusScale, parent = parent };
        parent.children.Add(bone);
        bones.Add(bone);
    }

    private static BoneInfo FindBone(string name, List<BoneInfo> bones)
    {
        return bones.Find(b => b.name.Contains(name));
    }

    private static void BuildRagdollComponents(List<BoneInfo> bones, float totalMass)
    {
        foreach (BoneInfo bone in bones)
        {
            if (bone.anchor == null) continue;
            Rigidbody rb = bone.anchor.gameObject.AddComponent<Rigidbody>();
            rb.mass = bone.density;
            if (bone.parent != null)
            {
                CharacterJoint joint = bone.anchor.gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = bone.parent.anchor.GetComponent<Rigidbody>();
                joint.axis = bone.axis;
                joint.swingAxis = bone.normalAxis;
                SoftJointLimit limit = new SoftJointLimit { limit = bone.minLimit };
                joint.lowTwistLimit = limit;
                limit.limit = bone.maxLimit;
                joint.highTwistLimit = limit;
                limit.limit = bone.swingLimit;
                joint.swing1Limit = limit;
            }
            if (bone.colliderType == typeof(CapsuleCollider))
            {
                CapsuleCollider col = bone.anchor.gameObject.AddComponent<CapsuleCollider>();
                col.radius = bone.radiusScale * 0.5f;
                col.height = 1.0f;
            }
        }
        NormalizeMass(bones, totalMass);
    }

    private static void NormalizeMass(List<BoneInfo> bones, float totalMass)
    {
        float currentMass = 0f;
        foreach (BoneInfo bone in bones) currentMass += bone.anchor.GetComponent<Rigidbody>().mass;
        float scale = totalMass / currentMass;
        foreach (BoneInfo bone in bones) bone.anchor.GetComponent<Rigidbody>().mass *= scale;
    }
}
