﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.ServiceLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> _services = new();
        public IEnumerable<object> RegisteredServices => _services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        public ServiceManager Register<T>(T service) => Register(typeof(T), service);
        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            if (!_services.TryAdd(type, service))
            {
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            //Debug.Log($"ServiceManager.Register: Registered service of type {type.FullName}");
            return this;
        }

        public ServiceManager Deregister<T>() where T : class => Deregister(typeof(T));
        public ServiceManager Deregister(Type type)
        {
            if (!_services.Remove(type))
            {
                Debug.LogError($"ServiceManager.Deregister: Service of type {type.FullName} not registered");
            }

            //Debug.Log($"ServiceManager.Deregister: Deregistered service of type {type.FullName}");
            return this;
        }
    }

}