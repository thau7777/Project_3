using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public Transform Origin { get; set; }
    // get the damage from the settings if we have it and put it here, if we dont have the settings setup then use the default
    [SerializeField]
    private float _damage = 40;
    public float Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public float KnockbackForce { get; set; }
    public bool ReverseKnockbackDirection { get; set; } = false;

    [field: SerializeField]
    public OneShotVFXSettings HitImpactEffect { get; private set; }

    public ElementalType ElementalType { get; set; }

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

    public void SetHitImpactVFX(OneShotVFXSettings vfxSetting)
    {
        HitImpactEffect = vfxSetting;
    }
    public void DealDamage(GameObject sender, GameObject target)
    {
        if (target.TryGetComponent<Damageable>(out var damageable))
        {
            if (damageable.CurrentHealth == 0) return;
            Vector3 hitDirection = target.transform.position - sender.transform.position;

            if(target.TryGetComponent<PlayerTopDownStateDriver>(out var player) && player.IsParrying && GetComponent<HitBoxHandler>().Parryable)
            {
                GetComponent<Flyweight>().ReturnToPool();

                if (sender.TryGetComponent<Damageable>(out var enemy))
                    enemy.TakeDamage(sender, 0, -hitDirection.normalized, 15, ElementalType, HitImpactEffect);

                return;
            }


            damageable.TakeDamage(sender,_damage, ReverseKnockbackDirection ? -hitDirection.normalized : hitDirection.normalized, KnockbackForce, ElementalType, HitImpactEffect); // Example damage value

            
        }
    }
}
