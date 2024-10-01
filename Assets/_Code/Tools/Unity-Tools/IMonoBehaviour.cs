using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using UnityEngine.Internal;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using UnityEngineInternal;
using Component = UnityEngine.Component;

namespace SolarStorm.UnityToolkit
{
    public interface IMonoBehaviour : IBehaviour
    {
        public bool IsInvoking();
        public void CancelInvoke();
        public void Invoke(string methodName, float time);
        public void InvokeRepeating(string methodName, float time, float repeatRate);
        public void CancelInvoke(string methodName);
        public bool IsInvoking(string methodName);
        public Coroutine StartCoroutine(string methodName);
        public Coroutine StartCoroutine(string methodName, object value = null);
        public Coroutine StartCoroutine(IEnumerator routine);
        public void StopCoroutine(IEnumerator routine);
        public void StopCoroutine(Coroutine routine);
        public void StopCoroutine(string methodName);
        public void StopAllCoroutines();
    }
    public interface IBehaviour : IComponent
    {
        public bool enabled
        {
            get;
            set;
        }
        public bool isActiveAndEnabled
        {
            get;
        }
    }
    public interface IComponent : IObject
    {
        public Transform transform
        {
            get;
        }
        public GameObject gameObject
        {
            get;
        }
        public string tag
        {
            get;
            set;
        }
        public Component GetComponent(Type type);

        public T GetComponent<T>();
        public bool TryGetComponent(Type type, out Component component);
        public bool TryGetComponent<T>(out T component);
        public Component GetComponent(string type);
        public Component GetComponentInChildren(Type t, bool includeInactive);
        public Component GetComponentInChildren(Type t);
        public T GetComponentInChildren<T>([UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
        public T GetComponentInChildren<T>();
        public Component[] GetComponentsInChildren(Type t, bool includeInactive);
        public Component[] GetComponentsInChildren(Type t);
        public T[] GetComponentsInChildren<T>(bool includeInactive);
        public void GetComponentsInChildren<T>(bool includeInactive, List<T> result);
        public T[] GetComponentsInChildren<T>();
        public void GetComponentsInChildren<T>(List<T> results);
        public Component GetComponentInParent(Type t, bool includeInactive);
        public Component GetComponentInParent(Type t);
        public T GetComponentInParent<T>([UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
        public T GetComponentInParent<T>();
        public Component[] GetComponentsInParent(Type t, [UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
        public Component[] GetComponentsInParent(Type t);
        public T[] GetComponentsInParent<T>(bool includeInactive);
        public void GetComponentsInParent<T>(bool includeInactive, List<T> results);
        public T[] GetComponentsInParent<T>();
        public Component[] GetComponents(Type type);
        public void GetComponents(Type type, List<Component> results);
        public void GetComponents<T>(List<T> results);
        public T[] GetComponents<T>();
        public bool CompareTag(string tag);
        public void SendMessageUpwards(string methodName, [UnityEngine.Internal.DefaultValue("null")] object value, [UnityEngine.Internal.DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        public void SendMessageUpwards(string methodName, object value);
        public void SendMessageUpwards(string methodName);
        public void SendMessageUpwards(string methodName, SendMessageOptions options);
        public void SendMessage(string methodName, object value);
        public void SendMessage(string methodName);
        public void SendMessage(string methodName, object value, SendMessageOptions options);
        public void SendMessage(string methodName, SendMessageOptions options);
        public void BroadcastMessage(string methodName, [UnityEngine.Internal.DefaultValue("null")] object parameter, [UnityEngine.Internal.DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        public void BroadcastMessage(string methodName, object parameter);
        public void BroadcastMessage(string methodName);
        public void BroadcastMessage(string methodName, SendMessageOptions options);
    }
    public interface IObject
    {
        public string name { get; set; }
        public HideFlags hideFlags { get; set; }
        public int GetInstanceID();
        public int GetHashCode();
        public bool Equals(object other);
    }
}
