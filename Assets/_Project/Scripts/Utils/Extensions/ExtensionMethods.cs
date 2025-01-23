using System;
using UnityEngine;
using System.Reflection;

namespace Utils.Extensions
{
    public static class ExtensionMethods
    {

        public static bool IsInRange<T>(this T[] array, int index) => index >= 0 && index < array.Length;

        public static void Enable(this GameObject gameObject) => gameObject.SetActive(true);
        public static void Disable(this GameObject gameObject) => gameObject.SetActive(false);

        public static void Unparent(this Transform transform) => transform.SetParent(null);

        public static void AlignToGround(this Transform transform)
        {
            Vector3 position = transform.position;
            float halfHeight = transform.localScale.y / 2;
            position.y = halfHeight;
            transform.position = position;
        }

        /// <summary>
        /// Instantiates a prefab at the specified position and rotation, optionally under a parent transform,
        /// and returns the component of type <typeparamref name="T"/> attached to the instantiated GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve from the instantiated GameObject.</typeparam>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="position">The position to place the instantiated GameObject. Defaults to <see cref="Vector3.zero"/>.</param>
        /// <param name="rotation">The rotation to apply to the instantiated GameObject. Defaults to <see cref="Quaternion.identity"/>.</param>
        /// <param name="parent">The parent transform to set for the instantiated GameObject. Defaults to null.</param>
        /// <param name="autoUnparent">If true, the instantiated GameObject will be unparented immediately after instantiation. Defaults to false.</param>
        /// <returns>The component of type <typeparamref name="T"/> attached to the instantiated GameObject, or null if no such component is found.</returns>
        public static T GetInstantiate<T>
        (
            GameObject prefab,
            Vector3 position = default,
            Quaternion rotation = default,
            Transform parent = null,
            bool autoUnparent = false
        )
        where T : MonoBehaviour
        {
            GameObject instance = MonoBehaviour.Instantiate(prefab, position, rotation, parent);
            if (autoUnparent) instance.transform.SetParent(null);
            return instance?.GetComponent<T>() ?? null;
        }

        public static T GetInstantiate<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
        {
            GameObject instance = MonoBehaviour.Instantiate(prefab, parent, false);
            return instance?.GetComponent<T>() ?? null;
        }


        /// <summary>
        /// Deletes the specified component from the target GameObject. If no component is specified,
        /// it will attempt to find and delete the component of type T from the target GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to delete.</typeparam>
        /// <param name="target">The GameObject from which the component will be deleted.</param>
        /// <param name="component">The specific component to delete. If null, the method will find and delete the component of type T.</param>
        public static void DeleteComponent<T>(this GameObject target, T component = null) where T : Component
        {
            component ??= target.GetComponent<T>();
            if (component != null) UnityEngine.Object.Destroy(component);
        }

        /// <summary>
        /// Moves a component of type <typeparamref name="T"/> from the target GameObject to the origin GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to move.</typeparam>
        /// <param name="target">The GameObject to which the component will be moved.</param>
        /// <param name="origin">The GameObject from which the component will be moved.</param>
        /// <param name="component">Optional. The specific component instance to move. If not provided, the method will attempt to get the component from the target GameObject.</param>
        /// <returns>The moved component of type <typeparamref name="T"/> if the operation is successful; otherwise, null.</returns>
        public static T MoveComponent<T>(this GameObject target, GameObject origin, T component = null) where T : Component
        {
            component ??= origin.GetComponent<T>();
            if (component == null) return null;

            T copy = target.CopyComponent<T>(component);
            target.DeleteComponent<T>(component);
            return copy;
        }

        /// <summary>
        /// Copies a component from one GameObject to another.
        /// </summary>
        /// <typeparam name="T">The type of the component to copy.</typeparam>
        /// <param name="destination">The GameObject to which the component will be copied.</param>
        /// <param name="component">The original component to copy.</param>
        /// <returns>The copied component attached to the destination GameObject.</returns>
        public static T CopyComponent<T>(this GameObject destination, T component = null) where T : Component
        {
            //TODO: if component is null, get the first component matching T type
            Type componentType = component.GetType();
            Component copy = destination.AddComponent(componentType);

            FieldInfo[] fields = componentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(component));
            }

            PropertyInfo[] properties = componentType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite && property.GetSetMethod(true) != null)
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(component));
                    }
                    catch { }
                }
            }

            return copy as T;
        }

    }
}