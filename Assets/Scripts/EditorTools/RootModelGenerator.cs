using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class RootModelGenerator : MonoBehaviour
{
    public Transform meshRoot;    // Transform del padre de los meshes
    public Transform colliderRoot; // Transform del padre de los colliders

    // Agrega el botón en el Inspector
    public void GenerateColliderStructure()
    {
        if (meshRoot == null || colliderRoot == null)
        {
            Debug.LogError("Por favor, selecciona ambos Transforms antes de generar.");
            return;
        }

        DuplicateStructure(meshRoot, colliderRoot);
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
        if (GUILayout.Button("Generate Collider Structure"))
        {
            generator.GenerateColliderStructure();
        }
    }
}