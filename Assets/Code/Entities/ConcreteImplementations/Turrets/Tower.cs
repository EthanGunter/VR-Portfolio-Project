
using SolarStorm.UnityToolkit;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace SolarStorm.Entities
{
    public class Tower : GameEntity
    {
        #region Variables

        // Inspector fields
        [Header("Turret Levels")]
        [field: SerializeField] public int Level { get; private set; }
        [SerializeField] private TurretLevelData[] levelData;

        [Header("Projectiles")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform[] projectileSpawnPoints;

        [Header("Targeting")]
        [SerializeField] private LayerMask targetLayers;
        [Tooltip("How many degrees away from the target a turret needs to be before it starts shooting")]
        [SerializeField] private FloatRef aimShootThreshold;
        [SerializeField] private Transform horizontalTR;
        [SerializeField] private Transform verticalTR;

        // C# fields
        public event Action<TurretLevelData> OnLevelChange;
        /// <summary>
        /// The position shooting from, and its index in the projectileSpawnPoints list
        /// </summary>
        public event Action<Transform, int> OnShoot;

        public TurretLevelData CurrentLevelData => levelData[Level];
        public GameEntity Target { get; private set; }

        protected ObjectPool<Projectile> projectiles;

        private float _angleFromTarget;
        private CancellationTokenSource _attackHandle;

        #endregion


        #region Unity Messages

        protected override void Awake()
        {
            base.Awake();
            projectiles = new ObjectPool<Projectile>(CreateProjectile, GetProjectile, ReleaseProjectile, DestroyProjectile);
        }

        protected override void Update()
        {
            base.Update();

            AcquireTarget();
            _angleFromTarget = Aim(CurrentLevelData.turnSpeed);
            if (_angleFromTarget < aimShootThreshold)
            {
                if (_attackHandle == null)
                {
                    _attackHandle = new CancellationTokenSource();
                    AttackTarget(_attackHandle.Token);
                }
            }
            else if (_attackHandle != null)
            {
                CancelAttack();
            }

            // TODO TEMP
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                LevelUp();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Level = Mathf.Max(Level - 1, 0);
                OnLevelChange?.Invoke(CurrentLevelData);
            }
            // END TEMP
        }

        #endregion


        public void SetTarget(GameEntity target)
        {
            Target = target;
        }

        public void LevelUp()
        {
            Level = Mathf.Min(Level + 1, levelData.Length - 1);
            OnLevelChange?.Invoke(levelData[Level]);
        }

        private void AcquireTarget()
        {
            // If the target goes out of range, unset it
            if (Target != null)
            {
                if (Vector3.Distance(Target.transform.position, transform.position) > CurrentLevelData.maxTargetingDistance)
                {
                    CancelAttack();
                    Target = null;
                }
            }

            // Then, if there is no target, get the closest one and set it as the target
            if (Target == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, CurrentLevelData.maxTargetingDistance, targetLayers);
                GameEntity closestTarget = null;
                float closestDist = float.MaxValue;
                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out GameEntity potentialTarget))
                    {
                        float dist = Vector3.Distance(potentialTarget.transform.position, transform.position);
                        if (dist < closestDist)
                        {
                            closestTarget = potentialTarget;
                            closestDist = dist;
                        }
                    }
                }

                Target = closestTarget;
            }
        }
        private float Aim(float turnSpeed)
        {
            if (Target != null)
            {
                //Horizontal
                Vector3 dirH = Target.transform.position - horizontalTR.position;
                Quaternion rotatingSideRot = Quaternion.RotateTowards(horizontalTR.localRotation, Quaternion.LookRotation(dirH), turnSpeed);
                horizontalTR.localRotation = Quaternion.Euler(0, rotatingSideRot.eulerAngles.y, 0);

                //Vertical
                Vector3 dirV = Target.transform.position - verticalTR.position;
                Quaternion pitchingSideRot = Quaternion.RotateTowards(verticalTR.localRotation, Quaternion.LookRotation(dirV), turnSpeed);
                verticalTR.localRotation = Quaternion.Euler(pitchingSideRot.eulerAngles.x, 0, 0);

                Quaternion barrelTargetRotation = Quaternion.LookRotation(Target.transform.position - projectileSpawnPoints[0].position);
                return Quaternion.Angle(Quaternion.Euler(pitchingSideRot.eulerAngles.x, rotatingSideRot.eulerAngles.y, 0), barrelTargetRotation);
            }
            else
            {
                // Slow pan
                // TODO
                return 360;
            }
        }
        private async Awaitable AttackTarget(CancellationToken token)
        {
            int spawnIndex = 0;
            while (Target != null && !token.IsCancellationRequested)
            {
                TurretLevelData data = levelData[Level];
                Projectile p = projectiles.Get();
                p.Initialize(Target, data.damagePerShot, data.projectileSpeed);

                spawnIndex = (spawnIndex + 1) % projectileSpawnPoints.Length;
                p.transform.position = projectileSpawnPoints[spawnIndex].position;

                OnShoot?.Invoke(projectileSpawnPoints[spawnIndex], spawnIndex);

                await Awaitable.WaitForSecondsAsync(60 / data.shotsPerMinute);
            }
        }
        private void CancelAttack()
        {
            if (_attackHandle != null)
            {
                _attackHandle.Cancel();
                _attackHandle.Dispose();
            }
            _attackHandle = null;
        }


        #region Projectile Pool

        private Projectile CreateProjectile()
        {
            Projectile p = Instantiate(projectilePrefab, transform);
            p.OnHit += OnProjectileHit;
            return p;
        }

        private void GetProjectile(Projectile projectile)
        {
            projectile.Show();
        }

        private async void ReleaseProjectile(Projectile projectile)
        {
            await projectile.HideAsync();
            projectile.Hide();
        }

        private void DestroyProjectile(Projectile projectile)
        {
            projectile.OnHit -= OnProjectileHit;
            Destroy(projectile);
        }

        private void OnProjectileHit(Projectile proj, GameObject ent)
        {
            projectiles.Release(proj);
        }

        #endregion
    }
}