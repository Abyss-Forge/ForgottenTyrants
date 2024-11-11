using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.InputSystem;

public class LivingChainsAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] float _cd;
    [SerializeField] float _range;
    [SerializeField] float _effectDuration;
    [SerializeField] float _animDuration;

    [SerializeField] Material _chainMaterial;

    [Header("Effect Modifiers")]
    [SerializeField] float percentageMovementReduction = 25f;
    [SerializeField] float percentageDamageReduction = 20f;
    [SerializeField] float percentageMovementBoost = 25f;
    [SerializeField] float percentageDamageBoost = 15f;

    Dictionary<GameObject, GameObject> _playerChains;
    private float _cooldownTimer = 0f;
    private bool _isAbilityActive = false;

    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    void Start()
    {
        _playerChains = new Dictionary<GameObject, GameObject>();
        MyInputManager.Instance.SubscribeToInput(EInputActions.ClassAbility1, OnCast, true);
    }

    void Update()
    {
        UpdateCooldownTimer();
        UpdateChains();
    }

    void OnDrawGizmos()
    {
        if (_isAbilityActive)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }

    private IEnumerator EntityDetection()
    {
        _isAbilityActive = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag(Tag.Ally))
            {
                ApplyAllyEffect();
            }
            else if (hitCollider.gameObject.CompareTag(Tag.Enemy))
            {
                ApplyEnemyEffect(hitCollider.gameObject);
            }
        }
        yield return new WaitForSeconds(_effectDuration);

        _isAbilityActive = false;
    }

    public void ApplyEnemyEffect(GameObject enemy)
    {
        if (!_playerChains.ContainsKey(enemy))
        {
            GameObject emptyObject = new GameObject("Chain");
            GameObject chain = Instantiate(emptyObject, Vector3.zero, Quaternion.identity, transform);
            LineRenderer lr = chain.AddComponent<LineRenderer>();

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, enemy.transform.position);

            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.startColor = Color.white;
            lr.endColor = Color.black;
            lr.material = _chainMaterial;
            //_chainMaterial.proper

            _playerChains.Add(enemy, chain);
        }
        Debug.Log("Enemy hit!");
    }


    void UpdateChains()
    {
        if (_isAbilityActive)
        {
            foreach (var item in _playerChains)
            {
                LineRenderer lineRenderer = item.Value.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, item.Key.transform.position);
            }
        }
        else if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            foreach (var item in _playerChains)
            {
                Destroy(item.Value.gameObject);
            }
        }
    }

    public void ApplyAllyEffect()
    {
        Debug.Log("Ally hit!");
    }

    public void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            Debug.Log("execute ability");
            StartCoroutine(EntityDetection());
            _cooldownTimer = _cd;
        }
    }

    public void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }


}
