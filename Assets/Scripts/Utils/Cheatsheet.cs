#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheatsheet : MonoBehaviour
{
    //When it has accessors { get; set; }, it's a property, when not, it's a field. Attributes are what goes in square brackets [], like SerializeField.

    private int _fieldA; //full private

    public int _propertyB { get; private set; } //readonly

    [SerializeField] private int _fieldC;    //inspector serializable readonly
    public int PropertyC => _fieldC;

    [field: SerializeField] public int _propertyD { get; private set; } //inspector serializable readonly BEST PRACTICE

    public int PropertyE { get; set; }  //full public

    private int _fieldF;    //full public BEST PRACTICE
    public int PropertyF { get => _fieldF; set { _fieldF = value; OnValueChanged(); } }

    private void OnValueChanged()
    {
        //Do something
    }

    void OnValidate()
    {

    }

    void Awake()
    {

    }

    void OnEnable()
    {

    }

    void Start()
    {

    }

    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {

    }

    void OnDrawGizmos()
    {

    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }

}

#endif