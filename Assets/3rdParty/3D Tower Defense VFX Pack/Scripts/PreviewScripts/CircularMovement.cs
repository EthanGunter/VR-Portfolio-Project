using UnityEngine;

namespace TowerDefenseVFXPack
{
    /// <summary>
    /// Make sure to remove this script from the prefab if you want to implement your own behaviour.
    /// </summary>
    public class CircularMovement : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float speed = 5f;

        private Vector3 centerPosition;
        private float angle = 0f;

        private void Start()
        {
            centerPosition = transform.position;
            transform.position = centerPosition + new Vector3(radius, 0f, 0f);
        }

        private void Update()
        {
            CircleBehaviour();
        }

        private void CircleBehaviour()
        {
            float angularSpeed = speed * Mathf.Deg2Rad * Time.deltaTime;

            angle += angularSpeed;

            if (angle >= Mathf.PI * 2f)
                angle -= Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            transform.position = centerPosition + new Vector3(x, 0f, z);
        }
    }
}