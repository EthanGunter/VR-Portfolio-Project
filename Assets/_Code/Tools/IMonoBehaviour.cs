using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public interface IMonoBehaviour
{
    public CancellationToken destroyCancellationToken { get; }
    public bool useGUILayout { get; }
    public bool didStart { get; }
    public bool didAwake { get; }
    public bool runInEditMode { get; }
    internal bool allowPrefabModeInPlayMode { get; }
    public bool IsInvoking();
    public void CancelInvoke();
    public void Invoke(string methodName, float time);
    public void InvokeRepeating(string methodName, float time, float repeatRate);
    public void CancelInvoke(string methodName);
    public bool IsInvoking(string methodName);
    public Coroutine StartCoroutine(string methodName);
    public Coroutine StartCoroutine(string methodName, object value);
    public Coroutine StartCoroutine(IEnumerator routine);
    public Coroutine StartCoroutine_Auto(IEnumerator routine);
    public void StopCoroutine(IEnumerator routine);
    public void StopCoroutine(Coroutine routine);
    //public unsafe void StopCoroutine(string methodName);
    public void StopAllCoroutines();
}