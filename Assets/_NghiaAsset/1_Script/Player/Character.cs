using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


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

        public bool isAlive
        {
            get { return stats.currentHP > 0; }
        }



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
        }

        public void UpdateOwnUI()
        {
            EnemyStatsUI uiComponent = GetComponentInChildren<EnemyStatsUI>();

            if (uiComponent != null)
            {
                uiComponent.UpdateUI();
            }
        }


        public void TakeDamage(int damageAmount, ElementType damageElement)
        {
            int remainingDamage = damageAmount;
            int traildblaze = 100;

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

                Vector3 spawnPosition = transform.position;

                Color popupColor = VFXManager.Instance.elementColorMap.GetColor(damageElement);

                DamagePopup.Create(
                    transform.position,
                    remainingDamage,
                    damagePopupCanvasParent,
                    popupColor
                );
                Debug.Log(gameObject.name + " nhận " + remainingDamage + " sát thương. Máu còn lại: " + stats.currentHP);

                if (this is Enemy enemyTarget)
                {
                    float elementMultiplier = 1.0f;

                    if (battleManager != null && battleManager.elementChart != null)
                    {
                        elementMultiplier = battleManager.elementChart.GetMultiplier(damageElement, enemyTarget.characterElement);
                    }

                    if (elementMultiplier > 1.0f)
                    {
                        enemyTarget.traildblaze -= traildblaze;
                        enemyTarget.traildblaze = Mathf.Max(0f, enemyTarget.traildblaze); 

                        if (enemyTarget.enemyUI != null)
                        {
                            enemyTarget.enemyUI.UpdateUI();
                        }
                        Debug.Log($"[{enemyTarget.gameObject.name}] Bị khắc chế! Traildblaze giảm xuống còn: {enemyTarget.traildblaze}");
                    }
                    else
                    {
                        Debug.Log($"[{enemyTarget.gameObject.name}] Không bị khắc chế ({elementMultiplier}). Traildblaze không giảm.");
                    }

                    if(enemyTarget.traildblaze <= 0)
                    {
                        enemyTarget.ApplyBreakStatus(enemyTarget.BreakDebuffSettings);
                    }
                }

            }
            else if (damageAmount > 0)
            {
                Debug.Log(gameObject.name + " không nhận sát thương do lá chắn còn đủ.");
            }


            UpdateOwnUI();

            if (battleManager != null)
            {
                battleUIManager.UpdateCharacterUI(this);
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


