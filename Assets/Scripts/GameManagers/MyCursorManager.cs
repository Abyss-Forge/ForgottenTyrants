using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCursorManager : Singleton<MyCursorManager>
{
    [SerializeField] private CrosshairController _crosshairController;

    public bool IsCaptured => GetCaptured();

    void Start()
    {
        Release();
        Capture();
    }

    private bool GetCaptured()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void Capture()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Release()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public GameObject GetCrosshairTarget()
    {
        return _crosshairController.TargetObject;
    }

    public void EnableCrosshair()
    {
        _crosshairController.gameObject.SetActive(true);
    }

    public void DisableCrosshair()
    {
        _crosshairController.gameObject.SetActive(false);
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
