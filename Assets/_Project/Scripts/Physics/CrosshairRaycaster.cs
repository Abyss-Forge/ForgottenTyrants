using UnityEngine;

public static class CrosshairRaycaster
{
    private static bool PerformCenterRaycast(out RaycastHit hitInfo, int layerMask = Physics.AllLayers)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray rayOrigin = Camera.main.ScreenPointToRay(screenCenter); // Camera.main.transform.forward
        return Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, layerMask);
    }

    public static GameObject GetImpactObject(int layerMask = Physics.AllLayers)
    {
        return PerformCenterRaycast(out RaycastHit hitInfo, layerMask) ? hitInfo.collider?.gameObject : null;
    }

    public static Vector3? GetImpactPosition(int layerMask = Physics.AllLayers)
    {
        return PerformCenterRaycast(out RaycastHit hitInfo, layerMask) ? hitInfo.point : null;
    }

}