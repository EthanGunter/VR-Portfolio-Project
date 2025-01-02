using UnityEngine;

public class SetPedestalHeight : MonoBehaviour
{
    [SerializeField] Transform[] topParts;
    [SerializeField] Transform bottomPart;
    [SerializeField] float readingOffset = .2f;

    private float[] topPartOffsets;
    private float bottomPartOffset;

    private void Awake()
    {
        topPartOffsets = new float[topParts.Length];
        for (int i = 0; i < topParts.Length; i++)
        {
            topPartOffsets[i] = topParts[i].localPosition.y - bottomPart.localScale.y;
        }
        bottomPartOffset = bottomPart.localPosition.y;
    }

    public void SetHeight(float height)
    {
        height = height - readingOffset;
        for (int i = 0; i < topParts.Length; i++)
        {
            Transform part = topParts[i];
            part.position = new Vector3(part.position.x, transform.position.y + topPartOffsets[i] + height, part.position.z);
        }

        bottomPart.localScale = new Vector3(bottomPart.localScale.x, height, bottomPart.localScale.z);
        bottomPart.localPosition = new Vector3(bottomPart.localPosition.x, height / 2, bottomPart.localPosition.z);
    }
}
