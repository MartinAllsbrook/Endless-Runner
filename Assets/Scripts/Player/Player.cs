using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Inventory))]
class Player : MonoBehaviour
{
    public static Player Instance;

    float steerInput = 0f;
    float throttleInput = 0f;
    bool shootInput = false;

    SwivelGun swivelGun;
    Health health;
    CarMovement carMovement;
    Rigidbody2D rb;

    bool dead = false;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Get components
        swivelGun = GetComponentInChildren<SwivelGun>();
        health = GetComponent<Health>();
        carMovement = GetComponent<CarMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        health.OnDeath += Die;

        // Inputs
        InputReader.Move += HandleSteer;
        InputReader.Shoot += HandleShoot;
        InputReader.Throttle += HandleThrottle;
    }

    void OnDisable()
    {
        health.OnDeath -= Die;

        // Inputs
        InputReader.Move -= HandleSteer;
        InputReader.Shoot -= HandleShoot;
        InputReader.Throttle -= HandleThrottle;
    }

    void Update()
    {
        // Handle shooting
        if (shootInput)
        {
            swivelGun.TryFire();
        }
    }

    void FixedUpdate()
    {
        // Send input to CarMovement component
        Vector2 movementInput = new Vector2(steerInput, throttleInput);
        carMovement.SetMovementInput(movementInput);
    }

    async void Die()
    {
        health.OnDeath -= Die;
        
        if (dead) return;
        dead = true;

        Debug.Log("Player has died!");
        await GameManager.Instance.EndGame();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("World Object"))
        {
            Health objectHealth = collision.gameObject.GetComponent<Health>();
            float impactStrength = collision.relativeVelocity.magnitude;

            if (objectHealth != null)
            {
                objectHealth.DecreaseHealth(impactStrength * 8); // Damage enemy on collision
                health.DecreaseHealth(impactStrength * 0.5f); // Damage player on collision
            }
        }
    }

    async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tunnel"))
        {
            Tunnel tunnel = other.GetComponent<Tunnel>();
            if (tunnel != null)
            {
                await tunnel.EnterTunnel();
            }
        }
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Setting Player Transform");
        transform.position = position;
        transform.rotation = rotation;
    
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
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

    void HandleShoot(bool shoot)
    {
        shootInput = shoot;
    }
    #endregion
}