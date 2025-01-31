using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private CharacterTemplate[] _characters = new CharacterTemplate[0];
    public CharacterTemplate[] Characters => _characters;

    public CharacterTemplate GetById(int id)
    {
        return _characters.FirstOrDefault(element => element.ID == id);
    }

    public bool IsValidId(int id)
    {
        return _characters.Any(x => x.ID == id);
    }

}