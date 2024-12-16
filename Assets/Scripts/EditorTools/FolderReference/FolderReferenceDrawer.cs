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

        // Soporte para Drag-and-Drop
        HandleDragAndDrop(position, folderPathProp);

        EditorGUI.EndProperty();
    }

    private void HandleDragAndDrop(Rect dropArea, SerializedProperty folderPathProp)
    {
        Event evt = Event.current;

        // Detectamos si el mouse está dentro del área de drop
        if (!dropArea.Contains(evt.mousePosition)) return;


        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // Verificamos que lo que se arrastra sea una carpeta
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        string path = AssetDatabase.GetAssetPath(draggedObject);
                        if (AssetDatabase.IsValidFolder(path))
                        {
                            folderPathProp.stringValue = path; // Asignamos la ruta al campo
                            folderPathProp.serializedObject.ApplyModifiedProperties();
                            break;
                        }
                        else
                        {
                            Debug.LogWarning("El objeto arrastrado no es una carpeta válida.");
                        }
                    }
                }
                Event.current.Use();
                break;
        }
    }
}
