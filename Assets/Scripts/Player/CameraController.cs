using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField, Range(0f, 90f)] private float _upperVerticalLimit = 90f, _lowerVerticalLimit = 90f;
    [SerializeField] private float _horizontalSensitivity = 15f, _verticalSensitivity = 15f;
    [SerializeField] private bool _smoothCameraRotation;
    [SerializeField, Range(1f, 50f)] private float _cameraSmoothingFactor = 25f;

    private Camera _cam;
    private Vector2 _look;
    private float _currentXAngle, _currentYAngle;

    void Awake()
    {
        _cam = GetComponentInChildren<Camera>();

        _currentXAngle = transform.localRotation.eulerAngles.x;
        _currentYAngle = transform.localRotation.eulerAngles.y;
    }

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputActions.Look, OnLook);
    }

    void Update()
    {
        RotateCamera(_look.x * _horizontalSensitivity, -_look.y * _verticalSensitivity);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    private void RotateCamera(float horizontalInput, float verticalInput)
    {
        if (_smoothCameraRotation)
        {
            horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * _cameraSmoothingFactor);
            verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * _cameraSmoothingFactor);
        }

        _currentXAngle += verticalInput * Time.deltaTime;
        _currentYAngle += horizontalInput * Time.deltaTime;

        _currentXAngle = Mathf.Clamp(_currentXAngle, -_upperVerticalLimit, _lowerVerticalLimit);

        transform.localRotation = Quaternion.Euler(_currentXAngle, 0, 0);
        _player.transform.localRotation = Quaternion.Euler(0, _currentYAngle, 0);
    }

}