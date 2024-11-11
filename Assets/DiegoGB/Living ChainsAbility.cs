using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ForgottenTyrants;

public class LivingChainsAbility : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRendererTemplate;

    [Header("Ability Settings")]
    [SerializeField] private float _range = 10;
    [SerializeField] private float _cooldownDuration = 5, _effectDuration = 5, _animDuration = 2;

    [Header("Effect Modifiers")]
    [SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float percentageDamageReduction = 20f, percentageMovementBoost = 25f, percentageDamageBoost = 15f;

    private Dictionary<GameObject, GameObject> _playerChains = new();
    private float _cooldownTimer = 0f;
    private bool _isAbilityActive = false;

    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    void Start()
    {
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
            Gizmos.color = new(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }

    private void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            StartCoroutine(EntityDetection());
            _cooldownTimer = _cooldownDuration;
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
        ResetChains();
    }

    private void ApplyAllyEffect()
    {
        Debug.Log("Ally hit!");
    }

    private void ApplyEnemyEffect(GameObject enemy)
    {
        if (!_playerChains.ContainsKey(enemy))
        {
            GameObject chain = new();
            chain.transform.SetParent(transform);
            LineRenderer lr = chain.CopyComponent(_lineRendererTemplate);
            chain.name = "Chain";

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, enemy.transform.position);
            //StartCoroutine(AnimateChain(lr, enemy.transform.position));

            _playerChains.Add(enemy, chain);
        }
    }

    private IEnumerator AnimateChain(LineRenderer lr, Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        float animationDuration = 2f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);
            lr.SetPosition(1, Vector3.Lerp(lr.GetPosition(0), targetPosition, t));

            yield return null;
        }

        lr.SetPosition(1, targetPosition);
        //Destroy(lr.gameObject);
    }

    private void ResetChains()
    {
        foreach (var item in _playerChains)
        {
            StartCoroutine(AnimateChain(item.Value.GetComponent<LineRenderer>(), transform.position));
        }
        _playerChains.Clear();
    }

    private void UpdateChains()
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
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }

}