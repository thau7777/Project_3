using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{

    [SerializeField]
    private LayerMask _layerIgnoreOnDeath;
    private CharacterControllerLayerIgnoreController _ccLayerIgnoreController;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    public bool hasShieldBreakingMechanic = false;
    public float MaxShieldHealth { get; private set; }
    public float ShieldHealth { get; private set; }
    public float InvincibleDuration { get; set; } = 0.1f;
    private float _invincibleElapsedTime = 0;

    private Coroutine _stunCoroutine;

    public UnityEvent<GameObject,float, Vector3, float> OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnShieldBreak;

    public StatusBarsUIController statusBarsUIController;
    public GameObject HealthBarUI;
    public GameObject ShieldBarUI;

    private void Awake()
    {
        _ccLayerIgnoreController = gameObject.GetOrAdd<CharacterControllerLayerIgnoreController>();
    }
    public void Initialize(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        if(hasShieldBreakingMechanic)
        {
            MaxShieldHealth = MaxHealth / 2f;
            ShieldHealth = MaxShieldHealth;
        }
        statusBarsUIController?.InitializeValue(CurrentHealth,MaxHealth,ShieldHealth, MaxShieldHealth);
        if (TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
        {
            if (HealthBarUI != null)
                HealthBarUI.SetActive(true);
            if (hasShieldBreakingMechanic && ShieldBarUI != null)
            {
                ShieldBarUI.SetActive(true);
            }
        }

    }
    private void OnEnable()
    {
        _invincibleElapsedTime = 0;
    }
    private void OnDisable()
    {
        if(_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
            _stunCoroutine = null;
        }

        if (TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
        {
            if (HealthBarUI != null)
            {
                HealthBarUI.SetActive(false);
            }
            if (hasShieldBreakingMechanic && ShieldBarUI != null)
            {
                ShieldBarUI.SetActive(false);
            }
        }
    }
    private void Update()
    {
        if(_invincibleElapsedTime > 0)
            _invincibleElapsedTime -= Time.deltaTime;
    }
    private void OnParticleCollision(GameObject other)
    {
        if(other.TryGetComponent<OneShotVFX>(out var oneShotVfx))
        {
            OneShotVFXSettings vfxSettings = oneShotVfx.settings as OneShotVFXSettings;
            if(vfxSettings.defaultDodgeLayers.Contains(gameObject.layer)) return;
        }
        //else if(other.TryGetComponent<ContinousVFX>(out var continousVFX))
        //{

        //}
        if (other.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            Vector3 hitDirection = transform.position - other.transform.position;
            damageDealer.DealDamage(other, gameObject);
        }
        if(other.TryGetComponent<EffectApplier>(out var effectApplier))
        {
            effectApplier.ApplyEffect(other, gameObject);
        }
    }
    public void TakeDamage(GameObject sender,float damage, Vector3 knockBackDirection, float knockBackForce, ElementalType attackType)
    {
        if (CurrentHealth == 0 || _invincibleElapsedTime > 0) return;
        float finalDamage = damage;

        if (TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
        {
            EnemyTopDownSettings enemySettings = enemy.settings as EnemyTopDownSettings;
            finalDamage = ElementalManager.Instance.CalculateDamage(damage, attackType, enemySettings.elementalType);
        }
        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0);
        statusBarsUIController?.UpdateHealth(CurrentHealth);
        if (hasShieldBreakingMechanic && ShieldHealth > 0)
        {
            ShieldHealth = Mathf.Max(ShieldHealth - finalDamage, 0);
            statusBarsUIController?.UpdateShield(ShieldHealth); 
            if (ShieldHealth == 0)
            {
                OnShieldBreak?.Invoke(3);
                StartStunCoroutine(3);
            }
        }
        if (CurrentHealth == 0)
        {
            ApplyIgnoreCollisionOnDeath(true);
            OnDeath?.Invoke();
            return;
        }

        OnTakeDamage?.Invoke(sender, CurrentHealth, knockBackDirection, knockBackForce);
        _invincibleElapsedTime = InvincibleDuration;

        
    }
    private void StartStunCoroutine(float duration)
    {
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        yield return Helpers.GetWaitForSeconds(duration);
        ShieldHealth = MaxShieldHealth;
        statusBarsUIController?.UpdateShield(ShieldHealth);
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        statusBarsUIController?.UpdateHealth(CurrentHealth);
    }

    public void ApplyIgnoreCollisionOnDeath(bool ignore)
    {
        if (ignore)
            _ccLayerIgnoreController.ApplyLayerIgnore(_layerIgnoreOnDeath);
        else
            _ccLayerIgnoreController.ResetLayerIgnore();
    }
} 
