using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Characters/Character")]
public class CharacterTemplate : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; } = -1;
    [field: SerializeField] public string DisplayName { get; private set; } = "New Display Name";
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject IntroPrefab { get; private set; }
    [field: SerializeField] public GameObject ModelRoot { get; private set; }
    [field: SerializeField] public PlayerRef PlayerRef { get; private set; }

}