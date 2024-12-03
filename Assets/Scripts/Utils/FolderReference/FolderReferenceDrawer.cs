using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FolderReference))]
public class FolderReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Obtenemos la propiedad interna del string de la carpeta
        SerializedProperty folderPathProp = property.FindPropertyRelative("folderPath");

        // Dividimos el espacio horizontalmente para el campo de texto y el botón
        Rect textFieldRect = new Rect(position.x, position.y, position.width - 25, position.height);
        Rect buttonRect = new Rect(position.x + position.width - 25, position.y, 25, position.height);

        // Dibujamos el campo de texto para mostrar la ruta actual
        EditorGUI.PropertyField(textFieldRect, folderPathProp, label);

        // Dibujamos el botón para abrir el cuadro de diálogo
        if (GUI.Button(buttonRect, "Browse"))
        {
            // Llamamos a la función SelectFolder() de la clase FolderReference
            FolderReference folderRef = fieldInfo.GetValue(property.serializedObject.targetObject) as FolderReference;
            folderRef?.SelectFolder();

            // Aplicamos cambios al objeto serializado
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}