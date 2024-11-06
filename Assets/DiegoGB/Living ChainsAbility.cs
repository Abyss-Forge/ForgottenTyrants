using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingChainsAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private KeyCode activationKey = KeyCode.W;
    [SerializeField] float _cd;
    [SerializeField] float _range;
    [SerializeField] float _effectDuration;
    [SerializeField] float _animDuration;

    [Header("Effect Modifiers")]
    [SerializeField] float percentageMovementReduction = 25f;
    [SerializeField] float percentageDamageReduction = 20f;
    [SerializeField] float percentageMovementBoost = 25f;
    [SerializeField] float percentageDamageBoost = 15f;


    private float cooldownTimer = 0f;
    private bool abilityActive = false;

    void Start()
    {

    }

    void Update()
    {
        Cast();
        UpdateCooldownTimer();
    }

    IEnumerator EntityDetection()
    {
        abilityActive = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag(STags.Ally))
            {
                ApplyAllyEffect();
            }
            else if (hitCollider.gameObject.CompareTag(STags.Enemy))
            {
                ApplyEnemyEffect();
            }
        }
        yield return new WaitForSeconds(_effectDuration);

        abilityActive = false;
    }

    public void ApplyEnemyEffect()
    {
        Debug.Log("Enemy hit!");
    }

    public void ApplyAllyEffect()
    {
        Debug.Log("Ally hit!");
    }

    public void Cast()
    {
        if (cooldownTimer <= 0f && Input.GetKeyDown(activationKey) && !abilityActive)
        {
            Debug.Log("execute ability");
            StartCoroutine(EntityDetection());
            cooldownTimer = _cd;
        }
    }

    public void UpdateCooldownTimer()
    {
        if (cooldownTimer > 0 && !abilityActive) cooldownTimer -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (abilityActive)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }
}
