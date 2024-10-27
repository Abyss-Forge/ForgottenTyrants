using UnityEngine;

public class MyCursorManager : Singleton<MyCursorManager>
{
    [SerializeField] private GameObject crosshairPrefab;

    public void Capture()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //crosshairPrefab.SetActive(true);
    }

    public void Release()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //crosshairPrefab.SetActive(false);
    }

    public bool IsCaptured()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public void EnableCrosshair()
    {
        crosshairPrefab.SetActive(true);
    }

    public void DisableCrosshair()
    {

    }

}
