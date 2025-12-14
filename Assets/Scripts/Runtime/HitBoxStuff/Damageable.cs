using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    CharacterController _characterController;

    [SerializeField]
    private LayerMask _layerIgnoreOnDeath;
    private CharacterControllerLayerIgnoreController _ccLayerIgnoreController;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public float InvincibleDuration { get; set; } = 0.1f;
    private float _invincibleElapsedTime = 0;

    public UnityEvent<GameObject,int, Vector3, float> OnTakeDamage;
    public UnityEvent<int> OnHeal;
    public UnityEvent OnDeath;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _ccLayerIgnoreController = gameObject.GetOrAdd<CharacterControllerLayerIgnoreController>();
    }
    public void Initialize(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
    private void OnEnable()
    {
        _invincibleElapsedTime = 0;
    }
    private void Update()
    {
        if(_invincibleElapsedTime > 0)
            _invincibleElapsedTime -= Time.deltaTime;
    }
    public void TakeDamage(GameObject sender,int damage, Vector3 knockBackDirection, float knockBackForce)
    {
        if (CurrentHealth == 0 || _invincibleElapsedTime > 0) return;

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (CurrentHealth == 0)
        {
            ApplyIgnoreCollisionOnDeath(true);
            OnDeath?.Invoke();
            return;
        }

        _invincibleElapsedTime = InvincibleDuration;
        OnTakeDamage?.Invoke(sender,CurrentHealth, knockBackDirection, knockBackForce);
        
    }
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public void ApplyIgnoreCollisionOnDeath(bool ignore)
    {
        if (ignore)
            _ccLayerIgnoreController.ApplyLayerIgnore(_layerIgnoreOnDeath);
        else
            _ccLayerIgnoreController.ResetLayerIgnore();
    }
} 
