using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
class CollidableObject : MonoBehaviour
{
    [Tooltip("How resistant this object is to impacts")]
    [SerializeField] float impactResistance = 1f;

    const float baseImpactResistance = 1000f;

    Health health;
    Rigidbody2D rb;

    void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        float impactStrength = SumImpactStrength(collision);
        float impactDamage = impactStrength / (impactResistance * baseImpactResistance);
        health.DecreaseHealth(impactDamage);
    }

    float SumImpactStrength(Collision2D collision)
    {
        float impactStrength = 0f;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            float contactImpact = contact.normalImpulse / Time.fixedDeltaTime;
            if (contactImpact > impactStrength)
            {
                impactStrength = contactImpact;
            }
        }
        return impactStrength;
    }
}