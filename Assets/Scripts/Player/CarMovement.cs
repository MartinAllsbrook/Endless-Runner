using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
class CarMovement : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] float driftFactor = 0.95f;
    [SerializeField] float acceleration = 10000f;
    [SerializeField] float turnFactor = 5f;
    [SerializeField] float speedSteeringFactor = 16f;
    [SerializeField] float rollingResistance = 2000f;
    [SerializeField] float maxSpeedKPH = 100f;

    float maxSpeed => maxSpeedKPH / 3.6f; // Convert km/h to m/s
    float accelerationInput = 0f;
    float steerInput = 0f;

    float forwardSpeed = 0f;
    float rotationAngle = 0f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        forwardSpeed = Vector2.Dot(transform.up, rb.linearVelocity);

        ApplyEngineForce();
        ApplyRollingResistance();
        ApplySteering();
        KillLateralVelocity();
    }

    void ApplyEngineForce()
    {
        if (forwardSpeed > maxSpeed && accelerationInput > 0f)
            return;

        if (forwardSpeed < -maxSpeed * 0.5f && accelerationInput < 0f)
            return;

        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0f)
            return;

        if (forwardSpeed > maxSpeed * 0.5f && accelerationInput > 0f)
        {
            // Apply reduced force when above half max speed
            float reducedAcceleration = acceleration * (1f - (forwardSpeed - maxSpeed * 0.5f) / (maxSpeed * 0.5f));
            Vector2 engineForce = transform.up * accelerationInput * reducedAcceleration;
            rb.AddForce(engineForce, ForceMode2D.Force);
            return;
        } 
        else
        {
            Vector2 engineForce = transform.up * accelerationInput * acceleration;
            rb.AddForce(engineForce, ForceMode2D.Force);
        }
    }

    void ApplyRollingResistance()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (forwardSpeed > 0f)
        {
            Vector2 reverseForce = -transform.up * rollingResistance;
            rb.AddForce(reverseForce, ForceMode2D.Force);
        } 
        else if (forwardSpeed < 0f)
        {
            Vector2 forwardForce = transform.up * rollingResistance;
            rb.AddForce(forwardForce, ForceMode2D.Force);
        }

    }

    void ApplySteering()
    {
        float speedFactor = Mathf.Clamp01(GetCurrentSpeed() / speedSteeringFactor);

        rotationAngle -= steerInput * turnFactor * speedFactor;
        rb.MoveRotation(rotationAngle); // Directly set rotation
    }

    void KillLateralVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

        rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }
        

    public void SetInput(Vector2 input)
    {
        steerInput = input.x;
        accelerationInput = input.y;
    }

    float GetCurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    public float GetCurrentSpeedKPH()
    {
        return GetCurrentSpeed() * 3.6f; // Convert m/s to km/h
    }
}