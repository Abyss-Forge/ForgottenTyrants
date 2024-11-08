using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    CharacterController _cc;
    [SerializeField] private Camera _camera;

    [Header("Aesthetic")]
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private AnimationCurve _dashFovCurve;

    [Header("Config")]
    [SerializeField] private float _lookSensitivity;    //en 2 lineas separadas para que no clone el header por cada field
    [SerializeField] private float _walkSpeed, _gravityMultiplier, _jumpForce, _dashForce, _dashDuration, _dashFovChange, _dashCooldown;

    private Vector2 _move, _look;
    private float _lookRotation;
    private bool _isGrounded, _canMove, _gravityEnabled, _canJump, _canDash;
    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

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
        if (Input.GetKeyDown(KeyCode.Tab)) MySceneManager.Instance.LoadSceneWithLoadingScreen(Scene.Next);//test
    }

    void LateUpdate()
    {
        //we move the camera in late update so all the movement has finished before positioning it
        //Look();
    }

    private void Look()
    {
        //rotate player
        transform.Rotate(Vector3.up * _look.x * _lookSensitivity);

        //rotate camera vertically (since camera is a child of player, it  automarically rotates on x axis)
        _lookRotation += -_look.y * _lookSensitivity;
        _lookRotation = Mathf.Clamp(_lookRotation, -90, 90);
        _camera.transform.eulerAngles = new Vector3(_lookRotation, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z);
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
        _trail.emitting = true;
        float originalFov = _camera.fieldOfView;

        Vector3 dashDirection = transform.right * _move.x + transform.forward * _move.y;

        float elapsedTime = 0f;
        while (elapsedTime < _dashDuration)
        {
            float progress = elapsedTime / _dashDuration;

            float curveValue = _dashFovCurve.Evaluate(progress);
            _camera.fieldOfView = originalFov + (curveValue * _dashFovChange);

            _cc.Move(dashDirection * _dashForce * Time.deltaTime / _dashDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _gravityEnabled = true;
        _trail.emitting = false;
        _camera.fieldOfView = originalFov;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

}
