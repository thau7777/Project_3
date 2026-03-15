using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{

    [SerializeField]
    private LayerMask _layerIgnoreOnDeath;
    private CCLayerIgnoreController _ccLayerIgnoreController;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; set; }

    public bool hasShieldBreakingMechanic = false;
    public float MaxShieldHealth { get; private set; }
    public float CurrentShieldHealth { get; private set; }


    [SerializeField] private float _invincibleDuration = 0.1f;
    public float InvincibleDuration => _invincibleDuration;
    private float _invincibleElapsedTime = 0;

    private Coroutine _stunCoroutine;

    public UnityEvent<GameObject,float, Vector3, float> OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnShieldBreak;

    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent<float, float> OnShieldChanged;

    public FloatingCombatTextSettings floatingCombatTextSettings;

    private void Awake()
    {
        _ccLayerIgnoreController = gameObject.GetOrAdd<CCLayerIgnoreController>();

    }
    public void Initialize(float currentHealth, float maxHealth, float shieldHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;

        if (!CompareTag("Player"))
            GetComponentInChildren<TopDownEnemyUIController>().InitializeValue(CurrentHealth, MaxHealth, shieldHealth, shieldHealth);

        if(hasShieldBreakingMechanic)
        {
            MaxShieldHealth = shieldHealth;
            CurrentShieldHealth = MaxShieldHealth;
            OnShieldChanged?.Invoke(CurrentShieldHealth, MaxShieldHealth);
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
            damageDealer.DealDamage(gameObject);
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
    public void TakeDamage(GameObject sender, GameObject hitOrigin, bool isMagicAttack, 
        int damage, bool dealTrueDamage, Vector3 knockBackDirection, float knockBackForce, 
        ElementalType attackElementalType, OneShotVFXSettings hitVfx = null, bool respectInvincibilityTime = true)
    {
        if (CurrentHealth == 0 || _invincibleElapsedTime > 0 && respectInvincibilityTime) return;
     
        FloatingCombatText floatingCombatTextEffect = null;
        if (floatingCombatTextSettings)
            floatingCombatTextEffect = FlyweightFactory.Spawn(floatingCombatTextSettings) as FloatingCombatText;

        var effectsManager = GetComponent<EffectsManager>();
        if (effectsManager.HasEffect("Unbreaking Thorn Effect"))
        {
            EffectData PoisonEffectData = new EffectData
            {
                effect = EffectsDatabase.Instance.GetEffectByName("Poison Effect"),
                stacksToApply = 1
            };
            sender.GetComponent<EffectsManager>()?.AddEffect(gameObject, PoisonEffectData);
            if (floatingCombatTextEffect) 
                floatingCombatTextEffect.Init("Unbreaking Thorn", FloatingCombatText.CombatTextType.Poison, sender.transform.position.Add(y: 1.5f), false);
        }
        if(effectsManager.HasEffect("Frost Shield Effect"))
        {
            EffectData effectData = new EffectData
            {
                effect = EffectsDatabase.Instance.GetEffectByName("Freeze Effect"),
                stacksToApply = 1
            };
            sender.GetComponent<EffectsManager>()?.AddEffect(gameObject, effectData);
            if(floatingCombatTextEffect)
                floatingCombatTextEffect.Init("Frost Shield", FloatingCombatText.CombatTextType.Frost, sender.transform.position.Add(y: 1), false);
             return;
        }
        if(effectsManager.HasEffect("Holy Shield Effect"))
        {
            effectsManager.RemoveEffectByName("Holy Shield Effect");
            CameraShaker.Instance.ShakeRandomDirection(force: 1, duration: 0.2f);

            if(floatingCombatTextEffect)
                floatingCombatTextEffect.Init("Holy Shield", FloatingCombatText.CombatTextType.Holy, transform.position.Add(y: 1), false);
            return;

        }
        CharacterStats senderStats = sender.GetComponent<CharacterStats>();
        CharacterStats receiverStats = GetComponent<CharacterStats>();
        float finalDamage = DamageCalculator.CalculateDamageByStats(senderStats, receiverStats, isMagicAttack, damage, attackElementalType);

        bool isCrit = senderStats.CriticalRate > 0 && UnityEngine.Random.Range(0, 100) < senderStats.CriticalRate;
        finalDamage = isCrit ? Mathf.RoundToInt(finalDamage * senderStats.CriticalMultiplier) : Mathf.RoundToInt(finalDamage);

        if (hitVfx)
        {
            var obj = FlyweightFactory.Spawn(hitVfx) as OneShotVFX;
            obj.FlyweightInitialize(transform.position.Add(y: 1));
            obj.InitializeVFX(hitVfx.DefaultSize, hitVfx.DefaultLifeTime);

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
                    effectsManager.AddEffect(gameObject, new EffectData
                    {
                        effect = activeEffect.effect,
                        stacksToApply = 1
                    });
                }
            }
        }
        finalDamage = Mathf.RoundToInt(finalDamage);
        CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if(!CompareTag("Player"))
            TopDownGameManager.Instance.AddDamageDealt((int)finalDamage);
        else
            TopDownGameManager.Instance.AddDamageReceived((int)finalDamage);

        FloatingCombatText floatingCombatTextNumber = FlyweightFactory.Spawn(floatingCombatTextSettings) as FloatingCombatText;
        if (floatingCombatTextNumber)
        {
            FloatingCombatText.CombatTextType combatTextType = attackElementalType switch
            {
                ElementalType.Fire => FloatingCombatText.CombatTextType.Fire,
                ElementalType.Water => FloatingCombatText.CombatTextType.Water,
                ElementalType.Frost => FloatingCombatText.CombatTextType.Frost,
                ElementalType.Lightning => FloatingCombatText.CombatTextType.Lightning,
                ElementalType.Poison => FloatingCombatText.CombatTextType.Poison,
                ElementalType.Holy => FloatingCombatText.CombatTextType.Holy,
                ElementalType.Dark => FloatingCombatText.CombatTextType.Dark,
                _ => FloatingCombatText.CombatTextType.Normal
            };

            floatingCombatTextNumber.Init(finalDamage.ToString(), combatTextType, transform.position.Add(y: 1), isCrit);
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
    public void TakeShieldDamage(float damage)
    {
        if (hasShieldBreakingMechanic && CurrentShieldHealth > 0)
        {
            var effectsManager = GetComponent<EffectsManager>();
            if (effectsManager.HasEffect("Poison Effect"))
                damage *= 2;
            

            CurrentShieldHealth = Mathf.Max(CurrentShieldHealth - damage, 0);
            OnShieldChanged?.Invoke(CurrentShieldHealth, MaxShieldHealth);
            if (CurrentShieldHealth == 0 && CurrentHealth > 0)
            {

                // spawn stun vfx
                var stunEffect = EffectsDatabase.Instance.GetEffectByName("Stun Effect");
                EffectData stunEffectData = new EffectData
                {
                    effect = stunEffect,
                    stacksToApply = 1
                };
                GetComponent<EffectsManager>().AddEffect(gameObject, stunEffectData);

                OnShieldBreak?.Invoke(stunEffect.durationOnApply);
                StartStunCoroutine(stunEffect.durationOnApply);
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
        CurrentShieldHealth = MaxShieldHealth;
        OnShieldChanged?.Invoke(CurrentShieldHealth, MaxShieldHealth);
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
