using SolarStorm.Entities;
using SolarStorm.UnityToolkit;
using System;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour, IView
{
    #region Variables

    [SerializeField] float damage;
    [SerializeField] float speed;

    private HealthComponent _target;

    public event Action<Projectile, GameObject> OnHit;

    #endregion


    #region Unity Messages

    #endregion


    public void Initialize(HealthComponent target)
    {
        _target = target;
    }

    private void Update()
    {
        if (MoveToTarget())
            HitTarget();
    }

    private bool MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, speed * Time.deltaTime);
        Vector3 dir = _target.transform.position - transform.position;

        if (dir.sqrMagnitude < 0.001f)
            return true;

        transform.rotation = Quaternion.LookRotation(dir);
        return false;
    }

    private void HitTarget()
    {
        _target.GetComponent<HealthComponent>().DealDamage(new DamageContext(damage));
        OnHit?.Invoke(this, _target.gameObject);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public async Awaitable HideAsync(CancellationToken cancellationToken = default)
    {
    }

    public async Awaitable ShowAsync(CancellationToken cancellationToken = default)
    {
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}