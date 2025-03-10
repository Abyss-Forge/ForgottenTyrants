using Systems.EventBus;
using Systems.FSM;
using Systems.GameManagers;
using Systems.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField, Range(0f, 90f)] private float _upperVerticalLimit = 90f, _lowerVerticalLimit = 90f;
    [SerializeField] private float _horizontalSensitivity = 15f, _verticalSensitivity = 15f;
    [SerializeField] private bool _smoothCameraRotation;
    [SerializeField, Range(1f, 50f)] private float _cameraSmoothingFactor = 25f;

    private Camera _camera;
    private Vector2 _look, _move;
    private float _lookAngleX, _lookAngleY;
    private float _moveAngleX, _moveAngleY;

    private Transform _followedTarget;
    private Vector3 _defaultPosition, _initialLocalPosition;

    EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
    EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

    #region Setup

    public enum ECameraMode
    {
        THIRD_PERSON, ORBITAL, FREE
    }

    public FiniteStateMachine<ECameraMode> _fsm { get; private set; }

    void Awake()
    {
        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(ECameraMode.THIRD_PERSON);

        _camera = GetComponentInChildren<Camera>();
        _lookAngleX = transform.localRotation.eulerAngles.x;
        _lookAngleY = transform.localRotation.eulerAngles.y;
        _moveAngleX = transform.localPosition.x;
        _moveAngleY = transform.localPosition.y;

        _defaultPosition = transform.localPosition;
    }

    protected virtual void InitializeStates()
    {
        _fsm.Add(new CameraThirdPerson(this));
        _fsm.Add(new CameraOrbital(this));
        _fsm.Add(new CameraFree(this));
    }

    void Update() => _fsm.Update();
    void FixedUpdate() => _fsm.FixedUpdate();
    void LateUpdate()
    {
        CalculateRotation(_look.x * _horizontalSensitivity, -_look.y * _verticalSensitivity);
        CalculateMovement(_move.x * _horizontalSensitivity, -_move.y * _verticalSensitivity);

        _fsm.LateUpdate();
    }

    #endregion

    void OnEnable()
    {
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandlePlayerDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);
        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandlePlayerRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);

        MyInputManager.Instance.Subscribe(EInputAction.LOOK, OnLook);
        MyInputManager.Instance.Subscribe(EInputAction.MOVE, OnMove);
    }

    void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);

        MyInputManager.Instance.Unsubscribe(EInputAction.LOOK, OnLook);
        MyInputManager.Instance.Unsubscribe(EInputAction.MOVE, OnMove);
    }

    private void HandlePlayerDeath()
    {
        _fsm.TransitionTo(ECameraMode.ORBITAL);
    }

    private void HandlePlayerRespawn()
    {
        _fsm.TransitionTo(ECameraMode.THIRD_PERSON);
    }

    private void OnLook(InputAction.CallbackContext context) => _look = context.ReadValue<Vector2>();
    private void OnMove(InputAction.CallbackContext context) => _move = context.ReadValue<Vector2>();

    private void CalculateRotation(float horizontalInput, float verticalInput)
    {
        if (_smoothCameraRotation)
        {
            horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * _cameraSmoothingFactor);
            verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * _cameraSmoothingFactor);
        }

        _lookAngleX += verticalInput * Time.deltaTime;
        _lookAngleY += horizontalInput * Time.deltaTime;

        _lookAngleX = Mathf.Clamp(_lookAngleX, -_upperVerticalLimit, _lowerVerticalLimit);
    }

    private void CalculateMovement(float horizontalInput, float verticalInput)
    {
        _moveAngleX += verticalInput * Time.deltaTime;
        _moveAngleY += horizontalInput * Time.deltaTime;
    }

    #region States

    public class CameraThirdPerson : State<ECameraMode>
    {
        readonly CameraController _camera;
        public CameraThirdPerson(CameraController camera) : base(ECameraMode.THIRD_PERSON) => _camera = camera;

        public override void LateUpdate()
        {
            base.LateUpdate();

            _camera.transform.localRotation = Quaternion.Euler(_camera._lookAngleX, 0, 0);
            _camera._player.transform.localRotation = Quaternion.Euler(0, _camera._lookAngleY, 0);
        }
    }

    public class CameraOrbital : State<ECameraMode>
    {
        readonly CameraController _camera;
        public CameraOrbital(CameraController camera) : base(ECameraMode.ORBITAL) => _camera = camera;

        public override void Enter()
        {
            base.Enter();

            ServiceLocator.Global.Get(out Animator playerAnimator);
            if (playerAnimator.isHuman)
            {
                Transform hipsTransform = playerAnimator.GetBoneTransform(HumanBodyBones.Hips);
                if (hipsTransform != null)
                {
                    _camera._followedTarget = hipsTransform;
                    _camera._initialLocalPosition = hipsTransform.InverseTransformPoint(_camera.transform.position);
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            _camera.transform.position = _camera._followedTarget.TransformPoint(_camera._initialLocalPosition);
            _camera.transform.localRotation = Quaternion.Euler(_camera._lookAngleX, _camera._lookAngleY, 0);
        }

        public override void Exit()
        {
            base.Exit();

            _camera.transform.localPosition = _camera._defaultPosition;
        }
    }

    public class CameraFree : State<ECameraMode>
    {
        readonly CameraController _camera;
        public CameraFree(CameraController camera) : base(ECameraMode.FREE) => _camera = camera;

        public override void LateUpdate()
        {
            base.LateUpdate();

            _camera.transform.localRotation = Quaternion.Euler(_camera._lookAngleX, _camera._lookAngleY, 0);
            _camera.transform.localPosition = new Vector3(_camera._moveAngleX, _camera._moveAngleY, 0);
        }
    }

    #endregion
}