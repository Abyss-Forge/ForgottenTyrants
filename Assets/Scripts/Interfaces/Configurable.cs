using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

public abstract class Configurable<T, U> : MonoBehaviour
{
    protected Dictionary<T, U> _settings, _defaultSettings;
    protected string _xmlFilePath;

    private string _keyName, _valueName, _childName, _rootName;

    protected Configurable(string rootName, string childName, string keyName, string valueName)
    {
        _rootName = rootName;
        _childName = childName;
        _keyName = keyName;
        _valueName = valueName;
    }

    void Awake()
    {
        _settings = new Dictionary<T, U>();
        _defaultSettings = new Dictionary<T, U>();
        _xmlFilePath = SGlobalSettings.XmlFilePath;
        LoadFromXml();
        OnAwake();
    }
    protected virtual void OnAwake() { }

    protected abstract void InitializeDefaults();

    public void ResetAllToDefault()
    {
        InitializeDefaults();
        _settings = _defaultSettings;
        SaveToXml();
    }

    public void ResetToDefault(/*SingleSetting foo*/)
    {
        InitializeDefaults();
        //TODO
    }

    public void SaveToXml()
    {
        File.Delete(_xmlFilePath);

        XmlDocument doc = new XmlDocument();
        XmlElement rootElement = doc.CreateElement(_rootName);
        doc.AppendChild(rootElement);

        foreach (var item in _settings)
        {
            XmlElement childElement = doc.CreateElement(_childName);
            rootElement.AppendChild(childElement);

            XmlElement keyElement = doc.CreateElement(_keyName);
            keyElement.InnerText = item.Key.ToString();
            childElement.AppendChild(keyElement);

            XmlElement valueElement = doc.CreateElement(_valueName);
            valueElement.InnerText = item.Value.ToString();
            childElement.AppendChild(valueElement);
        }

        doc.Save(_xmlFilePath);
    }

    public void LoadFromXml()
    {
        if (File.Exists(_xmlFilePath))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_xmlFilePath);
            _settings.Clear();

            XmlNodeList childNodes = doc.SelectNodes($"/{_rootName}/{_childName}");
            foreach (XmlNode childNode in childNodes)
            {
                T key = (T)Convert.ChangeType(childNode.SelectSingleNode(_keyName)?.InnerText, typeof(T));
                U value = (U)Convert.ChangeType(childNode.SelectSingleNode(_valueName)?.InnerText, typeof(U));

                _settings.Add(key, value);
            }
        }
        else
        {
            ResetAllToDefault();
            SaveToXml();
        }
    }

}