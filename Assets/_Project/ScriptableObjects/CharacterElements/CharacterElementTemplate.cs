using UnityEngine;

public abstract class CharacterElementTemplate : ScriptableObject
{
    [field: SerializeField] public string UID { get; protected set; }
    [field: SerializeField] public string DisplayName { get; protected set; }
    [field: SerializeField] public Stats Stats { get; protected set; }

}