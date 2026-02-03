using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
class CarMovement : MonoBehaviour
{
    [Header("Car Movement")]
    [SerializeField] float acceleration = 15f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float reverseMaxSpeed = 8f;
    [SerializeField] float turnSpeed = 200f;
    [SerializeField] float brakeForce = 25f;
    [SerializeField] float drag = 1f;
    [SerializeField] float lateralFriction = 5f; // Prevents sliding
    [SerializeField] float minSpeedForTurn = 0.3f;

    // Input vector: x = steering (-1 to 1), y = throttle/reverse (-1 to 1)
    Vector2 movementInput = Vector2.zero;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    public void SetMovementInput(Vector2 input)
    {
        movementInput = input;
    }

    void ApplyMovement()
    {
        // Get current velocity
        Vector2 velocity = rb.linearVelocity;
        float currentSpeed = velocity.magnitude;

        // Calculate forward direction
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        // Determine if we're moving forward or backward
        float forwardDot = Vector2.Dot(velocity.normalized, forward);
        bool movingForward = forwardDot > 0.1f;
        bool movingBackward = forwardDot < -0.1f;

        float steerInput = movementInput.x;
        float throttleInput = movementInput.y;

        // Apply throttle/reverse
        if (Mathf.Abs(throttleInput) > 0.1f)
        {
            if (throttleInput > 0) // Forward throttle
            {
                if (currentSpeed < maxSpeed)
                {
                    // More responsive acceleration at low speeds
                    float accelerationCurve = Mathf.Lerp(1.5f, 1f, currentSpeed / maxSpeed);
                    rb.AddForce(forward * acceleration * throttleInput * accelerationCurve, ForceMode2D.Force);
                }
            }
            else // Reverse
            {
                float reverseInput = -throttleInput;
                if (currentSpeed < reverseMaxSpeed || movingBackward)
                {
                    rb.AddForce(-forward * acceleration * reverseInput * 0.6f, ForceMode2D.Force);
                }
                else if (movingForward)
                {
                    // Apply stronger brake force when trying to reverse while moving forward
                    rb.AddForce(-velocity.normalized * brakeForce * reverseInput, ForceMode2D.Force);
                }
            }
        }
        else if (currentSpeed > 0.1f)
        {
            // Apply gentle braking when no input
            rb.AddForce(-velocity.normalized * brakeForce * 0.15f, ForceMode2D.Force);
        }

        // Apply steering (only when moving)
        if (Mathf.Abs(steerInput) > 0.1f && currentSpeed > minSpeedForTurn)
        {
            // Better steering curve: more responsive at mid speeds
            float steeringCurve = Mathf.Clamp01(currentSpeed / maxSpeed);
            steeringCurve = Mathf.Pow(steeringCurve, 0.6f); // Exponential curve for better feel
            
            // Reverse steering direction when moving backward
            float steeringDirection = movingBackward ? -1f : 1f;
            
            // Use torque for smoother, more realistic turning
            float torque = steerInput * turnSpeed * steeringCurve * steeringDirection;
            rb.AddTorque(-torque * Time.fixedDeltaTime, ForceMode2D.Force);
            
            // Dampen angular velocity to prevent over-rotation
            rb.angularVelocity *= 0.95f;
        }
        else
        {
            // Gradually reduce angular velocity instead of stopping instantly
            rb.angularVelocity *= 0.85f;
        }

        // Apply lateral friction (prevent sliding sideways)
        float lateralSpeed = Vector2.Dot(velocity, right);
        Vector2 lateralVelocity = right * lateralSpeed;
        Vector2 frictionForce = -lateralVelocity * lateralFriction;
        rb.AddForce(frictionForce, ForceMode2D.Force);

        // Apply forward drag (more realistic than lerp)
        float forwardSpeed = Vector2.Dot(velocity, forward);
        Vector2 forwardVelocity = forward * forwardSpeed;
        rb.AddForce(-forwardVelocity * drag * Time.fixedDeltaTime, ForceMode2D.Force);
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