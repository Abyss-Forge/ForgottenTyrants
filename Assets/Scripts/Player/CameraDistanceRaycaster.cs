using ForgottenTyrants;
using UnityEngine;

namespace AdvancedController
{
    public class CameraDistanceRaycaster : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTargetTransform, _cameraTransform;

        [SerializeField] private LayerMask _layerMask = Physics.AllLayers;
        [SerializeField] private float _minimumDistanceFromObstacles = 0.1f, _smoothingFactor = 25f;

        private Transform _tr;
        private float _currentDistance;

        void Awake()
        {
            _tr = transform;

            _layerMask &= ~(1 << Layer.Mask.IgnoreRaycast);
            _currentDistance = (_cameraTargetTransform.position - _tr.position).magnitude;
        }

        void LateUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            Vector3 castDirection = _cameraTargetTransform.position - _tr.position;

            float distance = GetCameraDistance(castDirection);

            _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * _smoothingFactor);
            _cameraTransform.position = _tr.position + castDirection.normalized * _currentDistance;
        }

        private float GetCameraDistance(Vector3 castDirection)
        {
            float distance = castDirection.magnitude + _minimumDistanceFromObstacles;
            // if (Physics.Raycast(new Ray(tr.position, castDirection), out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore)) {
            //     return Mathf.Max(0f, hit.distance - minimumDistanceFromObstacles);
            // }
            float sphereRadius = 0.5f;
            if (Physics.SphereCast(new Ray(_tr.position, castDirection), sphereRadius, out RaycastHit hit, distance, _layerMask, QueryTriggerInteraction.Ignore))
            {
                return Mathf.Max(0f, hit.distance - _minimumDistanceFromObstacles);
            }
            return castDirection.magnitude;
        }
    }

}