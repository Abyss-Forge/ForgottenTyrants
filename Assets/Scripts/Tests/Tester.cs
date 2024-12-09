using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using JetBrains.Annotations;
using UnityEditor;

[AttributeUsage(AttributeTargets.Field)]
public sealed class TagAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(TagAttribute))]
internal sealed class TagAttributePropertyDrawer : PropertyDrawer
{
    /// <inheritdoc />
    public override void OnGUI(Rect position, [NotNull] SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.String)
            EditorGUI.PropertyField(position, property, label);
        else
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);

        EditorGUI.EndProperty();
    }
}

public class Tester : MonoBehaviour
{
    [Tag, SerializeField, RequiredField] string _tag;
    [RequiredField, SerializeField] GameObject _bomb;
    [RequiredField, SerializeField] Transform _spawnPoint;
    [RequiredField, SerializeField] float _force = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject instance = Instantiate(_bomb, _spawnPoint.position, quaternion.identity, transform);
            instance.transform.SetParent(null); // esto es para que spawnee en la misma escena 

            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f;
            Vector3 adjustedDirection = (targetPoint - _spawnPoint.position).normalized;
            adjustedDirection.y += 0.5f;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.AddForce(adjustedDirection * _force * rb.mass, ForceMode.Impulse);
        }
    }
}