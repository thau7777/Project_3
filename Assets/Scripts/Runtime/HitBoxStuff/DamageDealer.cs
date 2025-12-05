using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    Transform _origin;

    // get the damage from the settings if we have it and put it here, if we dont have the settings setup then use the default
    [SerializeField]
    private int _damage = 40;
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    [SerializeField]
    private float _knockbackForce = 10f;
    public float KnockbackForce
    {
        get => _knockbackForce;
        set { _knockbackForce = value; }
    }

    [field: SerializeField]
    public OneShotVFXSettings HitImpactEffect { get; private set; }


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
    public void DealDamage(GameObject target)
    {
        if (target.TryGetComponent<Damageable>(out var damageable))
        {
            if (damageable.CurrentHealth == 0) return;
            Vector3 hitDirection = target.transform.position - _origin.position;
            damageable.TakeDamage(_damage, hitDirection.normalized, _knockbackForce); // Example damage value

            if (HitImpactEffect)
                FlyweightFactory.Spawn(HitImpactEffect).transform.position = target.transform.position.Add(y: 1);
        }
    }
}
