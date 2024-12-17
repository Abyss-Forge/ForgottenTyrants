using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Crosshair")]
public class Crosshair : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _size;

    public Sprite Sprite => _sprite;    //readonly
    public int Size => _size;
}
