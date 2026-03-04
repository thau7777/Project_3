using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum EnemyTopdownMovementType
{
    Slime,
    Normal,
    Range
}
[System.Serializable]
public class EnemyTopdownContext    
{
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public NavMeshSteering NavMeshSteering { get; private set; }
    public Transform RootTransform { get; private set; }
    [field: SerializeField] public Transform CurrentTargetTransform { get; private set; }
    public float BaseMoveSpeed { get; set; }
    public float BaseRotateSpeed { get; private set; }
    public float MovePauseDuration { get; private set; }
    [field: SerializeField]
    public float CurrentSpeed { get; set; }
    public float RotateSpeed { get; set; }
    [field: SerializeField]
    public Vector3 MoveDir { get; set; }

    public Vector3 KnockbackDirection { get; set; }
    public float KnockbackForce { get; set; }
    public float MaxAttackRange { get; private set; }
    public bool HasSpecialSkill { get; private set; }
    public float StunDuration { get; set; }
    public float MinRangeDistance { get; set; }
    public float MaxRangeDistance { get; set; }
    public bool ForceStopFacingTarget { get; set; }

    public Transform PlayerTransform { get; private set; }

    // State Properties
    [field: SerializeField] public bool IsSpawning { get; set; }
    [field: SerializeField] public bool IsMoving { get; set; }
    [field: SerializeField] public bool IsCharging { get; set; }
    [field: SerializeField] public bool IsAttacking { get; set; }
    [field: SerializeField] public bool IsHurting { get; set; }
    [field: SerializeField] public bool IsMoreHurt { get; set; }
    [field: SerializeField] public bool IsStunned { get; set; }
    [field: SerializeField] public bool IsDead { get; set; }

    // Cached Animator Hashes
    public int IdleHash => Animator.StringToHash("Idle");
    public int MoveHash => Animator.StringToHash("Move");
    public int HurtHash => Animator.StringToHash("Hurt");
    public int StunnedHash => Animator.StringToHash("Stunned");
    public int DeadHash => Animator.StringToHash("Dead");

    public EnemyTopdownMovementType EnemyType { get; set; }
    public List<EnemyAttackData> EnemyAttackDataList { get; private set; }
    public EnemyAttackData CurrentEnemyAttackData { get; set; }
    public float DistanceToTarget => Vector3.Distance(CurrentTargetTransform.position, RootTransform.position);
    private Dictionary<int, float> attackCooldowns = new Dictionary<int, float>();
    private int lastUsedAttackIndex = -1;
    public bool IsTargetInMaxAttackRange()
    {
        var distanceToTarget = Vector3.Distance(CurrentTargetTransform.position, RootTransform.position);
        return distanceToTarget <= MaxAttackRange;
    }

    public void SetCurrentTarget(Transform target)
    {
        CurrentTargetTransform = target;
    }
    public bool CheckAndPickRandomAttack()
    {
        if (EnemyAttackDataList.Count == 0) return false;

        // Build list of valid attacks with their weights
        List<int> validIndices = new List<int>();
        List<float> weights = new List<float>();

        for (int i = 0; i < EnemyAttackDataList.Count; i++)
        {
            var attack = EnemyAttackDataList[i];

            // Filter by distance
            if (DistanceToTarget < attack.minRange || DistanceToTarget > attack.maxRange)
                continue;

            // Check cooldown
            if (attackCooldowns.ContainsKey(i) && Time.time < attackCooldowns[i])
                continue;

            // Avoid immediate repetition - reduce weight heavily
            float finalWeight = attack.weight;
            if (i == lastUsedAttackIndex)
                finalWeight *= 0.1f; // Or set to 0 to completely prevent repetition

            validIndices.Add(i);
            weights.Add(finalWeight);
        }

        // If no valid attacks, return false
        if (validIndices.Count == 0) return false;

        // Weighted random selection
        int selectedIndex = GetWeightedRandomIndex(weights);
        int attackIndex = validIndices[selectedIndex];

        // Set current attack and update tracking
        CurrentEnemyAttackData = EnemyAttackDataList[attackIndex];
        lastUsedAttackIndex = attackIndex;
        attackCooldowns[attackIndex] = Time.time + CurrentEnemyAttackData.coolDown;

        switch (CurrentEnemyAttackData.spawnType)
        {
            case EnemyAttackData.SpawnType.AtSelf:
                CurrentEnemyAttackData.skillSpawnTransform = RootTransform;
                break;
            case EnemyAttackData.SpawnType.AtTarget:
                CurrentEnemyAttackData.skillSpawnTransform = CurrentTargetTransform;
                break;
            default:
                break;
        }
        return true;
    }
    private int GetWeightedRandomIndex(List<float> weights)
    {
        float totalWeight = 0;
        foreach (float weight in weights)
            totalWeight += weight;

        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
                return i;
        }

        return weights.Count - 1;
    }
    

    private EnemyTopdownContext() { }

    // -----------------------------
    // Builder
    // -----------------------------
    public class Builder
    {
        private readonly EnemyTopdownContext ctx = new EnemyTopdownContext();

        public Builder SetAnimator(Animator animator)
        {
            ctx.Animator = animator;
            return this;
        }
        
        public Builder SetCharacterController(CharacterController controller)
        {
            ctx.CharacterController = controller;
            return this;
        }

        public Builder SetNavMeshSteering(NavMeshSteering steering)
        {
            ctx.NavMeshSteering = steering;
            return this;
        }

        public Builder SetRootTransform(Transform root)
        {
            ctx.RootTransform = root;
            return this;
        }

        public Builder SetPlayerTransform(Transform target)
        {
            ctx.PlayerTransform = target;
            ctx.SetCurrentTarget(target);
            return this;
        }

        public Builder SetMoveSpeed(float distance)
        {
            ctx.BaseMoveSpeed = distance;
            return this;
        }

        public Builder SetMovePauseDuration(float duration)
        {
            ctx.MovePauseDuration = duration;
            return this;
        }

        public Builder SetRotateSpeed(float speed)
        {
            ctx.BaseRotateSpeed = speed;
            ctx.RotateSpeed = speed;
            return this;
        }

        public Builder SetEnemyType(EnemyTopdownMovementType type)
        {
            ctx.EnemyType = type;
            return this;
        }
        public Builder SetHasSpecialSkill(bool value)
        {
            ctx.HasSpecialSkill = value;
            return this;
        }
        public Builder SetEnemyAttackList(List<EnemyAttackData> attacks)
        {
            ctx.EnemyAttackDataList = attacks;

            if (attacks != null && attacks.Count > 0)
            {
                ctx.MaxAttackRange = attacks.Max(a => a.maxRange);
            }
            else
            {
                ctx.MaxAttackRange = 0f; // Default value
            }
            return this;
        }
        // set this after the attack list is set
        
        public EnemyTopdownContext Build()
        {
            // Optional: validate required fields
            if (ctx.Animator == null)
                Debug.LogWarning("SmallEnemyTopdownContext built without Animator.");
            if (ctx.RootTransform == null)
                Debug.LogWarning("SmallEnemyTopdownContext built without RootTransform.");

            return ctx;
        }
    }
}
