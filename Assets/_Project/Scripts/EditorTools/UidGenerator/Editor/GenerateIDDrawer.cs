using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenerateIDAttribute))]
public class GenerateIDDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer || property.type != "ulong")
        {
            EditorGUI.LabelField(position, label.text, "Use [GenerateID] with ulong only.");
            return;
        }

        Rect fieldRect = new Rect(position.x, position.y, position.width - 30, position.height);
        Rect buttonRect = new Rect(position.x + position.width - 25, position.y, 25, position.height);

        EditorGUI.PropertyField(fieldRect, property, label);
        if (GUI.Button(buttonRect, "⟳"))
        {
            property.longValue = (long)Random.Range(0f, long.MaxValue);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}