using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

[RequireComponent(typeof(TeleportationAnchor))]
public class SpawnPlayerAtStart : MonoBehaviour
{
    TeleportationProvider provider;
    TeleportationAnchor anchor;
    private void Awake()
    {
        anchor = GetComponent<TeleportationAnchor>();
        StartCoroutine(WaitToSpawn());
    }

    private IEnumerator WaitToSpawn()
    {
        while (!Player.HMDConnected)
        {
            yield return null;
        }
        anchor.RequestTeleport();
    }
}
