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


    [SerializeField] private float _invincibleDuration = 0.1f;
    public float InvincibleDuration => _invincibleDuration;
    private float _invincibleElapsedTime = 0;

    private Coroutine _stunCoroutine;
    [SerializeField]
    private ContinousVFXSettings _stunVFXSettings;
    private Transform _stunVFXSpawnTransform;
    private Flyweight _stunVfxFlyweight;

    public UnityEvent<GameObject,float, Vector3, float> OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnShieldBreak;

    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent<float, float> OnShieldChanged;


    private void Awake()
    {
        _ccLayerIgnoreController = gameObject.GetOrAdd<CharacterControllerLayerIgnoreController>();
        if(transform.tag != "Player")
        _stunVFXSpawnTransform = transform.Find("AboveHead") ?? transform;
    }
    public void Initialize(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        if(hasShieldBreakingMechanic)
        {
            MaxShieldHealth = MaxHealth / 2f;
            ShieldHealth = MaxShieldHealth;
            OnShieldChanged?.Invoke(ShieldHealth, MaxShieldHealth);
        }

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

    }
    public void UpdateMaxHealth(float newMaxHealth)
    {
        MaxHealth = newMaxHealth;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
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

    }
    private void Update()
    {
        if(_invincibleElapsedTime > 0)
            _invincibleElapsedTime -= Time.deltaTime;
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.root.TryGetComponent<OneShotVFX>(out var oneShotVfx))
        {
            OneShotVFXSettings vfxSettings = oneShotVfx.settings as OneShotVFXSettings;
            if(vfxSettings.defaultDodgeLayers.Contains(gameObject.layer)) return;
        }
        if (other.transform.root.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            Vector3 hitDirection = transform.position - other.transform.position;
            damageDealer.DealDamage(other, gameObject);
        }
        if(other.transform.root.TryGetComponent<EffectApplier>(out var effectApplier))
        {
            effectApplier.ApplyEffect(other, gameObject);
        }
    }
    public void TakeDamage(GameObject sender,float damage, Vector3 knockBackDirection, float knockBackForce, ElementalType attackType, OneShotVFXSettings hitVfx = null, bool respectInvincibilityTime = true)
    {
        if (CurrentHealth == 0 || _invincibleElapsedTime > 0 && respectInvincibilityTime) return;
        float finalDamage = damage;
        if (hitVfx)
        {
            var obj = FlyweightFactory.Spawn(hitVfx) as OneShotVFX;
            obj.FlyweightInitialize(transform.position.Add(y: 1));
            obj.InitializeVFX(hitVfx.DefaultSize, hitVfx.DefaultLifeTime);

        }
        if (TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
        {
            EnemyTopDownSettings enemySettings = enemy.settings as EnemyTopDownSettings;
            finalDamage = ElementalManager.Instance.CalculateDamage(damage, attackType, enemySettings.elementalType);
        }
        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (hasShieldBreakingMechanic && ShieldHealth > 0)
        {
            ShieldHealth = Mathf.Max(ShieldHealth - finalDamage, 0);
            OnShieldChanged?.Invoke(ShieldHealth, MaxShieldHealth);
            if (ShieldHealth == 0)
            {
                OnShieldBreak?.Invoke(3);
                StartStunCoroutine(3);

                if (_stunVFXSettings != null && _stunVFXSpawnTransform != null)
                {
                    _stunVfxFlyweight = FlyweightFactory.Spawn(_stunVFXSettings);
                    _stunVfxFlyweight.FlyweightInitialize(_stunVFXSpawnTransform.position, parent: _stunVFXSpawnTransform);
                    (_stunVfxFlyweight as ContinousVFX).InitializeVFX(_stunVFXSettings.DefaultSize);
                    _stunVfxFlyweight.transform.position = _stunVFXSpawnTransform.position;
                    _stunVfxFlyweight.transform.rotation = Quaternion.identity;
                }
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
        if (_stunVfxFlyweight)
        {
            _stunVfxFlyweight.ReturnToPool();
            _stunVfxFlyweight = null;
        }
        ShieldHealth = MaxShieldHealth;
        OnShieldChanged?.Invoke(ShieldHealth, MaxShieldHealth);
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void ApplyIgnoreCollisionOnDeath(bool ignore)
    {
        if (ignore)
            _ccLayerIgnoreController.ApplyLayerIgnore(_layerIgnoreOnDeath);
        else
            _ccLayerIgnoreController.ResetLayerIgnore();
    }
} 
