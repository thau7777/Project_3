using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool _isMagicAttack = false;
    [SerializeField]
    private OneShotVFXSettings _hitImpactVfx;
    [SerializeField]
    private int _damage = 40;
    private bool _dealTrueDamage = false;
    private float _knockBackForce;
    private bool _reverseKnockBackDirection = false;
    private ElementalType _elementalType;

    private GameObject _senderForParticle;

    private void Awake()
    {
        if(TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.OnColliderHit.AddListener(DealDamage);
        }
    }
    private void OnDestroy()
    {
        if (TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.OnColliderHit.RemoveListener(DealDamage);
        }
    }
    public void Setup(bool isMagicAttack, int damage, bool dealTrueDamage, float knockBackForce, bool reverseKnockBackDirection, ElementalType elementalType = ElementalType.Normal, OneShotVFXSettings hitImpactVfx = null)
    {
        _isMagicAttack = isMagicAttack;
        _damage = damage;
        _dealTrueDamage = dealTrueDamage;
        _knockBackForce = knockBackForce;
        _reverseKnockBackDirection = reverseKnockBackDirection;
        _elementalType = elementalType;
        _hitImpactVfx = hitImpactVfx;
    }
    public void SetupParicleDamageDealer(GameObject sender)
    {
        _senderForParticle = sender;
    }
    public void DealDamage(GameObject sender, GameObject hitOrigin, GameObject target)
    {
        if (target.TryGetComponent<Damageable>(out var targetDamageable))
        {
            if (targetDamageable.CurrentHealth == 0) return;
            Vector3 hitDirection = target.transform.position - hitOrigin.transform.position;

            //if player is parrying
            if(target.TryGetComponent<PlayerTopDownStateDriver>(out var player) && player.IsParrying && TryGetComponent<HitBoxHandler>(out var hitBoxHandler) && hitBoxHandler.ParryAble)
            {
                GetComponent<Flyweight>().ReturnToPool();
                if (sender.TryGetComponent<EnemyTopdownStateDriver>(out var enemy))
                {
                    enemy.OnTakeDamage(sender, 0, -hitDirection.normalized, 5);
                }
                if (sender.TryGetComponent<Damageable>(out var enemyDamageable) && enemyDamageable.floatingCombatTextSettings != null)
                {
                    FloatingCombatText floatingCombatTextNumber = FlyweightFactory.Spawn(enemyDamageable.floatingCombatTextSettings) as FloatingCombatText;
                    if (floatingCombatTextNumber)
                    {
                        FloatingCombatText.CombatTextType combatTextType = _elementalType switch
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

                        floatingCombatTextNumber.Init("Parried", combatTextType, player.transform.position.Add(y: 1), false);
                    }
                    
                }

                CharacterStats senderStats = sender.GetComponent<CharacterStats>();
                CharacterStats receiverStats = target.GetComponent<CharacterStats>();
                float parriedDamage = DamageCalculator.CalculateDamageByStats(senderStats, receiverStats, _isMagicAttack, _damage, _elementalType);

                bool isCrit = senderStats.CriticalRate > 0 && UnityEngine.Random.Range(0, 100) < senderStats.CriticalRate;
                parriedDamage = isCrit ? Mathf.RoundToInt(parriedDamage * senderStats.CriticalMultiplier) : Mathf.RoundToInt(parriedDamage);
                TopDownGameManager.Instance.AddParriedDamage(Mathf.RoundToInt(parriedDamage));
                TopDownGameManager.Instance.TriggerParryEffect();
                return;
            }

            float finalDamage = _damage;
            if (target.TryGetComponent<EnemyTopdownStateDriver>(out var enemyTopdownStateDriver) &&
                enemyTopdownStateDriver.TryGetComponent<Damageable>(out var damageable) &&
                damageable.hasShieldBreakingMechanic &&
                damageable.CurrentShieldHealth > 0 &&
                ElementalManager.Instance.IsStrongAgainst(_elementalType, damageable.GetComponent<CharacterStats>().ElementalType))
            {
                damageable.TakeShieldDamage(10); 
                if (enemyTopdownStateDriver.GetComponent<Damageable>().CurrentShieldHealth <= 0)
                    finalDamage *= 1.5f;
            }

            

            targetDamageable.TakeDamage(
                sender,
                hitOrigin,
                _isMagicAttack,
                Mathf.RoundToInt(finalDamage), 
                _dealTrueDamage, 
                _reverseKnockBackDirection ? -hitDirection.normalized : hitDirection.normalized,
                _knockBackForce, 
                _elementalType, 
                _hitImpactVfx); // Example damage value

            
        }
    }

    public void DealDamage(GameObject target)
    {
        DealDamage(_senderForParticle, gameObject, target);
    }
}
