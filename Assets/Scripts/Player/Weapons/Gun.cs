using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float rateOfFire = 4f;
    [SerializeField] float projectileSpeed = 45f;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] Transform firePoint;

    float fireCooldown = 0f;
    ObjectPool<Projectile> projectilePool;

    void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);
    }

    protected virtual void Update()
    {
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public void TryFire()
    {
        if (fireCooldown <= 0f)
        {
            Fire();
        }
    }  

    void Fire()
    {
        fireCooldown = 1f / rateOfFire;

        Vector3 direction = firePoint.up; // 2D forward direction
    
        Projectile proj = projectilePool.Get(firePoint.position, firePoint.rotation);
        if (proj != null)
        {
            Vector2 velocity = direction * projectileSpeed;
            velocity += Player.Instance.GetComponent<Rigidbody2D>().linearVelocity; // TODO: Don't do GET component every time
            proj.Initialize(velocity, projectileLifetime);
        }
    }
}