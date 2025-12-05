using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    CharacterController _characterController;

    [SerializeField]
    LayerMask _layerIgnoreOnDeath;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public UnityEvent<int, Vector3, float> OnTakeDamage;
    public UnityEvent<int> OnHeal;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    public void Initialize(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
    private void OnEnable()
    {
        ApplyIgnoreCollision(false);
    }
    public void TakeDamage(int damage, Vector3 knockBackDirection, float knockBackForce)
    {
        if (CurrentHealth == 0) return;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnTakeDamage?.Invoke(CurrentHealth, knockBackDirection, knockBackForce);
        if (CurrentHealth == 0) ApplyIgnoreCollision(true);
    }
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public void ApplyIgnoreCollision(bool ignore)
    {
        // Find all active colliders in the scene
        Collider[] allColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (var col in allColliders)
        {
            // Check if the collider's layer is one of the layers in the mask
            if (((1 << col.gameObject.layer) & _layerIgnoreOnDeath) != 0)
            {
                Physics.IgnoreCollision(_characterController, col, ignore);
            }
        }
    }
} 
