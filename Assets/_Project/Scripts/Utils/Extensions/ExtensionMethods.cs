using System;
using UnityEngine;
using System.Reflection;

namespace Utils.Extensions
{
    public static class ExtensionMethods
    {
        public static bool IsInRange<T>(this T[] array, int index) => index >= 0 && index < array.Length;

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