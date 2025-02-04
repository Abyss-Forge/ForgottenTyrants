using UnityEngine;

public abstract class CharacterElementTemplate : ScriptableObject
{
    [field: SerializeField, GenerateID] public ulong UID { get; protected set; }
    [field: SerializeField] public string DisplayName { get; protected set; }
    [field: SerializeField] public Stats Stats { get; protected set; }

}