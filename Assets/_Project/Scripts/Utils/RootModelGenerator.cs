#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class RootModelGenerator : MonoBehaviour
{
    public Transform _meshRoot, _colliderRoot;

    public void GenerateColliderStructure()
    {
        if (_meshRoot == null || _colliderRoot == null)
        {
            Debug.LogError("Por favor, selecciona ambos Transforms antes de generar.");
            return;
        }

        DuplicateStructure(_meshRoot, _colliderRoot);
        Debug.Log("Estructura de colliders generada exitosamente.");
    }

    private void DuplicateStructure(Transform source, Transform targetParent)
    {
        foreach (Transform child in source)
        {
            // Intentar encontrar un objeto con el mismo nombre en el targetParent
            Transform existingChild = targetParent.Find(child.name);
            GameObject colliderObject;

            if (existingChild != null)
            {
                colliderObject = existingChild.gameObject;
            }
            else
            {
                // Si no existe, crear un nuevo GameObject en la estructura de colliders
                colliderObject = new GameObject(child.name);
                colliderObject.transform.SetParent(targetParent);
                colliderObject.transform.localPosition = child.localPosition;
                colliderObject.transform.localRotation = child.localRotation;
                colliderObject.transform.localScale = child.localScale;
            }

            // Si el hijo tiene un Collider y el objeto equivalente en colliders aún no tiene uno, lo mueve al nuevo objeto
            if (child.TryGetComponent(out Collider originalCollider) && !colliderObject.GetComponent<Collider>())
            {
                Collider newCollider = CopyCollider(originalCollider, colliderObject);
                DestroyImmediate(originalCollider);
            }
            else if (!colliderObject.GetComponent<Collider>()) // Si no hay Collider, añadir un MeshCollider
            {
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    MeshCollider meshCollider = colliderObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                    meshCollider.convex = false; // Opcional: cambia según necesidad
                    colliderObject.transform.localPosition = Vector3.zero;
                }
                else if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                {
                    MeshCollider meshCollider = colliderObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
                    meshCollider.convex = false; // Opcional: cambia según necesidad
                    colliderObject.transform.localPosition = Vector3.zero;
                }
            }

            // Verificar si el objeto en la estructura de meshes tiene un Collider que no necesita
            RemoveRedundantCollider(child, colliderObject);

            // Recursión: duplicar los nietos y demás descendientes en la estructura de colliders
            DuplicateStructure(child, colliderObject.transform);
        }
    }

    private void RemoveRedundantCollider(Transform meshObject, GameObject colliderObject)
    {
        // Verifica si el meshObject tiene un collider y si el colliderObject tiene ya uno equivalente
        if (colliderObject.GetComponent<Collider>() && meshObject.GetComponent<Collider>())
        {
            DestroyImmediate(meshObject.GetComponent<Collider>());
        }
    }

    private Collider CopyCollider(Collider originalCollider, GameObject targetObject)
    {
        System.Type colliderType = originalCollider.GetType();
        Collider newCollider = (Collider)targetObject.AddComponent(colliderType);

        // Copiar propiedades específicas del tipo de collider
        if (colliderType == typeof(BoxCollider))
        {
            BoxCollider originalBox = (BoxCollider)originalCollider;
            BoxCollider newBox = (BoxCollider)newCollider;
            newBox.center = originalBox.center;
            newBox.size = originalBox.size;
        }
        else if (colliderType == typeof(SphereCollider))
        {
            SphereCollider originalSphere = (SphereCollider)originalCollider;
            SphereCollider newSphere = (SphereCollider)newCollider;
            newSphere.center = originalSphere.center;
            newSphere.radius = originalSphere.radius;
        }
        else if (colliderType == typeof(CapsuleCollider))
        {
            CapsuleCollider originalCapsule = (CapsuleCollider)originalCollider;
            CapsuleCollider newCapsule = (CapsuleCollider)newCollider;
            newCapsule.center = originalCapsule.center;
            newCapsule.radius = originalCapsule.radius;
            newCapsule.height = originalCapsule.height;
            newCapsule.direction = originalCapsule.direction;
        }
        else if (colliderType == typeof(MeshCollider))
        {
            MeshCollider originalMesh = (MeshCollider)originalCollider;
            MeshCollider newMesh = (MeshCollider)newCollider;
            newMesh.sharedMesh = originalMesh.sharedMesh;
            newMesh.convex = originalMesh.convex;

        }

        return newCollider;
    }
}


// Clase Editor personalizada para agregar un botón en el Inspector
[CustomEditor(typeof(RootModelGenerator))]
public class RootModelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RootModelGenerator generator = (RootModelGenerator)target;

        // Deshabilitar el botón si _colliderRoot es nulo
        GUI.enabled = generator._colliderRoot != null && generator._meshRoot;

        if (GUILayout.Button("Generate Collider Structure"))
        {
            generator.GenerateColliderStructure();
        }

        // Restaurar el estado predeterminado de GUI.enabled
        GUI.enabled = true;
    }
}

#endif