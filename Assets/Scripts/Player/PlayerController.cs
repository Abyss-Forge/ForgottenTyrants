using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _cc;
    [SerializeField] private Camera _cameraHolder;
    [SerializeField] private TrailRenderer _tr;
    [SerializeField] private AnimationCurve _dashFovCurve;

    [SerializeField] private float _lookSensitivity, _walkSpeed, _gravityMultiplier, _jumpForce, _dashForce, _dashDuration, _dashFovChange, _dashCooldown;

    private Vector2 _move, _look;
    private float _lookRotation;
    private bool _isGrounded, _canMove, _gravityEnabled, _canJump, _canDash;
    private Vector3 _velocity;

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
        if (context.performed && _isGrounded && _canJump) Jump();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && _canDash) StartCoroutine(Dash());
    }

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _canMove = true;
        _canJump = true;
        _canDash = true;
        _gravityEnabled = true;
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
        if (_canMove) Move();
        if (_gravityEnabled) ApplyGravity();
    }

    void Update()
    {
        //animation
        Debug.Log(_velocity.y);
    }

    void LateUpdate()
    {
        //we move the camera in late update so all the movement has finished before positioning it
        Look();
    }

    private void Look()
    {
        //turn
        transform.Rotate(Vector3.up * _look.x * _lookSensitivity);

        //look
        _lookRotation += -_look.y * _lookSensitivity;
        _lookRotation = Mathf.Clamp(_lookRotation, -90, 90);
        _cameraHolder.transform.eulerAngles = new Vector3(_lookRotation, _cameraHolder.transform.eulerAngles.y, _cameraHolder.transform.eulerAngles.z);
    }

    private void Move()
    {
        Vector3 moveDirection = transform.right * _move.x + transform.forward * _move.y;
        moveDirection *= _walkSpeed;

        _velocity.x = moveDirection.x;
        _velocity.z = moveDirection.z;

        _cc.Move(_velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _isGrounded = _cc.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; //keep the character grounded
        }
        else
        {
            _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
        }
    }

    private void Jump()
    {
        _velocity.y = Mathf.Sqrt(-Physics.gravity.y * _gravityMultiplier * _jumpForce);
    }

    private IEnumerator Dash()
    {
        _canDash = false;

        _gravityEnabled = false;
        _tr.emitting = true;
        float originalFov = _cameraHolder.fieldOfView;

        Vector3 dashDirection = transform.right * _move.x + transform.forward * _move.y;

        float elapsedTime = 0f;
        while (elapsedTime < _dashDuration)
        {
            float progress = elapsedTime / _dashDuration;

            float curveValue = _dashFovCurve.Evaluate(progress);
            _cameraHolder.fieldOfView = originalFov + (curveValue * _dashFovChange);

            _cc.Move(dashDirection * _dashForce * Time.deltaTime / _dashDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _gravityEnabled = true;
        _tr.emitting = false;
        _cameraHolder.fieldOfView = originalFov;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

}
