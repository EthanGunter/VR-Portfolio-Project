using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public interface IGameObject : IComponent
{

    public Transform transform { get; }
    public int layer { get; }
    public bool active { get; }
    public bool activeSelf { get; }
    public bool activeInHierarchy { get; }
    public bool isStatic { get; }
    internal bool isStaticBatchable { get; }
    //public unsafe string tag { get; }
    public Scene scene { get; }
    public ulong sceneCullingMask { get; }
    public GameObject gameObject { get; }
    //public unsafe T GetComponent<T>();
    public Component GetComponent(Type type);
    public Component GetComponent(string type);
    public Component GetComponentInChildren(Type type, bool includeInactive);
    public Component GetComponentInChildren(Type type);
    public T GetComponentInChildren<T>();
    public T GetComponentInChildren<T>([UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
    public Component GetComponentInParent(Type type, bool includeInactive);
    public Component GetComponentInParent(Type type);
    public T GetComponentInParent<T>();
    public T GetComponentInParent<T>([UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
    public Component[] GetComponents(Type type);
    public T[] GetComponents<T>();
    public void GetComponents(Type type, List<Component> results);
    public void GetComponents<T>(List<T> results);
    public Component[] GetComponentsInChildren(Type type);
    public Component[] GetComponentsInChildren(Type type, [UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
    public T[] GetComponentsInChildren<T>(bool includeInactive);
    public void GetComponentsInChildren<T>(bool includeInactive, List<T> results);
    public T[] GetComponentsInChildren<T>();
    public void GetComponentsInChildren<T>(List<T> results);
    public Component[] GetComponentsInParent(Type type);
    public Component[] GetComponentsInParent(Type type, [UnityEngine.Internal.DefaultValue("false")] bool includeInactive);
    public void GetComponentsInParent<T>(bool includeInactive, List<T> results);
    public T[] GetComponentsInParent<T>(bool includeInactive);
    public T[] GetComponentsInParent<T>();
    //public unsafe bool TryGetComponent<T>(out T component);
    public bool TryGetComponent(Type type, out Component component);
    public void SendMessageUpwards(string methodName, SendMessageOptions options);
    public void SendMessage(string methodName, SendMessageOptions options);
    public void BroadcastMessage(string methodName, SendMessageOptions options);
    public Component AddComponent(Type componentType);
    public T AddComponent<T>();
    public int GetComponentCount();
    public Component GetComponentAtIndex(int index);
    public T GetComponentAtIndex<T>(int index);
    public int GetComponentIndex(Component component);
    public void SetActive(bool value);
    public bool CompareTag(string tag);
    public bool CompareTag(TagHandle tag);
    //public unsafe void SendMessageUpwards(string methodName, [UnityEngine.Internal.DefaultValue("null")] object value, [UnityEngine.Internal.DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
    public void SendMessageUpwards(string methodName, object value);
    public void SendMessageUpwards(string methodName);
    //public unsafe void SendMessage(string methodName, [UnityEngine.Internal.DefaultValue("null")] object value, [UnityEngine.Internal.DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
    public void SendMessage(string methodName, object value);
    public void SendMessage(string methodName);
    //public unsafe void BroadcastMessage(string methodName, [UnityEngine.Internal.DefaultValue("null")] object parameter, [UnityEngine.Internal.DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
    public void BroadcastMessage(string methodName, object parameter);
    public void BroadcastMessage(string methodName);
}