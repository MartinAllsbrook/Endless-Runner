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

    [SerializeField] Gun[] guns;
    [SerializeField] float collisionDamageFactor = 0.01f;
    float steerInput = 0f;
    float throttleInput = 0f;
    bool shootInput = false;

    Health health;
    CarMovement carMovement;
    Rigidbody2D rb;
    Collider2D playerCollider;

    bool dead = false;

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
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
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
            foreach (Gun gun in guns)
            {
                gun.TryFire();
            }
        }
    }

    void FixedUpdate()
    {
        // Send input to CarMovement component
        Vector2 movementInput = new Vector2(steerInput, throttleInput);
        carMovement.SetInput(movementInput);
    }

    async void Die()
    {
        health.OnDeath -= Die;
        
        if (dead) return;
        dead = true;

        Debug.Log("Player has died!");
        await GameManager.Instance.EndGame();
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
        throttleInput = value;
    }

    void HandleShoot(bool shoot)
    {
        shootInput = shoot;
    }
    #endregion
}