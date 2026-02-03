using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

    void HandleShoot(bool shoot)
    {
        shootInput = shoot;
    }
    #endregion
}