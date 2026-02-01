using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace Turnbase
{
    public class Character : MonoBehaviour
    {
        public CharacterStateMachine stateMachine;

        public CharacterClass characterClass;

        public CharacterElement characterElement;

        [TabGroup("Stats")] public CharacterInfo info;

        [TabGroup("Stats")] public CharacterStats stats;

        [TabGroup("Class")] public List<CharacterClassProfile> allClassProfiles;

        [TabGroup("Skill")] public List<Skill> skills;

        [TabGroup("Item")] public List<Tb_Item> item;

        [TabGroup("Skill Passive")] public List<SkillPassive> passiveSkills;

        public bool isPlayer;
        public bool isVirtualTracker = false;
        public Character target;
        public GameObject targetMarker;

        public Animator animator;

        public float actionGauge;

        public Vector3 initialPosition;
        public Quaternion initialRotation;

        public BattleManager battleManager;

        public BattleUIManager battleUIManager;

        public CharacterBuffManager buffManager;

        public CharacterDebuffManager debuffManager;


        public bool isParryable;

        public PlayerActionUI ownUI;

        public Action OnAttackHitFrame;
        private Action damageCallback;

        public bool isAttackReadyForParry = false;
        public bool isParryWindowFinished = false;
        public float parryWindowDuration = 0f;

        public RenderTexture RenderTexture;

        public Transform damagePopupCanvasParent;

        public Skill selectedSkill { get; set; }

        public EnemyStatsUI enemyStatsUI;

        public Transform SkillSpawnPoint;

        public Transform SkillSpawnPoint2;

        public Transform buffEffectSpawnPoint;

        public bool isAttackBlocked = false;

        [HideInInspector] public HealthSystem healthSystem;

        public bool isAlive
        {
            get { return stats.currentHP > 0; }
        }

        public bool isParrySuccessful = false;



        void Awake()
        {
            stateMachine = GetComponent<CharacterStateMachine>();
            buffManager = GetComponent<CharacterBuffManager>();
            debuffManager = GetComponent<CharacterDebuffManager>();
            animator = GetComponent<Animator>();



            if (stats == null)
            {
                stats = GetComponent<CharacterStats>();
            }
            if (info == null)
            {
                info = GetComponent<CharacterInfo>();
            }


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

            if (targetProfile.initialPassiveSkills != null) 
            {
                passiveSkills.Clear();
                passiveSkills.AddRange(targetProfile.initialPassiveSkills);
                Debug.Log($"[LOG] Đã nạp {passiveSkills.Count} nội tại cho {gameObject.name}");
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


        public void TakeDamage(Character attacker, int amount, ElementType element, bool ignoreBlock = false)
        {
            if (healthSystem == null)
            {
                healthSystem = GetComponent<HealthSystem>();
                if (healthSystem == null)
                {
                    healthSystem = gameObject.AddComponent<HealthSystem>();
                }
                healthSystem.Init(this);
            }

            healthSystem.TakeDamage(attacker, amount, element, ignoreBlock);
        }

        public void ProcessOnDeathPassives()
        {
            if (battleManager != null && battleManager.turnbuffManager != null)
            {
                battleManager.turnbuffManager.ProcessOnDeathPassives(this);
            }
        }

        public void PrepareHitCallBack(Action callback)
        {
            this.damageCallback = callback;
        }

        public void TriggerDamage()
        {
            // Không dùng return ở đây nữa
            if (this.isAttackBlocked)
            {
                Debug.Log($"[LOG] {gameObject.name} bị chặn, nhưng vẫn gửi callback để giải phóng Command.");
            }

            damageCallback?.Invoke();
            // Sau khi gọi xong nên null để tránh gọi trùng lặp nếu animation loop
            damageCallback = null;
        }

        #region Heal and Buffs Methods
        public void Heal(int amount)
        {
            if (!isAlive) return;

            stats.currentHP = Mathf.Min(stats.currentHP + amount, stats.maxHP);

            UpdateOwnUI();

            if (battleManager != null)
            {
                battleUIManager.UpdateCharacterUI(this);
            }

            Debug.Log($"{gameObject.name} hồi {amount} máu! Máu hiện tại: {stats.currentHP}");
        }

        public void RestoreMana(int amount)
        {
            if (!isAlive) return;

            stats.currentMP = Mathf.Min(stats.currentMP + amount, stats.maxMP);

            UpdateOwnUI();

            if (battleManager != null)
            {
                battleUIManager.UpdateCharacterUI(this);
            }

            Debug.Log($"{gameObject.name} hồi {amount} mana! Mana hiện tại: {stats.currentMP}");
        }

        public void AddShield(int amount, int duration, Sprite icon, Flyweight_TB vfxInstance = null)
        {
            if (buffManager != null)
            {
                buffManager.AddShield(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyAttackBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyAttackBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMaxHPBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMaxHPBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyDefenseBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyDefenseBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyAgilityBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {   
            if (buffManager != null)
            {
                buffManager.ApplyAgilityBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMagicAttackBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalAttackBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMagicDefenseBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalDefenseBuff(amount, duration, vfxInstance,icon);
            }
        }
        #endregion

    }

}


