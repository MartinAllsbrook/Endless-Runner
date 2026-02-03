using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Serialized fields
    [SerializeField] float maxHealth = 100f;

    // Private fields
    float currentHealth;

    // Events
    public event Action<float> OnHealthChanged = delegate { };
    public event Action<float> OnHealthChangedPercent = delegate { };
    public event Action OnDeath = delegate { };

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        OnHealthChanged.Invoke(currentHealth);
        OnHealthChangedPercent.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void IncreaseHealth(float amount)
    {
        SetHealth(currentHealth + amount);
    }

    public void DecreaseHealth(float amount)
    {
        SetHealth(currentHealth - amount);
    }

    public void ResetHealth()
    {
        SetHealth(maxHealth);
    }

    void Die()
    {
        OnDeath.Invoke();
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
}
