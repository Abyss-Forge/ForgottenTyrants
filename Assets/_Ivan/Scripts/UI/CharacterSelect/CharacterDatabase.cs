using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Database", menuName = "Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private CharacterTemplate[] characters = new CharacterTemplate[0];

    public CharacterTemplate[] Characters => characters;

    public CharacterTemplate GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.ID == id)
            {
                return character;
            }
        }

        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return characters.Any(x => x.ID == id);
    }
}
