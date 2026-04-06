using UnityEngine;

[System.Serializable]
public class EnemyAttackData
{
    public enum LockMovementState
    {
        None,
        ChargeOnly,
        ExecutingOnly,
        Full
    }
    public enum FaceTargetState
    {
        None,
        ChargeOnly,
        ExecutingOnly,
        Full
    }
    public enum SpawnType
    {
        AtSelf,
        AtTarget,
        AtCustomSpawnTransform
    }
    public bool Parryable = true;

    [Header("Spawn Settings")]
    public SpawnType spawnType = SpawnType.AtSelf;
    public Transform skillSpawnTransform;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    [Header("Basic Stuffs")]
    public LayerMask dodgeLayers;
    public float coolDown = 2f;
    public float weight;
    public float minRange;
    public float maxRange;
    public int damage = 10;
    public bool dealTrueDamage;
    public bool reverseKnockbackDirection;
    public float knockBackForce = 10f;
    public float stunTime = 0f;


    [Header("Execute Settings")]
    public bool forPhaseTwo;
    public string executeAnimationName = "Attack1";
    public float crossfadeDuration = 0.1f;
    public FlyweightSettings skillVFX;
    public float projectileRange = 1f;
    public float projectileSpeed = 10;
    public float skillSize = 1;
    public AudioClip executionSound;

    [Header("Movement")]
    public float movementSpeed = 0f;
    public float rotateSpeed = 10;
    public FaceTargetState faceTargetState = FaceTargetState.Full;
    public LockMovementState lockMovementState = LockMovementState.Full;

    [Header("Charge Settings")]
    [Tooltip("If false then it won't need these settings below")]
    public bool needCharge;
    public string chargeAnimationName = "";
    public Transform chargeSpawnTransform;
    public OneShotVFXSettings chargeEffect;
    public float chargeEffectSize;
    public float chargeDuration;


    [Header("Skill Indicator Settings")]
    public SkillIndicatorSettings indicator;
    public float indicatorWidth = 2f;
    public float indicatorLength = 2f;

}

