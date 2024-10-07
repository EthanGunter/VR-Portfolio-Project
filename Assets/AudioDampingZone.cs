using SolarStorm.UnityToolkit;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;


/*
 * !BEWARE! This component abandoned
 * This class attempted to implement a "simple" zone system that detected how deep an object was within its bounds.
 * Turns out that's prohibitively difficult with Unity's current collider API, and I don't want to deal with that right now :)
 */

[RequireComponent(typeof(Collider))]
public class AudioDampingZone : MonoBehaviour
{
    [SerializeField] FloatRef fadeDistance = 1;
    [SerializeField] AudioMixerGroup affectedGroup;
    [SerializeField] StringRef lowpassCutoffParamName = "lowpassCutoffFreq";
    [SerializeField] FloatRef cutoffFrequency = 1;
    [SerializeField] FloatRef defaultFrequency = 22000;

    private Collider _col;
    private Collider listenerCollider;

    private CancellationTokenSource filterToken;

    /*private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (listenerCollider == null && other.GetComponentInChildren<AudioListener>(true) != null)
        {
            listenerCollider = other;
        }

        if (other == listenerCollider)
        {
            if (filterToken == null)
            {
                filterToken = new CancellationTokenSource();
                Filter(filterToken.Token);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == listenerCollider)
        {
            if (filterToken == null) Debug.LogException(new InvalidOperationException($"Listener object {other.name} left zone without entering it. What?"), this);
            filterToken.Cancel();
            filterToken.Dispose();
            filterToken = null;
        }
    }

    private async Task Filter(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // dist from listener center to closest point on trigger collider
            float distInCollider = Vector3.Distance(listenerCollider.bounds.center, _col.ClosestPointOnBounds(listenerCollider.bounds.center));
            float strength = Mathf.Clamp(distInCollider / fadeDistance, 0, 1);
            float cutoff = strength.Remap(0, 1, 22000, cutoffFrequency);

            Debug.Log($"Dist in: {distInCollider} | Strength: {strength} | Cutoff: {cutoff}", this);
            affectedGroup.audioMixer.SetFloat(lowpassCutoffParamName, cutoff);
            await Awaitable.EndOfFrameAsync();
        }
        affectedGroup.audioMixer.SetFloat(lowpassCutoffParamName, defaultFrequency);
    }

    private void OnDrawGizmos()
    {
        if (listenerCollider != null)
        {
            Gizmos.color = Color.red;
            Vector3 colliderPos = listenerCollider.bounds.center;
            Gizmos.DrawSphere(colliderPos, .1f);
            Gizmos.color = Color.green;
            Vector3 closest = _col.ClosestPointOnBounds(colliderPos);
            Gizmos.DrawSphere(closest, .1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(closest, listenerCollider.bounds.center);
            Gizmos.color = Color.white;
        }

        if (_col is BoxCollider boxCol)
        {
            Vector3 innerSize = boxCol.bounds.size;
            innerSize.x = Mathf.Clamp(innerSize.x - fadeDistance, 0, float.MaxValue);
            innerSize.y = Mathf.Clamp(innerSize.y - fadeDistance, 0, float.MaxValue);
            innerSize.z = Mathf.Clamp(innerSize.z - fadeDistance, 0, float.MaxValue);

            Gizmos.DrawWireCube(boxCol.bounds.center, innerSize);
            Gizmos.DrawWireCube(boxCol.bounds.center, boxCol.bounds.size);
        }
        else if (_col is SphereCollider sphereCol)
        {
            Gizmos.DrawWireSphere(sphereCol.center, Mathf.Clamp(sphereCol.radius - fadeDistance, 0, float.MaxValue));
            //Gizmos.DrawWireSphere(sphereCol.center, sphereCol.radius);
        }
    }*/
}
