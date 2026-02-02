using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CarMovement))]
class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] Projectile projectilePrefab;
    [SerializeField] HealthBar healthBar;

    float steerInput = 0f;
    float throttleInput = 0f;

    ObjectPool<Projectile> projectilePool;

    Health health;
    CarMovement carMovement;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Get components
        health = GetComponent<Health>();
        carMovement = GetComponent<CarMovement>();

        // Initialize projectile pool
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);
    }

    void OnEnable()
    {
        health.OnDeath += Die;
        health.OnHealthChangedPercent += healthBar.SetFill;

        // Inputs
        InputReader.Move += HandleSteer;
        InputReader.Shoot += HandleShoot;
        InputReader.Throttle += HandleThrottle;
    }

    void OnDisable()
    {
        health.OnDeath -= Die;
        health.OnHealthChangedPercent -= healthBar.SetFill;

        // Inputs
        InputReader.Move -= HandleSteer;
        InputReader.Shoot -= HandleShoot;
        InputReader.Throttle -= HandleThrottle;
    }

    void FixedUpdate()
    {
        // Send input to CarMovement component
        Vector2 movementInput = new Vector2(steerInput, throttleInput);
        carMovement.SetMovementInput(movementInput);
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Implement respawn or game over logic here
    }

    #region Input Handlers
    void HandleSteer(float value)
    {
        steerInput = value;
    }

    void HandleThrottle(float value)
    {
        Debug.Log("Throttle input: " + value);
        throttleInput = value;
    }

    void HandleShoot()
    {
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
            proj.Initialize(rotation, 10f, 10f); // Example speed/range, adjust as needed
        }
    }
    #endregion
}