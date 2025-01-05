using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterElement/Database")]
public abstract class CharacterElementDatabase<T> : ScriptableObject where T : CharacterElementTemplate
{
    [SerializeField] protected T[] _elements = new T[0];
    public T[] Elements => _elements;

    public T GetById(string id)
    {
        return _elements.FirstOrDefault(element => element.UID == id);
    }

    public bool IsValidId(string id)
    {
        return _elements.Any(element => element.UID == id);
    }

}