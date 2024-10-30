using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private float _walkSpeed, _lookSensivity, _maxForce, _jumpForce, _dashForce;
    private Vector2 _move, _look;
    private float _lookRotation;
    private bool _isGrounded, _canMove;

    private void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) Dash();
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputActions.Move, OnMove, true);
        MyInputManager.Instance.SubscribeToInput(EInputActions.Look, OnLook, true);
        MyInputManager.Instance.SubscribeToInput(EInputActions.Jump, OnJump, true);
        MyInputManager.Instance.SubscribeToInput(EInputActions.Dash, OnDash, true);
    }

    void FixedUpdate()
    {
        //physics belong inside fixed update
        Move();
    }

    void Update()
    {
        //animation
    }

    void LateUpdate()
    {
        //We move the camera in late update so all the movement has finished before positioning it
        Look();
    }

    private void Look()
    {
        //turn
        transform.Rotate(Vector3.up * _look.x * _lookSensivity);

        //look
        _lookRotation += -_look.y * _lookSensivity;
        _lookRotation = Mathf.Clamp(_lookRotation, -90, 90);
        _cameraHolder.transform.eulerAngles = new Vector3(_lookRotation, _cameraHolder.transform.eulerAngles.y, _cameraHolder.transform.eulerAngles.z);
    }

    private void Move()
    {
        //find target speed
        Vector3 currentVelocty = _rb.velocity;
        Vector3 targetVelocity = new Vector3(_move.x, 0, _move.y);
        targetVelocity *= _walkSpeed;

        //align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //calculate forces
        Vector3 velocityChange = targetVelocity - currentVelocty;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        //limit force
        Vector3.ClampMagnitude(velocityChange, _maxForce);

        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        Vector3 jumpForces = Vector3.zero;

        if (IsGrounded())
        {
            jumpForces = Vector3.up * _jumpForce;
        }

        jumpForces = Vector3.ClampMagnitude(jumpForces, _maxForce);

        _rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    private void Dash()
    {//TOTEST
        Vector3 dashDirection;

        if (_move == Vector2.zero)
        {
            dashDirection = _cameraHolder.transform.forward;
            //dashDirection.y = 0;
        }
        else
        {
            dashDirection = transform.TransformDirection(new Vector3(_move.x, 0, _move.y));
        }

        dashDirection.Normalize();
        dashDirection *= _dashForce;
        dashDirection = Vector3.ClampMagnitude(dashDirection, _maxForce);

        _rb.AddForce(dashDirection, ForceMode.Impulse);
    }


    private bool IsGrounded()
    {
        float rayLength = 1.2f;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength))
        {
            return true;
        }
        return false;
    }


}