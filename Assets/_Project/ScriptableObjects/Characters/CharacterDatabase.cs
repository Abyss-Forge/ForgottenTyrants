using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private CharacterTemplate[] _characters = new CharacterTemplate[0];
    public CharacterTemplate[] Characters => _characters;

    public CharacterTemplate GetById(int id)
    {
        foreach (var character in _characters)
        {
            if (character.ID == id)
            {
                return character;
            }
        }

        return null;
    }

    public bool IsValidId(int id)
    {
        return _characters.Any(x => x.ID == id);
    }
}
