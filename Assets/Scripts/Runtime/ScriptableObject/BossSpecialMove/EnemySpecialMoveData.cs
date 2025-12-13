using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemySpecialMove", menuName = "Scriptable Objects/Enemy Special Move Data")]
public class EnemySpecialMoveData : ScriptableObject
{
    public enum LockMovementState
    {
        None,
        ChargeOnly,
        ExecutingOnly,
        Recovering,
        Full
    }
    public enum FaceTargetState
    {
        None,
        ChargeOnly,
        ExecutingOnly,
        Recovering,
        Full
    }
    [FoldoutGroup("Animation")]
    public string chargeAnimationName = "";
    [FoldoutGroup("Animation")]
    public string executeAnimationName = "Special_1";
    [FoldoutGroup("Animation")]
    public float crossfadeDuration = 0.1f;

    [FoldoutGroup("Movement")]
    public float movementSpeed = 0f;
    [FoldoutGroup("Movement")]
    public FaceTargetState faceTargetState = FaceTargetState.ChargeOnly;

    [FoldoutGroup("Timing")]
    public float chargeDuration = 1f;
    [FoldoutGroup("Timing")]
    public float executionDuration = 4f;
    [FoldoutGroup("Timing")]
    public float recoveryDuration = 1f;

    [FoldoutGroup("Attack Stuffs")]
    public float damageAmount = 10f;
    [FoldoutGroup("Attack Stuffs")]
    public LayerMask targetLayers;

    [FoldoutGroup("Skill Indicator Settings")]
    public SkillIndicatorSettings indicator;

    private bool _isCircleIndicator = false;

    [FoldoutGroup("Skill Indicator Settings")]
    [ShowIf("_isCircleIndicator")]
    public bool followSelf = true;
    [FoldoutGroup("Skill Indicator Settings")]
    public float indicatorWidth = 2f;
    [FoldoutGroup("Skill Indicator Settings")]
    public float indicatorLength = 2f;

    [FoldoutGroup("Effects")]
    public FlyweightSettings skillEffect;
    [FoldoutGroup("Effects")]
    public float skillEffectSize;
    [FoldoutGroup("Effects")]
    public AdvanceOneShotVFXSettings chargeEffect;
    [FoldoutGroup("Effects")]
    public float chargeEffectSize;
    [FoldoutGroup("Effects")]
    public AudioClip attackSound;

    [FoldoutGroup("Special Behavior")]
    public bool applyKnockback = false;
    [FoldoutGroup("Special Behavior")]
    public float knockbackForce = 5f;

    public float GetTotalDuration()
    {
        return chargeDuration + executionDuration + recoveryDuration;
    }
    public void OnValidate()
    {
        _isCircleIndicator = indicator && indicator.type == FlyweightType.IndicatorCircleEnemy;
    }
}
