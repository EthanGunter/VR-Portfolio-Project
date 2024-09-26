using UnityEngine;

namespace TowerDefenseVFXPack
{
    /// <summary>
    /// Make sure to remove this script from the prefab if you want to implement your own behaviour.
    /// </summary>
    public class ProjectileBehaviour : MonoBehaviour
    {
        [SerializeField] private ParticleSystem onHitAnimation;
        [SerializeField] private float projectileSpeed;

        private Transform target;

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        private void Update()
        {
            if (MoveToTarget())
                HitTarget();
        }

        private bool MoveToTarget()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, projectileSpeed * Time.deltaTime);
            Vector3 dir = target.position - transform.position;

            if (dir.sqrMagnitude < 0.001f)
                return true;

            transform.rotation = Quaternion.LookRotation(dir);
            return false;
        }

        private void HitTarget()
        {
            if (onHitAnimation != null)
                Instantiate(onHitAnimation, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}