using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crosshair", menuName = "ScriptableObject/Crosshair", order = 1)]
public class Crosshair : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _size;

    public Sprite Sprite => _sprite;    //readonly
    public int Size => _size;
}
