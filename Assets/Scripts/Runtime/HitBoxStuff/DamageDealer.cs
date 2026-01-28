using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    Transform _origin;

    // get the damage from the settings if we have it and put it here, if we dont have the settings setup then use the default
    [SerializeField]
    private float _damage = 40;
    public float Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public float KnockbackForce { get; set; }

    [field: SerializeField]
    public OneShotVFXSettings HitImpactEffect { get; private set; }

    public ElementalType ElementalType { get; set; }

    private void Awake()
    {
        _origin = transform.root;
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
            damageable.TakeDamage(sender,_damage, hitDirection.normalized, KnockbackForce, ElementalType, HitImpactEffect); // Example damage value

            
        }
    }
}
