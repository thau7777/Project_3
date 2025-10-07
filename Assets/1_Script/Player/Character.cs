using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using HSM;

public enum BattleState
{
    Waiting,
    Ready,
    Attacking,
    TakingDamage,
    Dead,
    Parrying,
    Interrupted
}

[System.Serializable]
public enum CharacterClass
{
    Sword_Shield,
    Magical,
    Summon,
    Enemy,


}

[System.Serializable]
public class CharacterStats
{
    public Sprite Avatar;
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int maxShield;
    public int currentShield;
    public int attack;
    public int defense;
    public int magicAttack;
    public int magicDefense;
    public int agility;
}

public class Character : MonoBehaviour
{
    public CharacterStateMachine stateMachine;

    public CharacterClass characterClass;

    [TabGroup("Class")] public List<CharacterClassProfile> allClassProfiles;

    [TabGroup("Stats")] public CharacterStats stats;

    [TabGroup("Skill")] public List<Skill> skills;


    public bool isPlayer;
    public Character target;
    public GameObject targetMarker;

    public Animator animator;

    public float actionGauge;


    public Vector3 initialPosition;
    public Quaternion initialRotation;

    public BattleManager battleManager;

    public bool isParryable;

    public PlayerActionUI ownUI;

    public Action OnAttackHitFrame;
    public bool isAttackReadyForParry = false;
    public bool isParryWindowFinished = false;
    public float parryWindowDuration = 0f;

    [Header("Attack Buff")]
    [HideInInspector] public int attackBuffTurnsRemaining = 0;
    [HideInInspector] private int originalBaseAttack = 0;

    [Header("Shield Buff")]
    [HideInInspector] public int shieldTurnsRemaining = 0; 
    [HideInInspector] private int baseShieldAmount = 0;

    [Header("MAX HP Buff")]
    [HideInInspector] private int originalBaseMaxHP = 0;
    [HideInInspector] public int maxHPBuffTurnsRemaining = 0;

    [Header("Defense Buff")]
    [HideInInspector] private int originalBaseDefense = 0;
    [HideInInspector] public int defenseBuffTurnsRemaining = 0;

    [Header("Original Buff")]
    [HideInInspector] private int originalBaseAgility = 0;
    [HideInInspector] public int agilityBuffTurnsRemaining = 0;



    public bool isAlive
    {
        get { return stats.currentHP > 0; }
    }


    void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        animator = GetComponent<Animator>();


        InitializeCharacterFrom(characterClass);

    }

    public void InitializeCharacterFrom(CharacterClass classTypeToLoad)
    {
        CharacterClassProfile targetProfile =
            allClassProfiles.FirstOrDefault(p => p.characterClass == classTypeToLoad);

        if (targetProfile == null)
        {
            Debug.LogWarning($"Không tìm thấy Class Profile cho lớp: {classTypeToLoad} trên {gameObject.name}!");

        }
        characterClass = targetProfile.characterClass;

        if (animator != null && targetProfile.animatorController != null)
        {
            animator.runtimeAnimatorController = targetProfile.animatorController;
        }

        if (targetProfile.initialSkills != null)
        {
            skills.Clear();
            skills.AddRange(targetProfile.initialSkills);
        }
    }

    public void UpdateOwnUI()
    {
        EnemyStatsUI uiComponent = GetComponentInChildren<EnemyStatsUI>();

        if (uiComponent != null)
        {
            uiComponent.UpdateUI();
        }
    }


    public void TakeDamage(int damageAmount)
    {
        int remainingDamage = damageAmount;

        if(stats.currentShield > 0)
        {
            int shieldAbsorb = Mathf.Min(stats.currentShield, remainingDamage);
            stats.currentShield -= shieldAbsorb;
            remainingDamage -= shieldAbsorb;
            Debug.Log(gameObject.name + " hấp thụ " + shieldAbsorb + " sát thương bằng lá chắn. Lá chắn còn lại: " + stats.currentShield);

        }
        if (remainingDamage > 0)
        {
            stats.currentHP -= remainingDamage;
            Debug.Log(gameObject.name + " nhận " + remainingDamage + " sát thương. Máu còn lại: " + stats.currentHP);
        }
        else if(damageAmount > 0)
        {
            Debug.Log(gameObject.name + " không nhận sát thương do lá chắn còn đủ.");
        }


        UpdateOwnUI();

        if(battleManager != null)
        {
            battleManager.UpdateCharacterUI(this);
        }
        if(stats.currentHP <= 0)
        {
            stats.currentHP = 0;
            Debug.Log($"{gameObject.name} đã chết!");
            stateMachine.SwitchState(stateMachine.deadState);
            if(battleManager != null)
            {
                battleManager.RemoveCombatant(this);
            }
        }
        else
        {
            if(damageAmount>0)
            {
                stateMachine.SwitchState(stateMachine.takingDamageState);
            }
        }
    }

    public void Heal(int amount)
    {
        if (!isAlive) return;

        stats.currentHP = Mathf.Min(stats.currentHP + amount, stats.maxHP);

        UpdateOwnUI();

        if (battleManager != null)
        {
            battleManager.UpdateCharacterUI(this);
        }

        Debug.Log($"{gameObject.name} hồi {amount} máu! Máu hiện tại: {stats.currentHP}");
    }

    public void AddShield(int amount)
    {
        if (amount <= 0) return;

        if (shieldTurnsRemaining <= 0)
        {
            baseShieldAmount = amount;
        }
        else
        {
            baseShieldAmount += amount;
        }

        shieldTurnsRemaining = 2;

        stats.currentShield = Mathf.Min(stats.currentShield + amount, stats.maxShield);

        Debug.Log(gameObject.name + " đã nhận thêm " + amount + " Shield. Shield hiện tại: " + stats.currentShield);

        if (battleManager != null)
        {
            battleManager.UpdateCharacterUI(this);
        }
    }

    public void ApplyAttackBuff(int amount, int duration)
    {
        if (amount <= 0 || duration <= 0) return;

        if (attackBuffTurnsRemaining <= 0)
        {
            originalBaseAttack = stats.attack;

            stats.attack += amount;
        }
        else
        {
            Debug.Log("Buff Attack đã được làm mới thời gian.");
        }

        attackBuffTurnsRemaining = duration;

        Debug.Log($"{gameObject.name} đã nhận buff +{amount} Attack, hiệu lực {duration} lượt. Attack hiện tại: {stats.attack}");
    }

    public void ApplyMaxHPBuff(int amount, int duration)
    {
        if (amount <= 0 || duration <= 0) return;

        if (maxHPBuffTurnsRemaining <= 0)
        {
            originalBaseMaxHP = stats.maxHP;

            stats.maxHP += amount;

            stats.currentHP += amount;
        }
        else
        {
            Debug.Log($"Buff MaxHP của {gameObject.name} đã được làm mới thời gian.");
        }

        maxHPBuffTurnsRemaining = duration;

        battleManager.UpdateCharacterUI(this);

        Debug.Log($"{gameObject.name} đã nhận buff +{amount} MaxHP, hiệu lực {duration} lượt. MaxHP hiện tại: {stats.maxHP}");
    }

    public void ApplyDefenseBuff(int amount, int duration)
    {
        if (amount <= 0 || duration <= 0) return;

        if (defenseBuffTurnsRemaining <= 0)
        {
            originalBaseDefense = stats.defense;
            stats.defense += amount;
        }
        else
        {
            Debug.Log($"Buff Defense của {gameObject.name} đã được làm mới thời gian.");
        }

        defenseBuffTurnsRemaining = duration;

        Debug.Log($"{gameObject.name} đã nhận buff +{amount} Defense, hiệu lực {duration} lượt. Defense hiện tại: {stats.defense}");
    }

    public void ApplyAgilityBuff(int amount, int duration)
    {
        if (amount <= 0 || duration <= 0) return;

        if (agilityBuffTurnsRemaining <= 0)
        {
            originalBaseAgility = stats.agility;
            stats.agility += amount;
        }
        else
        {
            Debug.Log($"Buff Agility của {gameObject.name} đã được làm mới thời gian.");
        }

        agilityBuffTurnsRemaining = duration;

        Debug.Log($"{gameObject.name} đã nhận buff +{amount} Agility, hiệu lực {duration} lượt. Agility hiện tại: {stats.agility}");
    }


    public void RemoveExpiredAttackBuff()
    {
        if (attackBuffTurnsRemaining > 0) return;

        stats.attack = originalBaseAttack;

        originalBaseAttack = 0;

        attackBuffTurnsRemaining = 0;

        Debug.Log($"Buff Attack của {gameObject.name} đã hết hạn và bị gỡ bỏ. Attack hiện tại: {stats.attack}");
    }

    public void RemoveExpiredShield()
    {
        if (shieldTurnsRemaining > 0) return;

        stats.currentShield = Mathf.Max(0, stats.currentShield - baseShieldAmount);

        baseShieldAmount = 0;
        shieldTurnsRemaining = 0;

        Debug.Log($"Shield của {gameObject.name} đã hết hạn và bị gỡ bỏ. Shield hiện tại: {stats.currentShield}");

        if (battleManager != null)
        {
            battleManager.UpdateCharacterUI(this);
        }
    }

    public void RemoveExpiredMaxHPBuff()
    {
        if (maxHPBuffTurnsRemaining > 0 || originalBaseMaxHP == 0) return;

        int oldMaxHP = stats.maxHP;
        stats.maxHP = originalBaseMaxHP;

        if (stats.currentHP > stats.maxHP)
        {
            stats.currentHP = stats.maxHP;
        }

        originalBaseMaxHP = 0;
    }


    public void RemoveExpiredDefenseBuff()
    {
        if (defenseBuffTurnsRemaining > 0 || originalBaseDefense == 0) return;

        stats.defense = originalBaseDefense;

        originalBaseDefense = 0;
        defenseBuffTurnsRemaining = 0;

        Debug.Log($"Buff Defense của {gameObject.name} đã hết hạn và bị gỡ bỏ. Defense hiện tại: {stats.defense}");
    }


    public void RemoveExpiredAgilityBuff()
    {
        if (agilityBuffTurnsRemaining > 0 || originalBaseAgility == 0) return;

        stats.agility = originalBaseAgility;

        originalBaseAgility = 0;
        agilityBuffTurnsRemaining = 0;

        Debug.Log($"Buff Agility của {gameObject.name} đã hết hạn và bị gỡ bỏ. Agility hiện tại: {stats.agility}");
    }





}