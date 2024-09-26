using UnityEngine;

public class PatrolBetween : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    [SerializeField] float speed = 1;

    private int index = 0;
    private void Update()
    {
        Transform target = positions[index];
        if (Vector3.Distance(transform.position, target.position) > .2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed);
        }
        else
        {
            index = (index + 1) % positions.Length;
        }
    }
}
