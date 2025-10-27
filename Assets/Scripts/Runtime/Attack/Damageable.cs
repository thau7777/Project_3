using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public UnityEvent<int, Vector3, float> OnTakeDamage;
    public UnityEvent<int> OnHeal;

    public void Initialize(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector3 knockBackDirection, float knockBackForce)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnTakeDamage?.Invoke(CurrentHealth, knockBackDirection, knockBackForce);
    }
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
} 
