using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCursorManager : Singleton<MyCursorManager>
{
    [SerializeField, RequiredField] private CrosshairController _crosshairController;

    public GameObject GetCrosshairTarget()
    {
        return _crosshairController.TargetObject;
    }

    public Vector3? GetCrosshairImpactPoint()
    {
        if (_crosshairController == null)
        {
            Debug.LogWarning("CrosshairController no está asignado.");
            return null;
        }

        Ray rayOrigin = Camera.main.ScreenPointToRay(_crosshairController.transform.position);

        if (Physics.Raycast(rayOrigin, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return null;
    }

    public GameObject GetCrosshairImpactObject()
    {
        if (_crosshairController == null)
        {
            Debug.LogWarning("CrosshairController no está asignado.");
            return null;
        }

        Ray rayOrigin = Camera.main.ScreenPointToRay(_crosshairController.transform.position);

        if (Physics.Raycast(rayOrigin, out RaycastHit hitInfo))
        {
            return hitInfo.collider.gameObject;
        }

        return null;
    }

}
