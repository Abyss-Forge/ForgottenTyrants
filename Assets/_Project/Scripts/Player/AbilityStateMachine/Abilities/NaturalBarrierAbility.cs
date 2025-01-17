using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;

public class NaturalBarrierAbility : AbilityStateMachine, IAbilityWithRange, IAbilityWithTarget
{
    #region Specific ability properties

    [SerializeField] private GameObject _barrierPrefab, _previewBarrierPrefab;
    private GameObject _barrierInstance, _impactObject;
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
            _fsm.TransitionTo(EAbilityState.ACTIVE);
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
            Vector3 position = _ability._propSpawnPosition ?? Vector3.zero;
            Quaternion rotation = _ability.SpawnPoint.transform.rotation;

            _ability._barrierInstance = Instantiate(_ability._previewBarrierPrefab, position, rotation, _ability.transform);
            _ability._barrierInstance.transform.SetParent(null);
        }

        private void TryGetTarget()
        {
            _ability._impactObject = CrosshairRaycaster.GetImpactObject();
            _ability._propSpawnPosition = CrosshairRaycaster.GetImpactPosition();

            if (_ability._impactObject == null || _ability._propSpawnPosition == null)
            {
                _ability._barrierInstance.SetActive(false);
                return;
            }

            if (_ability._impactObject.layer == Layer.Terrain && CheckIfInRange())
            {
                if (!_ability._barrierInstance.activeSelf) _ability._barrierInstance.SetActive(true);
                UpdatePropPosition();
            }
            else
            {
                _ability._barrierInstance.SetActive(false);
            }
        }

        private bool CheckIfInRange()
        {
            float distance = Vector3.Distance(_ability.transform.position, _ability._propSpawnPosition.Value);
            return _ability.Range <= distance;
        }

        private void UpdatePropPosition()
        {
            Vector3 position = _ability._propSpawnPosition ?? Vector3.zero;
            Quaternion rotation = _ability.SpawnPoint.transform.rotation;

            _ability._barrierInstance.transform.position = position;
            _ability._barrierInstance.transform.rotation = rotation;
        }

        private void DespawnProp()
        {
            if (_ability._barrierInstance != null) Destroy(_ability._barrierInstance);
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

            TryGetTarget();
        }

        public override void Exit()
        {
            base.Exit();

            _ability.StartCoroutine(AnimateBarrier(true));
        }

        private void TryGetTarget()
        {
            _ability._impactObject = CrosshairRaycaster.GetImpactObject();
            _ability._propSpawnPosition = CrosshairRaycaster.GetImpactPosition();

            if (_ability._impactObject == null || _ability._propSpawnPosition == null)
            {
                _ability.FSM.TransitionTo(EAbilityState.COOLDOWN);
                return;
            }

            if (_ability._impactObject.layer == Layer.Terrain && CheckIfInRange()) SpawnProp();
            //  if (_ability._impactObject.layer != Layer.Terrain && CheckIfInRange()) SpawnProp();
        }

        private bool CheckIfInRange()
        {
            float distance = Vector3.Distance(_ability.transform.position, _ability._propSpawnPosition.Value);
            return _ability.Range <= distance;
        }

        private void SpawnProp()
        {
            Vector3 position = _ability._propSpawnPosition ?? Vector3.zero;
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