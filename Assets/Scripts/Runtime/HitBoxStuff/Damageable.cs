using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{

    [SerializeField]
    private LayerMask _layerIgnoreOnDeath;
    private CharacterControllerLayerIgnoreController _ccLayerIgnoreController;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; set; }

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
            MaxShieldHealth = 100;
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
        if (TryGetComponentInHierarchy<DamageDealer>(other.transform, out var damageDealer))
        {
            damageDealer.DealDamage(other, other, gameObject);
        }

        if (TryGetComponentInHierarchy<EffectApplier>(other.transform, out var effectApplier))
        {
            effectApplier.ApplyEffect(other, other, gameObject);
        }
    }

    private bool TryGetComponentInHierarchy<T>(Transform start, out T component) where T : Component
    {
        Transform current = start;

        while (current != null)
        {
            if (current.TryGetComponent<T>(out component))
                return true;

            current = current.parent;
        }

        component = null;
        return false;
    }
    public void TakeDamage(GameObject sender, GameObject hitOrigin, float damage, bool dealTrueDamage, Vector3 knockBackDirection, float knockBackForce, ElementalType attackElementalType, OneShotVFXSettings hitVfx = null, bool respectInvincibilityTime = true)
    {
        if (CurrentHealth == 0 || _invincibleElapsedTime > 0 && respectInvincibilityTime) return;
     
        var effectsManager = GetComponent<EffectsManager>();
        if (effectsManager.HasEffect("Unbreaking Thorn Effect"))
        {
            EffectData PoisonEffectData = new EffectData
            {
                effect = EffectsDatabase.Instance.GetEffectByName("Poison Effect"),
                stacksToApply = 1
            };
            sender.GetComponent<EffectsManager>()?.AddEffect(PoisonEffectData);
        }
        if(effectsManager.HasEffect("Frost Shield Effect"))
        {
            EffectData effectData = new EffectData
            {
                effect = EffectsDatabase.Instance.GetEffectByName("Freeze Effect"),
                stacksToApply = 1
            };
            sender.GetComponent<EffectsManager>()?.AddEffect(effectData);
        }
        if(effectsManager.HasEffect("Holy Shield Effect"))
        {
            effectsManager.RemoveEffectByName("Holy Shield Effect");
            CameraShaker.Instance.ShakeRandomDirection(force: 1, duration: 0.2f);
            return;

        }
        float finalDamage = damage;

        if (hitVfx)
        {
            var obj = FlyweightFactory.Spawn(hitVfx) as OneShotVFX;
            obj.FlyweightInitialize(transform.position.Add(y: 1));
            obj.InitializeVFX(hitVfx.DefaultSize, hitVfx.DefaultLifeTime);

        }
        if (TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
        {
            ElementalType enemyElementalType = enemy.GetComponent<CharacterStats>().ElementalType;
            finalDamage = ElementalManager.Instance.CalculateDamage(damage, attackElementalType, enemyElementalType);
        }

        if (hitOrigin != null && hitOrigin.name == "Spell_Dark_3" &&
            effectsManager.HasAnyActiveEffect())
        {
            finalDamage *= 2;
            List<ActiveEffect> activeEffects = effectsManager.GetActiveEffectsList();
            ActiveEffect randomEffect = activeEffects[UnityEngine.Random.Range(0, activeEffects.Count)];
            effectsManager.RemoveEffect(randomEffect);
        }else if(hitOrigin != null && hitOrigin.name == "Spell_Dark_4")
        {
            List<ActiveEffect> activeEffects = effectsManager.GetActiveEffectsList();
            if (activeEffects.Count != 0)
            {
                foreach (var activeEffect in activeEffects)
                {
                    effectsManager.AddEffect(new EffectData
                    {
                        effect = activeEffect.effect,
                        stacksToApply = 1
                    });
                }
            }
        }

        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            ApplyIgnoreCollisionOnDeath(true);
            OnDeath?.Invoke();
            return;
        }

        OnTakeDamage?.Invoke(sender, CurrentHealth, knockBackDirection, knockBackForce);
        _invincibleElapsedTime = InvincibleDuration;

        
    }
    public void TakeShieldDamage(float damage)
    {
        if (hasShieldBreakingMechanic && ShieldHealth > 0)
        {
            var effectsManager = GetComponent<EffectsManager>();
            if (effectsManager.HasEffect("Poison Effect"))
                damage *= 2;
            

            ShieldHealth = Mathf.Max(ShieldHealth - damage, 0);
            OnShieldChanged?.Invoke(ShieldHealth, MaxShieldHealth);
            if (ShieldHealth == 0 && CurrentHealth > 0)
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
        Debug.Log($"Healing for {amount}");
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
