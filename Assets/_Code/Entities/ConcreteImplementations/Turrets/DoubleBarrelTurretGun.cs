using SolarStorm.Entities;
using System;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public class DoubleBarrelTurretGun : MonoBehaviour, ITurretWeapon
{
    #region Variables

    [Header("Projectiles")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform[] projectileSpawnPoints;

    /// <summary>
    /// The position shooting from, and its index in the projectileSpawnPoints list
    /// </summary>
    public event Action<int> OnShoot;
    protected ObjectPool<Projectile> projectiles;

    protected int spawnIndex = 0;

    #endregion


    #region Unity Messages

    protected void Awake()
    {
        projectiles = new ObjectPool<Projectile>(CreateProjectile, GetProjectile, ReleaseProjectile, DestroyProjectile);
    }

    #endregion


    public void Attack(GameObject target, TurretLevelData data)
    {
        Projectile p = projectiles.Get();
        p.Initialize(target, data.damagePerShot, data.projectileSpeed);

        spawnIndex = (spawnIndex + 1) % projectileSpawnPoints.Length;
        p.transform.position = projectileSpawnPoints[spawnIndex].position;
        RaiseOnShoot(spawnIndex);
    }

    protected void RaiseOnShoot(int spawnIndex)
    {
        OnShoot?.Invoke(spawnIndex);
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