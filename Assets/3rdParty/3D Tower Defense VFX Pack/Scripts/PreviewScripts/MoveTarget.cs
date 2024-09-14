using System.Collections;
using UnityEngine;

namespace TowerDefenseVFXPack
{
    /// <summary>
    /// Make sure to remove this script from the prefab if you want to implement your own behaviour.
    /// </summary>
    public class MoveTarget : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float distance = 5f;

        private Vector3 targetPosition;
        private Vector3 positionA;
        private Vector3 positionB;

        private bool movingToA = false;
        private bool coroutineIsRunning = false;

        private void Awake()
        {
            Vector3 currentPosition = transform.position;

            positionA = currentPosition + new Vector3(distance, 0, 0);
            positionB = currentPosition - new Vector3(distance, 0, 0);

            targetPosition = positionA;
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f && !coroutineIsRunning)
                StartCoroutine(SwitchPosition());
        }

        private IEnumerator SwitchPosition()
        {
            coroutineIsRunning = true;

            yield return new WaitForSeconds(1f);

            if (movingToA)
            {
                targetPosition = positionA;
            }
            else
            {
                targetPosition = positionB;
            }

            movingToA = !movingToA;

            coroutineIsRunning = false;
        }
    }
}