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
        /// Copies a component from one GameObject to another.
        /// </summary>
        /// <typeparam name="T">The type of the component to copy.</typeparam>
        /// <param name="destination">The GameObject to which the component will be copied.</param>
        /// <param name="originalComponent">The original component to copy.</param>
        /// <returns>The copied component attached to the destination GameObject.</returns>
        public static T CopyComponent<T>(this GameObject destination, T originalComponent) where T : Component
        {
            Type componentType = originalComponent.GetType();
            Component copy = destination.AddComponent(componentType);

            FieldInfo[] fields = componentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(originalComponent));
            }

            PropertyInfo[] properties = componentType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite && property.GetSetMethod(true) != null)
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(originalComponent));
                    }
                    catch { }
                }
            }

            return copy as T;
        }

    }
}