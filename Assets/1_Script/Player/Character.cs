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
        stats.currentHP -= damageAmount;
        Debug.Log(gameObject.name + " đã nhận " + damageAmount + " sát thương. Máu còn lại: " + stats.currentHP);


        UpdateOwnUI();


        if (battleManager != null)
        {
            battleManager.UpdateCharacterUI(this);
        }

        if (stats.currentHP <= 0)
        {
            stats.currentHP = 0;
            stateMachine.SwitchState(stateMachine.deadState);

            if (battleManager != null)
            {
                battleManager.RemoveCombatant(this);

            }
        }
        else
        {
            stateMachine.SwitchState(stateMachine.takingDamageState);
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



}