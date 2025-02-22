using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdjacentShadowAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private float _cooldownDuration = 5, _animationDuration = 2;

    [Header("Effect Modifiers")]
    [SerializeField] private float _damageDealt = 30f;
    [SerializeField] private float _distanceBehind = 10f;
    [SerializeField] private float _range = 20f;
    float _cooldownTimer = 0f;
    bool _isAbilityActive = false;
    GameObject enemy;

    void Start()
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_2, OnCast, true);
    }

    void Update()
    {
        UpdateCooldownTimer();
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cast();
        }
    }

    // Callback para la acción de input asignada a la habilidad
    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    // Inicia la ejecución de la habilidad si no está en cooldown ni activa
    public void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            StartCoroutine(CastAdjacentShadow());
            _cooldownTimer = _cooldownDuration;
        }
    }

    private IEnumerator CastAdjacentShadow()
    {
        // Obtiene el objeto impactado por el rayo del crosshair
        enemy = CrosshairRaycaster.GetImpactObject();

        // Verifica que se haya impactado un objeto, que tenga la etiqueta "Enemy" y esté dentro del rango
        if (enemy.CompareTag(Tag.Enemy) && CalculateIsInRange())
        {
            _isAbilityActive = true;
            TeleportToEnemy();
            yield return new WaitForSeconds(_animationDuration);
            DealDamage();
            enemy = null;
            _isAbilityActive = false;
        }
        else Debug.LogWarning("Enemy not detected or too far distance");

    }

    // Calcula si el enemigo está dentro del rango permitido
    private bool CalculateIsInRange()
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        return _range >= distanceToEnemy;
    }

    private void TeleportToEnemy()
    {
        // Calcula la posición detrás del enemigo
        Vector3 positionBehind = enemy.transform.position - enemy.transform.forward * _distanceBehind;

        // Desactiva temporalmente el CharacterController para evitar conflictos al cambiar la posición
        GetComponent<CharacterController>().enabled = false;
        transform.position = positionBehind;
        GetComponent<CharacterController>().enabled = true;

        // Reinicia la velocidad del jugador y alinea la rotación con la del enemigo
        GetComponent<PlayerController>().SetVelocity(Vector3.zero);
        transform.rotation = enemy.transform.rotation;
    }

    private void DealDamage()
    {
        Debug.Log("Damage dealt: " + _damageDealt);
    }

    // Actualiza el temporizador de enfriamiento reduciéndolo con el tiempo transcurrido
    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new(2, 1, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, _range);
    }
}
