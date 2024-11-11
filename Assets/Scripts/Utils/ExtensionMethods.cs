using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class ExtensionMethods
{
    public static TComponent CopyComponent<TComponent>(this GameObject destination, TComponent originalComponent) where TComponent : Component
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

        return copy as TComponent;
    }

}
