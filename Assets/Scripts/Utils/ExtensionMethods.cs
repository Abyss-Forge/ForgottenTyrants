using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class ExtensionMethods
{

    public static bool IsInRange<T>(this T[] array, int index) => index >= 0 && index < array.Length;

    /// <summary>
    /// Returns the object itself if it exists, null otherwise.
    /// </summary>
    /// <remarks>
    /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
    /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
    /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
    /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
    /// </remarks>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object being checked.</param>
    /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
    public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;

    public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

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