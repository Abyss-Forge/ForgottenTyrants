using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangoAttribute))]
public class RangoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RangoAttribute rango = (RangoAttribute)attribute;

        // Verificar que el atributo se use en listas o arrays
        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            int serializedCount = GetSerializedElementCount(property);
            bool isOutOfRange = serializedCount < rango.Min || serializedCount > rango.Max;

            // Guardar el color original
            Color defaultColor = GUI.color;

            // Cambiar el color a rojo si está fuera del rango
            if (isOutOfRange)
            {
                GUI.color = Color.red;
            }

            // Dibujar el array o lista en el Inspector
            EditorGUI.PropertyField(position, property, label, true);

            // Restaurar el color original
            GUI.color = defaultColor;

            // Mostrar un mensaje de advertencia si está fuera del rango
            if (isOutOfRange)
            {
                Rect helpBoxRect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(property, true) + 2, position.width, EditorGUIUtility.singleLineHeight * 2);
                EditorGUI.HelpBox(helpBoxRect, $"El tamaño debe ser entre {rango.Min} y {rango.Max} objetos serializados (no nulos).", MessageType.Warning);
            }
        }
        else
        {
            // Mostrar un mensaje de error si el atributo no se usa sobre listas o arrays
            EditorGUI.LabelField(position, label.text, "Use Rango con listas o arrays serializados.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        RangoAttribute rango = (RangoAttribute)attribute;

        // Calcular altura adicional para el mensaje de advertencia
        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            int serializedCount = GetSerializedElementCount(property);
            bool isOutOfRange = serializedCount < rango.Min || serializedCount > rango.Max;
            return base.GetPropertyHeight(property, label) + (isOutOfRange ? EditorGUIUtility.singleLineHeight * 2 + 4 : 0);
        }

        return base.GetPropertyHeight(property, label);
    }

    private int GetSerializedElementCount(SerializedProperty property)
    {
        int count = 0;

        // Recorrer los elementos del array y contar los serializados (no nulos)
        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);

            // Verificar si el elemento está serializado y no es nulo
            if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue != null)
            {
                count++;
            }
        }

        return count;
    }
}
