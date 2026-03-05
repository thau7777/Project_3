using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // get the damage from the settings if we have it and put it here, if we dont have the settings setup then use the default
    [SerializeField]
    private OneShotVFXSettings _hitImpactVfx;
    [SerializeField]
    private float _damage = 40;
    private bool _dealTrueDamage = false;
    private float _knockBackForce;
    private bool _reverseKnockBackDirection = false;
    private ElementalType _elementalType;

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
    public void Setup(float damage, bool dealTrueDamage, float knockBackForce, bool reverseKnockBackDirection, ElementalType elementalType = ElementalType.Normal, OneShotVFXSettings hitImpactVfx = null)
    {
        _damage = damage;
        _dealTrueDamage = dealTrueDamage;
        _knockBackForce = knockBackForce;
        _reverseKnockBackDirection = reverseKnockBackDirection;
        _elementalType = elementalType;
        _hitImpactVfx = hitImpactVfx;
    }
    public void DealDamage(GameObject sender, GameObject hitOrigin, GameObject target)
    {
        if (target.TryGetComponent<Damageable>(out var targetDamageable))
        {
            if (targetDamageable.CurrentHealth == 0) return;
            Vector3 hitDirection = target.transform.position - hitOrigin.transform.position;

            //if player is parrying
            if(target.TryGetComponent<PlayerTopDownStateDriver>(out var player) && player.IsParrying && GetComponent<HitBoxHandler>().ParryAble)
            {
                GetComponent<Flyweight>().ReturnToPool();

                if (sender.TryGetComponent<Damageable>(out var enemy))
                    enemy.TakeDamage(sender, hitOrigin, 0, _dealTrueDamage, -hitDirection.normalized, 5, _elementalType, _hitImpactVfx);

                return;
            }
            targetDamageable.TakeDamage(
                sender,
                hitOrigin, 
                _damage, 
                _dealTrueDamage, 
                _reverseKnockBackDirection ? -hitDirection.normalized : hitDirection.normalized,
                _knockBackForce, 
                _elementalType, 
                _hitImpactVfx); // Example damage value

            if(target.TryGetComponent<EnemyTopdownStateDriver>(out var enemyTopdownStateDriver) && 
                enemyTopdownStateDriver.TryGetComponent<Damageable>(out var damageable))
            {
                if(ElementalManager.Instance.IsStrongAgainst(_elementalType, damageable.GetComponent<CharacterStats>().ElementalType))
                {
                    damageable.TakeShieldDamage(10);
                }
            }
        }
    }
}
