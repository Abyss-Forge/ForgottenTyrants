using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public abstract class Presettable<T> : MonoBehaviour where T : class
{
    protected List<T> _presets { get; private set; } = new();
    protected string _xmlFilePath { get; set; } = SGlobalSettings.XmlFilePath;

    private string _rootName, _parentName;
    private int _presetLimit;
    private bool _arePresetsDeleteable;

    protected Presettable(string rootNameXML, string elementNameXML, int presetLimit = -1, bool arePresetsDeleteable = true)
    {
        _rootName = rootNameXML;
        _parentName = elementNameXML;
        _presetLimit = presetLimit;
        _arePresetsDeleteable = arePresetsDeleteable;
    }

    void Start()
    {
        LoadFromXml();
        OnStart();
    }
    protected virtual void OnStart() { }

    public bool CreatePreset(T preset)
    {
        if (_presetLimit <= 0 || _presetLimit - 1 > _presets.Count) //si es ilimitado, o queda espacio
        {
            _presets.Add(preset);
            SaveToXml();
            return true;
        }
        else
        {
            while (_presetLimit - 1 <= _presets.Count)   //borrar sobrantes
            {
                _presets.RemoveAt(_presets.Count - 1);
            }
            return false;
        }
    }

    public bool DeletePreset(T preset)
    {
        if (_presets.Contains(preset))
        {
            return DeletePreset(_presets.IndexOf(preset));
        }
        return false;
    }

    public bool DeletePreset(int index)
    {
        if (_arePresetsDeleteable && _presets.Count > 0 && index >= 0 && index <= _presets.Count)
        {
            _presets.RemoveAt(index);
            SaveToXml();
            return true;
        }
        return false;
    }

    public void SaveToXml()
    {
        File.Delete(_xmlFilePath);

        XmlDocument doc = new();
        XmlElement rootElement = doc.CreateElement(_rootName);
        doc.AppendChild(rootElement);

        var properties = typeof(T).GetProperties();
        foreach (var item in _presets)
        {
            XmlElement parentElement = doc.CreateElement(_parentName);
            rootElement.AppendChild(parentElement);

            foreach (var property in properties)
            {
                XmlElement childElement = doc.CreateElement(property.Name);
                var value = property.GetValue(item)?.ToString() ?? string.Empty;
                childElement.InnerText = value;
                parentElement.AppendChild(childElement);
            }
        }

        doc.Save(_xmlFilePath);
    }

    public void LoadFromXml()
    {
        if (File.Exists(_xmlFilePath))
        {
            XmlDocument doc = new();
            doc.Load(_xmlFilePath);
            _presets.Clear();

            var properties = typeof(T).GetProperties();
            XmlNodeList parentNodes = doc.SelectNodes($"/{_rootName}/{_parentName}");

            foreach (XmlNode parentNode in parentNodes)
            {
                T item = Activator.CreateInstance<T>();

                foreach (var property in properties)
                {
                    var node = parentNode.SelectSingleNode(property.Name);
                    if (node != null)
                    {
                        var value = Convert.ChangeType(node.InnerText, property.PropertyType);
                        property.SetValue(item, value);
                    }
                }

                _presets.Add(item);
            }
        }
    }

}