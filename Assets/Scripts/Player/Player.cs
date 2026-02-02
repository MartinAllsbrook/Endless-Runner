using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] Projectile projectilePrefab;
    
    [Header("Car Movement")]
    [SerializeField] float acceleration = 10f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float turnSpeed = 150f;
    [SerializeField] float brakeForce = 20f;
    [SerializeField] float drag = 2f;

    float steerInput = 0f;
    float throttleInput = 0f;
    float brakeInput = 0f;

    ObjectPool<Projectile> projectilePool;

    Rigidbody2D rb;
    Health health;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Get components
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        // Initialize projectile pool
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);
    }

    void OnEnable()
    {
        health.OnDeath += Die;

        // Inputs
        InputReader.Move += HandleSteer;
        InputReader.Shoot += HandleShoot;
        InputReader.Throttle += HandleThrottle;
        InputReader.Brake += HandleBrake;
    }

    void OnDisable()
    {
        health.OnDeath -= Die;

        // Inputs
        InputReader.Move -= HandleSteer;
        InputReader.Shoot -= HandleShoot;
        InputReader.Throttle -= HandleThrottle;
        InputReader.Brake -= HandleBrake;
    }

    void FixedUpdate()
    {
        // Apply car-like movement
        ApplyMovement();
    }

    void ApplyMovement()
    {
        // Get current velocity
        Vector2 velocity = rb.linearVelocity;
        float currentSpeed = velocity.magnitude;

        // Calculate forward direction
        Vector2 forward = transform.up;

        // Apply throttle (acceleration)
        if (throttleInput > 0.1f)
        {
            if (currentSpeed < maxSpeed)
            {
                rb.AddForce(forward * acceleration * throttleInput, ForceMode2D.Force);
            }
        }

        // Apply brake
        if (brakeInput > 0.1f)
        {
            rb.AddForce(-velocity.normalized * brakeForce * brakeInput, ForceMode2D.Force);
        }

        // Apply steering (only when moving)
        if (Mathf.Abs(steerInput) > 0.1f && currentSpeed > 0.5f)
        {
            float turnAmount = steerInput * turnSpeed * currentSpeed * Time.fixedDeltaTime;
            // Reduce turn speed at higher velocities for more realistic handling
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            rb.angularVelocity = -turnAmount * speedFactor * 10f;
        }
        else
        {
            rb.angularVelocity = 0f;
        }

        // Apply drag
        rb.linearVelocity = Vector2.Lerp(velocity, Vector2.zero, drag * Time.fixedDeltaTime);
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

    void HandleBrake(float value)
    {
        Debug.Log("Brake input: " + value);
        brakeInput = value;
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