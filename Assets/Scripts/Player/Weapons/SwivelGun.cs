using UnityEngine;
using UnityEngine.InputSystem;

class SwivelGun : Gun
{
    [SerializeField] float rotationSpeed = 360f; // Degrees per second

    float targetAngle = 0f;

    protected override void Update()
    {
        base.Update();

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
}
