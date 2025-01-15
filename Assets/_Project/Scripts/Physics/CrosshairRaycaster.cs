using UnityEngine;

public static class CrosshairRaycaster
{
    private static bool PerformCenterRaycast(out RaycastHit hitInfo)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray rayOrigin = Camera.main.ScreenPointToRay(screenCenter); // Camera.main.transform.forward
        return Physics.Raycast(rayOrigin, out hitInfo);
    }

    public static GameObject GetImpactObject()
    {
        if (PerformCenterRaycast(out RaycastHit hitInfo))
        {
            return hitInfo.collider?.gameObject;
        }

        return null;
    }

    public static Vector3? GetImpactPosition()
    {
        if (PerformCenterRaycast(out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return null;
    }

}