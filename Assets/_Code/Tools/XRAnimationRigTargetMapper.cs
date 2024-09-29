using System;
using UnityEngine;

[Serializable]
public class TransformMap
{
    public Transform Source;
    public Transform Follower;
    //public Vector3 PositionOffset;
    //public Vector3 RotationOffset;

    public void Map()
    {
        if (Source == null || Follower == null) return;

        Vector3 rot = Source.transform.rotation.eulerAngles;
        Follower.transform.SetPositionAndRotation(
            Source.transform.position /*+ PositionOffset*/,
            Quaternion.Euler(
                rot.x /*+ RotationOffset.x*/,
                rot.y /*+ RotationOffset.y*/,
                rot.z /*+ RotationOffset.z*/
                )
            );
    }
}

public class XRAnimationRigTargetMapper : MonoBehaviour
{
    [SerializeField] Vector3 headFloorOffset;
    [SerializeField] TransformMap head;
    [SerializeField] TransformMap leftHand;
    [SerializeField] TransformMap leftElbowTarget;
    [SerializeField] TransformMap rightHand;

    public float yRotation; // TODO Temp

    private void LateUpdate()
    {
        float yRot = yRotation = head.Source.eulerAngles.y;
        transform.SetPositionAndRotation(
            head.Source.position + headFloorOffset,
            Quaternion.Euler(transform.eulerAngles.x, yRot, transform.eulerAngles.z)
        );

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
