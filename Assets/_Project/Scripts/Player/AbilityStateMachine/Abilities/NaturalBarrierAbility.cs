using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public class NaturalBarrierAbility : AbilityStateMachine, IAbilityWithRange, IAbilityWithTarget
{
    #region Specific ability properties

    [SerializeField] private GameObject _barrierPrefab;
    [SerializeField] private PlaceablePreview _previewBarrierPrefab;
    [SerializeField] private float _maxPreviewDistance = 50f;

    private PlaceablePreview _previewBarrierInstance;
    private GameObject _barrierInstance;
    private Vector3? _propSpawnPosition;

    #endregion
    #region Interface implementation

    [SerializeField] float _range;
    public float Range => _range;

    private GameObject _target;
    public GameObject Target => _target;

    #endregion
    #region States

    protected override void UpdateState()
    {
        if (_fsm.CurrentState.ID == EAbilityState.READY)
        {
            _fsm.TransitionTo(EAbilityState.PREVIEW);
        }
        else if (_fsm.CurrentState.ID == EAbilityState.PREVIEW)
        {
            if (_previewBarrierInstance != null && _previewBarrierInstance.IsValid)
            {
                _fsm.TransitionTo(EAbilityState.ACTIVE);
            }
            else
            {
                _fsm.TransitionTo(EAbilityState.READY);
            }
        }
        else if (_fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            if (CanBeCanceled) ActiveTimer = 0;
        }
    }

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<NaturalBarrierAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewState(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<NaturalBarrierAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<NaturalBarrierAbility>(this, EAbilityState.LOCKED));
    }
    //TODO limpiar la guarrada que hay aqui montada
    private class AbilityPreviewState : AbilityPreviewBaseState<NaturalBarrierAbility>
    {
        public AbilityPreviewState(NaturalBarrierAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        public override void Enter()
        {
            SpawnProp();
        }

        public override void Update()
        {
            base.Update();

            TryGetTarget();
        }

        public override void Exit()
        {
            base.Exit();

            DespawnProp();
        }

        private void SpawnProp()
        {
            //_ability._previewBarrierInstance = Instantiate(_ability._previewBarrierPrefab.gameObject, _ability.transform, false);
            _ability._previewBarrierInstance = ExtensionMethods.InstantiateAndGet<PlaceablePreview>(_ability._previewBarrierPrefab.gameObject, _ability.transform);
            _ability._previewBarrierInstance.transform.SetParent(null);
            _ability._previewBarrierInstance.gameObject.Disable();
        }

        private void TryGetTarget()
        {
            _ability._propSpawnPosition = CrosshairRaycaster.GetImpactPosition(Layer.Mask.Terrain);

            if (_ability._propSpawnPosition == null || !_ability.transform.position.InRangeOf(_ability._propSpawnPosition.Value, _ability._maxPreviewDistance))
            {
                _ability._previewBarrierInstance.gameObject.Disable();
                return;
            }

            if (!_ability._previewBarrierInstance.gameObject.activeSelf) _ability._previewBarrierInstance.gameObject.Enable();

            bool isInRange = _ability.transform.position.InRangeOf(_ability._propSpawnPosition.Value, _ability.Range);
            _ability._previewBarrierInstance.SetValid(isInRange);

            UpdatePropPosition();
        }

        private void UpdatePropPosition()
        {
            Vector3 position = GetTargetPosition();
            Quaternion rotation = GetTargetRotation(position);

            _ability._previewBarrierInstance.transform.position = position;
            _ability._previewBarrierInstance.transform.rotation = rotation;
        }

        private Quaternion GetTargetRotation(Vector3 position)
        {
            if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hit, 5f, Layer.Mask.Terrain))
            {
                Debug.DrawRay(position + Vector3.up, Vector3.down * 5f, Color.green);

                Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Quaternion yRotation = Quaternion.Euler(0, _ability.SpawnPoint.transform.eulerAngles.y, 0);
                return surfaceRotation * yRotation;
            }

            return _ability.SpawnPoint.transform.rotation;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 targetPosition = _ability._propSpawnPosition.Value;
            Transform barrierTransform = _ability._barrierPrefab.transform;
            float halfHeight = barrierTransform.localScale.y / 2;
            targetPosition.y += halfHeight;
            return targetPosition;
        }

        private void DespawnProp()
        {
            if (_ability._previewBarrierInstance.gameObject != null) Destroy(_ability._previewBarrierInstance.gameObject);
        }
    }

    private class AbilityActiveState : AbilityActiveBaseState<NaturalBarrierAbility>
    {
        public AbilityActiveState(NaturalBarrierAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        public override void Enter()
        {
            base.Enter();

            SpawnProp();
        }

        public override void Exit()
        {
            base.Exit();

            _ability.StartCoroutine(AnimateBarrier(backwards: true));
        }

        private void SpawnProp()
        {
            Vector3 position = GetTargetPosition(true);
            Quaternion rotation = _ability.SpawnPoint.transform.rotation; //Quaternion.LookRotation(_propSpawnPosition.Value - position, Vector3.up);

            _ability._barrierInstance = Instantiate(_ability._barrierPrefab, position, rotation, _ability.transform);
            _ability._barrierInstance.transform.SetParent(null);
            _ability._barrierInstance.GetComponent<NetworkObject>().Spawn();
            _ability.StartCoroutine(AnimateBarrier());
        }

        private void DespawnProp()
        {
            if (_ability._barrierInstance != null) Destroy(_ability._barrierInstance);
        }

        private IEnumerator AnimateBarrier(bool backwards = false)
        {
            Vector3 targetPosition = GetTargetPosition(backwards);

            while (Vector3.Distance(_ability._barrierInstance.transform.position, targetPosition) > 0.1f)
            {
                float step = 5 * Time.deltaTime;
                _ability._barrierInstance.transform.position = Vector3.MoveTowards(_ability._barrierInstance.transform.position, targetPosition, step);
                yield return null;
            }

            if (backwards) DespawnProp();
            yield return null;
        }

        private Vector3 GetTargetPosition(bool backwards)
        {
            Vector3 targetPosition = _ability._propSpawnPosition.Value;

            Renderer renderer = _ability._barrierPrefab.GetComponent<Renderer>();
            float halfHeight = renderer.bounds.size.y / 2;

            if (backwards) halfHeight *= -1;

            targetPosition.y += halfHeight;
            return targetPosition;
        }
    }

    #endregion
}