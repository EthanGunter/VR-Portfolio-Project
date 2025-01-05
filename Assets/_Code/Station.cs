using Sirenix.OdinInspector;
using SolarStorm.UnityToolkit;
using UnityEngine;

public class Station : MonoBehaviour
{
    #region Variables

    [ShowInInspector, Title("Temp, global")] static float headCounterOffset = 0.4f;

    [SerializeField] ScriptableEvent<float> playerHeightChanged;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        playerHeightChanged.AddListener(HandlePlayerHeightChanged);
    }
    private void OnDestroy()
    {
        playerHeightChanged.RemoveListener(HandlePlayerHeightChanged);
    }


    #endregion


    #region Event Handlers

    private void HandlePlayerHeightChanged(float newHeight)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 2, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
        {
            // Place the counter surface just below the player's head
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            transform.position = hit.point + Vector3.up * (newHeight - headCounterOffset);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            Debug.LogError("Station find-ground raycast failed", this);
        }
    }

    #endregion
}