using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SwivelGun : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float rateOfFire = 4f;

    float fireCooldown = 0f;

    ObjectPool<Projectile> projectilePool;

    Rigidbody2D rb;

    void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);

        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
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

        // Get mouse position in world space (2D)
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Get direction from player to mouse
        Vector3 playerPos = transform.position;
        Vector3 direction = (mouseWorldPos - playerPos).normalized;

        // Calculate rotation towards cursor
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Get projectile from pool
        Projectile proj = projectilePool.Get(playerPos, rotation);
        if (proj != null)
        {
            proj.SetPool(projectilePool);

            Vector2 velocity = direction * 10f; // Example speed
            velocity += rb.linearVelocity; // Add player's current velocity
            proj.Initialize(velocity, 10f); // Example range
        }
    }
}
