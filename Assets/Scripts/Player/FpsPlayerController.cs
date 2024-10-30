using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FpsPlayerController : MonoBehaviour
{

    private CharacterController characterController;

    [Header("Camera")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private bool _isThirdPersonEnabled = false;
    [SerializeField] private float _lookSpeed = 2f;
    [SerializeField] private float _lookHeightLimit = 90f;

    [Header("Movement")]
    [SerializeField] private float _walkingSpeed = 7.5f;
    [SerializeField] private float _runningSpeed = 11.5f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _gravity = 20f;

    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0f;
    private bool _canMove = true;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        InitCameraPosition();
    }

    void Start()
    {
        MyCursorManager.Instance.Capture();
    }

    void Update()
    {
        if (_canMove)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            //MyInputManager.Instance.IsKeyActive(eBindableAction.Run);
            bool dashPressed = Input.GetKey(KeyCode.LeftShift);
            bool jumpPressed = Input.GetKey(KeyCode.Space);

            float crouchedSpeedMultiplier = 1;

            //Running logic
            float curSpeedX = _walkingSpeed * crouchedSpeedMultiplier * Input.GetAxis("Vertical");
            float curSpeedY = _walkingSpeed * crouchedSpeedMultiplier * Input.GetAxis("Horizontal");
            float movementDirectionY = _moveDirection.y;
            _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            //Jumping logic
            if (jumpPressed && characterController.isGrounded)
            {
                _moveDirection.y = _jumpForce;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if (!characterController.isGrounded)
            {
                _moveDirection.y -= _gravity * Time.deltaTime;
            }
            characterController.Move(_moveDirection * Time.deltaTime);
        }

        // Player and Camera rotation
        if (MyCursorManager.Instance.IsCaptured())
        {
            _rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -_lookHeightLimit, _lookHeightLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeed, 0);
        }
    }

    private void InitCameraPosition()
    {
        Vector3 cameraPosition = _playerCamera.transform.position;
        if (_isThirdPersonEnabled)
        {
            cameraPosition.z = -3f;
            cameraPosition.y = 2f;
        }
        _playerCamera.transform.position = cameraPosition;
    }
}