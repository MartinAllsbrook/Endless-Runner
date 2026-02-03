using UnityEngine;
using UnityEngine.InputSystem;

public class SwivelGun : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] float rateOfFire = 4f;
    [SerializeField] float projectileSpeed = 45f;
    [SerializeField] float rotationSpeed = 360f; // Degrees per second

    float fireCooldown = 0f;
    float targetAngle = 0f;

    ObjectPool<Projectile> projectilePool;


    void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);

    }

    public void Update()
    {
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

        // Update target rotation based on mouse position
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 playerPos = transform.position;
        Vector3 direction = (mouseWorldPos - playerPos).normalized;

        // Calculate target angle towards cursor
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Smoothly rotate towards target angle
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
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

        // Fire in the direction the gun is facing
        Vector3 direction = transform.up; // Since we rotated by -90, "up" is the forward direction
        
        // Get projectile from pool
        Projectile proj = projectilePool.Get(transform.position, transform.rotation);
        if (proj != null)
        {
            Vector2 velocity = direction * projectileSpeed;
            velocity += Player.Instance.GetComponent<Rigidbody2D>().linearVelocity; // TODO: Don't do GET component every time
            proj.Initialize(velocity, 5f); // Example Lifetime
        }
    }
}
