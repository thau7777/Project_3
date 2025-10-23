using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using HSM;


namespace Turnbase
{
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
        Tank,
        Enemy,


    }

    [System.Serializable]
    public enum CharacterElement
    {
        None,
        Fire,
        Water,
        Earth,
        Light,
        Dark
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

        public CharacterElement characterElement;

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

        public CharacterBuffManager buffManager;

        public bool isParryable;

        public PlayerActionUI ownUI;

        public Action OnAttackHitFrame;
        private Action damageCallback;

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
            int remainingDamage = damageAmount;

            if (stats.currentShield > 0)
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
            else if (damageAmount > 0)
            {
                Debug.Log(gameObject.name + " không nhận sát thương do lá chắn còn đủ.");
            }


            UpdateOwnUI();

            if (battleManager != null)
            {
                battleManager.UpdateCharacterUI(this);
            }
            if (stats.currentHP <= 0)
            {
                stats.currentHP = 0;
                Debug.Log($"{gameObject.name} đã chết!");
                stateMachine.SwitchState(stateMachine.deadState);
                if (battleManager != null)
                {
                    battleManager.RemoveCombatant(this);
                }
            }
            else
            {
                if (damageAmount > 0)
                {
                    stateMachine.SwitchState(stateMachine.takingDamageState);
                }
            }
        }

        public void PrepareHitCallBack(Action callback)
        {
            this.damageCallback = callback;
        }

        public void TriggerDamage()
        {
            damageCallback?.Invoke();

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
            if (buffManager != null)
            {
                buffManager.AddShield(amount);
            }
        }

        public void ApplyAttackBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyAttackBuff(amount, duration);
            }
        }

        public void ApplyMaxHPBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMaxHPBuff(amount, duration);
            }
        }

        public void ApplyDefenseBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyDefenseBuff(amount, duration);
            }
        }

        public void ApplyAgilityBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyAgilityBuff(amount, duration);
            }
        }

        public void ApplyMagicAttackBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalAttackBuff(amount, duration);
            }
        }

        public void ApplyMagicDefenseBuff(int amount, int duration)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalDefenseBuff(amount, duration);
            }
        }

    }
}
