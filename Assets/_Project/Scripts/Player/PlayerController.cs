using System;
using System.Collections;
using System.Threading.Tasks;
using Systems.EventBus;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController _characterController;

    [SerializeField] private Camera _camera;

    [Header("Aesthetic")]
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private AnimationCurve _dashFovCurve;

    [Header("Config")]
    [SerializeField] private float _lookSensitivityX;    //en 2 lineas separadas para que no clone el header por cada field
    [SerializeField] private float _walkSpeed, _gravityMultiplier, _jumpForce, _dashForce, _dashDuration, _dashCooldown, _dashFovChange;

    private Vector3 _velocity;
    private Vector2 _move, _look;
    private float _lookRotationX;
    private bool _isGrounded, _isDashing, _isDashOnCooldown;

    public bool IsGravityEnabled { get; set; } = true;
    public bool CanMove { get; set; } = true;
    public bool CanJump { get; set; } = true;
    public bool CanDash { get; set; } = true;

    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
    private EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;
    private EventBinding<PlayerMovementEvent> _playerMovementEventBinding;

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
        if (context.performed && CanJump && _isGrounded) Jump();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && CanDash && !_isDashing && !_isDashOnCooldown) StartCoroutine(Dash());
    }

    void Awake()
    {
        CursorHelper.Capture();
        // _characterController = GetComponent<CharacterController>();

        //Diego troleando
        StartGlowingEffect(5);
        _currentSpeed = _walkSpeed;
    }

    void OnEnable()
    {
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandleDeathEvent);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);
        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawnEvent);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);

        _playerMovementEventBinding = new EventBinding<PlayerMovementEvent>(HandleMovementEvent);
        EventBus<PlayerMovementEvent>.Register(_playerMovementEventBinding);

        MyInputManager.Instance.Subscribe(EInputAction.MOVE, OnMove);
        MyInputManager.Instance.Subscribe(EInputAction.LOOK, OnLook);
        MyInputManager.Instance.Subscribe(EInputAction.JUMP, OnJump);
        MyInputManager.Instance.Subscribe(EInputAction.DASH, OnDash);
    }

    void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);

        EventBus<PlayerMovementEvent>.Deregister(_playerMovementEventBinding);

        MyInputManager.Instance.Unsubscribe(EInputAction.MOVE, OnMove);
        MyInputManager.Instance.Unsubscribe(EInputAction.LOOK, OnLook);
        MyInputManager.Instance.Unsubscribe(EInputAction.JUMP, OnJump);
        MyInputManager.Instance.Unsubscribe(EInputAction.DASH, OnDash);
    }

    void FixedUpdate()  //physics belong inside fixed update
    {
        if (CanMove) Move();
        if (IsGravityEnabled) ApplyGravity();

        if (_isDashing) _velocity.y = 0; // esto hace que el dash no tenga verticalidad, me parece un diseño de mierda par aun juego 3D pero bueno
    }

    void Update()
    {
        //animation
    }

    void LateUpdate()   //we move the camera in late update so all the movement has finished before positioning it
    {
        //  Look();
    }

    private void Look()
    {
        //rotate player
        transform.Rotate(Vector3.up * _look.x * _lookSensitivityX);

        //rotate camera vertically (since camera is a child of player, it  automarically rotates on X axis)
        _lookRotationX += -_look.y * _lookSensitivityX;
        _lookRotationX = Mathf.Clamp(_lookRotationX, -90, 90);
        _camera.transform.eulerAngles = new Vector3(_lookRotationX, _camera.transform.eulerAngles.y, _camera.transform.eulerAngles.z);
    }

    private void Move()
    {
        Vector3 moveDirection = transform.right * _move.x + transform.forward * _move.y;
        moveDirection *= _currentSpeed;     //_walkSpeed

        _velocity.x = moveDirection.x;
        _velocity.z = moveDirection.z;

        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _isGrounded = _characterController.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = Physics.gravity.y; //keep the character grounded
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

    public IEnumerator Dash()
    {
        _isDashing = true;
        _isDashOnCooldown = true;

        _trail.emitting = true;
        float originalFov = _camera.fieldOfView;

        Vector3 dashDirection = transform.right * _move.x + transform.forward * _move.y;

        if (dashDirection == Vector3.zero) //default direction when no input
        {
            dashDirection = transform.forward;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _dashDuration)
        {
            float progress = elapsedTime / _dashDuration;

            float curveValue = _dashFovCurve.Evaluate(progress);
            _camera.fieldOfView = originalFov + (curveValue * _dashFovChange);

            _characterController.Move(dashDirection * _dashForce * Time.deltaTime / _dashDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _trail.emitting = false;
        _camera.fieldOfView = originalFov;

        _isDashing = false;
        yield return new WaitForSeconds(_dashCooldown + _dashDuration); //  avoid counting the performing time as cooldown
        _isDashOnCooldown = false;
    }

    private void HandleDeathEvent()
    {
        FreezeMovement(true);
    }

    private void HandleRespawnEvent()
    {
        FreezeMovement(false);
    }

    private void HandleMovementEvent(PlayerMovementEvent @event)
    {
        FreezeMovement(!@event.Activate);
    }

    private void FreezeMovement(bool freeze = true)
    {
        bool b = !freeze;

        CanMove = b;
        CanJump = b;
        CanDash = b;
        IsGravityEnabled = b;
    }

    #region cosas de diego

    [SerializeField] private Color _emissionColor = Color.yellow;
    [SerializeField] private float _emissionIntensity = 1.2f;
    [SerializeField] private Renderer _renderer;
    private Material _objectMaterial;

    private float _currentSpeed;
    public float JumpForce => _jumpForce;
    public Vector3 Velocity => _velocity;

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
    }

    public void SetJumpForce(float jumpForce)
    {
        _jumpForce = jumpForce;
    }

    public void SetSpeed(float delta)
    {
        // Suma o resta a la velocidad actual
        _currentSpeed += delta;
        // Asegúrate de que no quede por debajo de 0 si no quieres velocidades negativas
        // currentSpeed = Mathf.Max(0, currentSpeed);
    }

    public float GetSpeed()
    {
        return _currentSpeed;
    }

    void Start()
    {
        if (_renderer != null) _objectMaterial = _renderer.material;// Usamos el material instanciado para no afectar materiales compartidos
    }

    public void StartGlowingEffect(float duration)
    {
        StartCoroutine(GlowingEffect(duration));
    }

    private IEnumerator GlowingEffect(float duration)
    {
        ActivateEmission();
        yield return new WaitForSeconds(duration);
        DeactivateEmission();
    }

    private void ActivateEmission()
    {
        if (_objectMaterial != null)
        {
            _objectMaterial.EnableKeyword("_EMISSION");

            Color finalEmissionColor = _emissionColor * _emissionIntensity;
            _objectMaterial.SetColor("_EmissionColor", finalEmissionColor);
        }
    }

    public void DeactivateEmission()
    {
        if (_objectMaterial != null) _objectMaterial.DisableKeyword("_EMISSION");
    }

    #endregion
}