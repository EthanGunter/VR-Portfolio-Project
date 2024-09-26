using UnityEngine;

namespace TowerDefenseVFXPack
{
    /// <summary>
    /// Make sure to remove this script from the prefab if you want to implement your own behaviour.
    /// </summary>
    public class ShootAtTransform : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] porjectileSpawnPoints;
        [SerializeField] private float shootCooldown;

        [Header("Prefabs")]
        [SerializeField] private ProjectileBehaviour projectilePrefab;

        [Header("Animations")]
        [SerializeField] private ParticleSystem onShootParticleSystem;
        [SerializeField] private Animator towerAnimator;
        [SerializeField] private Transform horizontalTR;
        [SerializeField] private Transform verticalTR;
        [SerializeField] private string[] animatorTriggerNames;


        private float currentCooldown;
        private int nextSpawnPoint = 0;

        private void Awake()
        {
            currentCooldown = shootCooldown;
        }

        private void Update()
        {
            if (target == null) return;

            Aim();

            if (ReadyToShoot())
                Shoot();
        }

        private bool ReadyToShoot()
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0)
            {
                currentCooldown = shootCooldown;
                return true;
            }

            return false;
        }

        private void Shoot()
        {
            if (onShootParticleSystem != null)
                onShootParticleSystem.Play();

            if (projectilePrefab != null)
                Instantiate(projectilePrefab, porjectileSpawnPoints[nextSpawnPoint].position, Quaternion.identity).SetTarget(target);

            if(towerAnimator != null)
                towerAnimator.SetTrigger(animatorTriggerNames[nextSpawnPoint]);

            nextSpawnPoint = (nextSpawnPoint + 1) % porjectileSpawnPoints.Length;
        }

        private void Aim()
        {
            //Horizontal
            Vector3 dirH = target.position - horizontalTR.position;
            Quaternion rotatingSideRot = Quaternion.Lerp(horizontalTR.localRotation, Quaternion.LookRotation(dirH), Time.deltaTime * 50);
            horizontalTR.localRotation = Quaternion.Euler(0, rotatingSideRot.eulerAngles.y, 0);

            //Vertical
            Vector3 dirV = target.position - verticalTR.position;
            Quaternion pitchingSideRot = Quaternion.Lerp(verticalTR.localRotation, Quaternion.LookRotation(dirV), Time.deltaTime * 50);
            verticalTR.localRotation = Quaternion.Euler(pitchingSideRot.eulerAngles.x, 0, 0);
        }
    }
}