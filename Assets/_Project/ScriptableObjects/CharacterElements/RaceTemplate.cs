using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterElement/Race")]
public class RaceTemplate : CharacterElementTemplate
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Animator ModelRoot { get; private set; }

}