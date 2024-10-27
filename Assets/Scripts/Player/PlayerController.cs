using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private GameObject _camHolder;
    [SerializeField] private float _speed, _lookSensivity, _maxForce, _jumpForce;
    private Vector2 _move, _look;
    private float _lookRotation;
    private bool _isGrounded;

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        MyCursorManager.Instance.Capture();
    }

    void FixedUpdate()
    {
        //physics go inside fixed update
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
        _camHolder.transform.eulerAngles = new Vector3(_lookRotation, _camHolder.transform.eulerAngles.y, _camHolder.transform.eulerAngles.z);
    }

    private void Move()
    {
        //find target speed
        Vector3 currentVelocty = _rb.velocity;
        Vector3 targetVelocity = new Vector3(_move.x, 0, _move.y);
        targetVelocity *= _speed;

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

        _rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.2f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            return true;
        }
        return false;
    }


}