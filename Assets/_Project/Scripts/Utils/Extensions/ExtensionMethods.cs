using System;
using System.Reflection;
using UnityEngine;

namespace Utils.Extensions
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Checks if the specified index is within the bounds of the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to check.</param>
        /// <param name="index">The index to check.</param>
        /// <returns>True if the index is within the bounds of the array; otherwise, false.</returns>
        public static bool IsInRange<T>(this T[] array, int index) => index >= 0 && index < array.Length;

        public static void SwapTuple<T>(ref T value1, ref T value2) where T : struct => (value1, value2) = (value2, value1);
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void Enable(this GameObject go) => go.SetActive(true);
        public static void Disable(this GameObject go) => go.SetActive(false);
        public static void Destroy(this GameObject go) => MonoBehaviour.Destroy(go);
        public static void Unparent(this Transform tr) => tr.SetParent(null);
        public static bool CompareLayer(this GameObject go, int layer) => go.layer == layer;

        public static void AlignToGround(this Transform tr)
        {
            Vector3 position = tr.position;
            float halfHeight = tr.localScale.y / 2;
            position.y = halfHeight;
            tr.position = position;
        }

        public static void CopyTransform(this Transform target, Transform source)
        {
            target.position = source.position;
            target.rotation = source.rotation;
            target.localScale = source.localScale;
        }

        /// <summary>
        /// Instantiates a prefab at the specified position and rotation, optionally under a parent transform,
        /// and returns the component of type <typeparamref name="T"/> attached to the instantiated GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve from the instantiated GameObject.</typeparam>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="position">The position to place the instantiated GameObject. Defaults to <see cref="Vector3.zero"/>.</param>
        /// <param name="rotation">The rotation to apply to the instantiated GameObject. Defaults to <see cref="Quaternion.identity"/>.</param>
        /// <param name="scale">The scale to apply to the instantiated GameObject. Defaults to the prefab scale.</param>
        /// <param name="parent">The parent transform to set for the instantiated GameObject. Defaults to null.</param>
        /// <param name="autoUnparent">If true, the instantiated GameObject will be unparented immediately after instantiation. Defaults to false.</param>
        /// <returns>The component of type <typeparamref name="T"/> attached to the instantiated GameObject, or null if no such component is found.</returns>
        public static T InstantiateAndGet<T>
        (
            GameObject prefab,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3? scale = null,
            Transform parent = null,
            bool autoUnparent = false
        )
        where T : MonoBehaviour
        {
            GameObject instance = MonoBehaviour.Instantiate(prefab, position, rotation, parent);
            if (scale != null) instance.transform.localScale = scale ?? Vector3.one;
            if (autoUnparent) instance.transform.SetParent(null);
            return instance?.GetComponent<T>() ?? null;
        }

        public static T InstantiateAndGet<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
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
        public static void RemoveComponent<T>(this GameObject target, T component = null) where T : Component
        {
            component ??= target.GetComponent<T>();
            if (component != null) UnityEngine.Object.Destroy(component);
        }


        /// <summary>
        /// Moves a component of type <typeparamref name="T"/> from the target GameObject to the origin GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to move.</typeparam>
        /// <param name="destination">The GameObject to which the component will be moved.</param>
        /// <param name="source">The GameObject from which the component will be moved.</param>

        /// <returns>The moved component of type <typeparamref name="T"/> if the operation is successful; otherwise, null.</returns>
        public static T MoveComponent<T>(this GameObject destination, GameObject source) where T : Component => destination.MoveComponent<T>(source.GetComponent<T>());
        /// <summary>
        /// Moves a component of type <typeparamref name="T"/> from the target GameObject to the origin GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to move.</typeparam>
        /// <param name="destination">The GameObject to which the component will be moved.</param>
        /// <param name="component">Optional. The specific component instance to move. If not provided, the method will attempt to get the component from the target GameObject.</param>
        /// <returns>The moved component of type <typeparamref name="T"/> if the operation is successful; otherwise, null.</returns>
        public static T MoveComponent<T>(this GameObject destination, T component) where T : Component
        {
            if (component == null) return null;

            T copy = destination.CopyComponent<T>(component);
            component.gameObject.RemoveComponent<T>(component);
            return copy;
        }


        /// <summary>
        /// Copies a component from one GameObject to another.
        /// </summary>
        /// <typeparam name="T">The type of the component to copy.</typeparam>
        /// <param name="destination">The GameObject to which the component will be copied.</param>
        /// <param name="source">The original game object with the component to copy.</param>
        /// <returns>The copied component attached to the destination GameObject.</returns>
        public static T CopyComponent<T>(this GameObject destination, GameObject source) where T : Component => destination.CopyComponent<T>(source.GetComponent<T>().OrNull());
        /// <summary>
        /// Copies a component from one GameObject to another.
        /// </summary>
        /// <typeparam name="T">The type of the component to copy.</typeparam>
        /// <param name="destination">The GameObject to which the component will be copied.</param>
        /// <param name="component">The original component to copy.</param>
        /// <returns>The copied component attached to the destination GameObject.</returns>
        public static T CopyComponent<T>(this GameObject destination, T component) where T : Component
        {
            if (component == null) { return null; }

            Type componentType = component.GetType();
            Component copy = destination.AddComponent(componentType);
            //copy.Copy(component);

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

        public static T Copy<T>(this Component destination, T component) where T : Component
        {
            Type componentType = component.GetType();

            FieldInfo[] fields = componentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(component));
            }

            PropertyInfo[] properties = componentType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite && property.GetSetMethod(true) != null)
                {
                    try
                    {
                        property.SetValue(destination, property.GetValue(component));
                    }
                    catch { }
                }
            }

            return destination as T;
        }



        public static bool TryGetComponentInParent<T>(this GameObject go, out T component) where T : Component
        {
            component = go.GetComponentInParent<T>().OrNull();
            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject go, out T component) where T : Component
        {
            component = go.GetComponentInChildren<T>().OrNull();
            return component != null;
        }

        public static T GetComponentInParentOrChildren<T>(this GameObject go) where T : Component
        {
            return go.GetComponentInParent<T>().OrNull() ?? go.GetComponentInChildren<T>().OrNull();
        }

    }
}