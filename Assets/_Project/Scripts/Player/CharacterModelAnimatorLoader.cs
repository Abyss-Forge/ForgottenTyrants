using System;
using ForgottenTyrants;
using Systems.ServiceLocator;
using UnityEngine;
using Utils.Extensions;

public class CharacterModelAnimatorLoader : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Global.Get(out PlayerInfo player).Get(out Animator oldAnimator);
        Transform spawnPoint = oldAnimator.transform;

        Animator newAnimator = Instantiate(player.ClientData.Race.ModelRoot, spawnPoint.position, spawnPoint.rotation, spawnPoint.parent);
        newAnimator.runtimeAnimatorController = oldAnimator.runtimeAnimatorController;
        newAnimator.applyRootMotion = false;
        newAnimator.gameObject.SetLayersRecursively(Layer.Player);
        // newAnimator.GetComponent<NetworkObject>().Spawn();

        ServiceLocator.Global.Deregister<Animator>();
        ServiceLocator.Global.Register<Animator>(newAnimator);

        foreach (Renderer renderer in oldAnimator.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        var syncer = newAnimator.GetComponent<AnimatorEnabledStateSync>();
        syncer.Initialize();

        CopyHitbox(newAnimator, oldAnimator);
    }

    private void CopyHitbox(Animator newAnimator, Animator oldAnimator)
    {
        if (!newAnimator.isHuman || !oldAnimator.isHuman) { return; }

        BodyPart[] oldBones = FindObjectsOfType<BodyPart>();

        foreach (var oldBone in oldBones)
        {
            HumanBodyBones boneType = oldAnimator.GetBoneFromTransform(oldBone.transform);
            if (boneType == HumanBodyBones.LastBone) { continue; }

            GameObject oldBoneGO = oldBone.gameObject;
            GameObject newBoneGO = newAnimator.GetBoneTransform(boneType).gameObject;

            newBoneGO.CopyComponent<Collider>(oldBoneGO);

            newBoneGO.GetOrAdd<Rigidbody>().Copy(oldBoneGO.GetComponent<Rigidbody>());

            //newBoneGO.CopyComponent<BodyPart>(oldBoneGO).Initialize();

            CharacterJoint newBoneJoint = newBoneGO.CopyComponent<CharacterJoint>(oldBoneGO);
            if (newBoneJoint)
            {
                HumanBodyBones connectedBone = oldAnimator.GetBoneFromTransform(newBoneJoint.connectedBody.transform);
                Transform connectedBoneTr = newAnimator.GetBoneTransform(connectedBone);
                newBoneJoint.connectedBody = connectedBoneTr.gameObject.GetOrAdd<Rigidbody>();
            }
        }
    }

    private void SwapModel(GameObject newModelPrefab, GameObject oldModel)
    {
        GameObject newModel = Instantiate(newModelPrefab);
        newModel.SetActive(false);

        SkinnedMeshRenderer oldRenderer = oldModel.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer newRenderer = newModel.GetComponentInChildren<SkinnedMeshRenderer>();

        oldRenderer.sharedMesh = newRenderer.sharedMesh;
        oldRenderer.materials = newRenderer.materials;

        Animator oldAnimator = oldModel.GetComponent<Animator>();
        Animator newAnimator = newModel.GetComponent<Animator>();

        oldAnimator.avatar = newAnimator.avatar;
        foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (bone == HumanBodyBones.LastBone) { continue; }

            Transform oldBone = oldAnimator.GetBoneTransform(bone);
            Transform newBone = newAnimator.GetBoneTransform(bone);

            if (oldBone != null && newBone != null)
            {
                oldBone.localPosition = newBone.localPosition;
                oldBone.localRotation = newBone.localRotation;
                oldBone.localScale = newBone.localScale;
            }
        }

        Destroy(newModel);
    }

    private void Test(Animator targetAnimator)
    {
        RagdollBuilder.BuildRagdoll(
            targetAnimator.GetBoneTransform(HumanBodyBones.Hips),
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftFoot),
            targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
            targetAnimator.GetBoneTransform(HumanBodyBones.RightFoot),
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm),
            targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm),
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm),
            targetAnimator.GetBoneTransform(HumanBodyBones.Spine),
            targetAnimator.GetBoneTransform(HumanBodyBones.Head),
            20f,
            false
        );

        targetAnimator.enabled = false;
        foreach (var rb in targetAnimator.GetComponentsInChildren<Rigidbody>())//pfff no me gusta nada tener que usar esto pero la alternativa es peor
        {
            rb.isKinematic = false;
        }
    }


}