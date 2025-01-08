using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Ability")]
public class AbilityTemplate : ScriptableObject
{
    [SerializeField, Tooltip("In principle, same prefab for every ability, edit the sprites below")] AbilityIcon _iconPrefab;
    public AbilityIcon IconPrefab => _iconPrefab;

    [SerializeField] Sprite _iconSprite, _backgroundSprite, _borderSprite;

    [SerializeField, Tooltip("Different prefab for each ability")] AbilityStateMachine _abilityPrefab;
    public AbilityStateMachine AbilityPrefab => _abilityPrefab;

    void Awake()
    {
        _iconPrefab.InitializeSprites(_iconSprite, _backgroundSprite, _borderSprite);
    }
}