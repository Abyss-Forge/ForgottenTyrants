using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform objectToFollow;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private bool rotationEnabled;
    [SerializeField] private bool hideShadows;
    private float storedShadowDistance;

    void LateUpdate()
    {
        Vector3 newPosition = objectToFollow.position;
        newPosition.y = minimapCamera.transform.position.y;
        minimapCamera.transform.position = newPosition;

        float angle = rotationEnabled ? objectToFollow.eulerAngles.y : 0;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, angle, 0f);
    }

    void OnEnable()
    {
        Camera.onPreCull += HandleOnPreCull;
        Camera.onPostRender += HandleOnPostRender;
    }

    void OnDisable()
    {
        Camera.onPreCull -= HandleOnPreCull;
        Camera.onPostRender -= HandleOnPostRender;
    }

    private void HandleOnPreCull(Camera cam)
    {
        if (cam == minimapCamera && hideShadows)
        {
            storedShadowDistance = QualitySettings.shadowDistance;
            QualitySettings.shadowDistance = 0;
        }
    }

    private void HandleOnPostRender(Camera cam)
    {
        if (cam == minimapCamera && hideShadows)
        {
            QualitySettings.shadowDistance = storedShadowDistance;
        }
    }

}
