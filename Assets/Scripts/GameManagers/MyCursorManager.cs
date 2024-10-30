using UnityEngine;

public class MyCursorManager : Singleton<MyCursorManager>
{
    [SerializeField] private GameObject _crosshairPrefab;

    void Start()
    {
        Release();
        Capture();
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

    public bool IsCaptured()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void EnableCrosshair()
    {
        _crosshairPrefab.SetActive(true);
    }

    public void DisableCrosshair()
    {
        _crosshairPrefab.SetActive(false);
    }

}
