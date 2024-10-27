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

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private bool canMove = true;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        MyCursorManager.Instance.Capture();
        InitCameraPosition();
    }

    void Update()
    {
        if (canMove)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            //MyInputManager.Instance.IsKeyActive(eBindableAction.Run);
            bool runPressed = Input.GetKey(KeyCode.LeftShift);
            bool jumpPressed = Input.GetKey(KeyCode.Space);

            float crouchedSpeedMultiplier = 1;

            //Running logic
            float curSpeedX = (runPressed ? _runningSpeed : _walkingSpeed) * crouchedSpeedMultiplier * Input.GetAxis("Vertical");
            float curSpeedY = (runPressed ? _runningSpeed : _walkingSpeed) * crouchedSpeedMultiplier * Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            //Jumping logic
            if (jumpPressed && characterController.isGrounded)
            {
                moveDirection.y = _jumpForce;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if (!characterController.isGrounded)
            {
                moveDirection.y -= _gravity * Time.deltaTime;
            }
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // Player and Camera rotation
        if (MyCursorManager.Instance.IsCaptured())
        {
            rotationX += -Input.GetAxis("Mouse Y") * _lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -_lookHeightLimit, _lookHeightLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
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