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
        Physical,
        Magical,
        Fire,
        Water,
        Ice,
        Poison,
        Lightning,
        Dark,
        Frost,
        Holy,

    }

    [System.Serializable]
    public class StackData
    {
        public string stackId; 
        public int currentStacks;
        public Sprite icon;
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public string name;
        public Sprite Avatar;
        public int level;
    }


    [System.Serializable]
    public class CharacterStats
    {
        public int maxHP;
        public int currentHP;
        public int maxMP;
        public int currentMP;
        public int maxShield;
        public int currentShield;
        public int physicalAttack;
        public int physicalDefense;
        public int magicAttack;
        public int magicDefense;
        public int crit;
        public int critDamage;
        public int agility;
    }

    public class Character : MonoBehaviour
    {
        public CharacterStateMachine stateMachine;

        public CharacterClass characterClass;

        public CharacterElement characterElement;

        [TabGroup("Class")] public List<CharacterClassProfile> allClassProfiles;

        [TabGroup("Stats")] public CharacterInfo info;

        [TabGroup("Stats")] public CharacterStats stats;

        [TabGroup("Skill")] public List<Skill> skills;

        [TabGroup("Skill Passive")] public List<SkillPassive> passiveSkills;

        public bool isPlayer;
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

        public void AddShield(int amount, int duration, Sprite icon, Flyweight vfxInstance = null)
        {
            if (buffManager != null)
            {
                buffManager.AddShield(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyAttackBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyAttackBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMaxHPBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMaxHPBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyDefenseBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyDefenseBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyAgilityBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {   
            if (buffManager != null)
            {
                buffManager.ApplyAgilityBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMagicAttackBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalAttackBuff(amount, duration, vfxInstance, icon);
            }
        }

        public void ApplyMagicDefenseBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (buffManager != null)
            {
                buffManager.ApplyMagicalDefenseBuff(amount, duration, vfxInstance,icon);
            }
        }
        #endregion

        #region Lấy Dữ liệu Hiệu ứng (Cho CharacterStatUI)
        public List<StatusEffectData> GetActiveStatusEffects()
        {
            List<StatusEffectData> effects = new List<StatusEffectData>();

            if (buffManager == null || debuffManager == null) return effects;


            if (buffManager.shieldTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Lá chắn",
                    TurnsRemaining = buffManager.shieldTurnsRemaining,
                    Detail = $"{stats.currentShield} Shield",
                    IsBuff = true,
                    Icon = buffManager.shieldIcon

                });
            }

            if (buffManager.attackBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.physicalAttack - buffManager.originalBaseAttack;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng P.Attack",
                    TurnsRemaining = buffManager.attackBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.attackBuffIcon
                });
            }

            if (buffManager.defenseBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.physicalDefense - buffManager.originalBaseDefense;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng P.Defense",
                    TurnsRemaining = buffManager.defenseBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.defenseBuffIcon
                });
            }

            if (buffManager.agilityBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.agility - buffManager.originalBaseAgility;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng Agility",
                    TurnsRemaining = buffManager.agilityBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.agilityBuffIcon
                });
            }

            if (buffManager.maxHPBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.maxHP - buffManager.originalBaseMaxHP;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng MaxHP",
                    TurnsRemaining = buffManager.maxHPBuffTurnsRemaining,
                    Detail = $"+{buffAmount} MaxHP",
                    IsBuff = true,
                    Icon = buffManager.maxHPBuffIcon
                });
            }

            if (buffManager.magicalAttackBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.magicAttack - buffManager.magicalOriginalBaseAttack;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng M.Attack",
                    TurnsRemaining = buffManager.magicalAttackBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.magicalAttackBuffIcon
                });
            }

            if (buffManager.magicalDefenseBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.magicDefense - buffManager.magicalOriginalBaseDefense;
                effects.Add(new StatusEffectData
                {
                    Name = "Tăng M.Defense",
                    TurnsRemaining = buffManager.magicalDefenseBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.magicalDefenseBuffIcon
                });
            }



            if (debuffManager.burnTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Thiêu đốt",
                    TurnsRemaining = debuffManager.burnTurnsRemaining,
                    Detail = $"{debuffManager.burnDamagePerTurn} Sát thương/lượt",
                    IsBuff = false,
                    Icon = debuffManager.burnIcon
                });
            }

            if (debuffManager.poisonTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Độc",
                    TurnsRemaining = debuffManager.poisonTurnsRemaining,
                    Detail = $"{debuffManager.poisonDamagePerTurn} Sát thương/lượt",
                    IsBuff = false,
                    Icon = debuffManager.poisonIcon
                });
            }

            if (debuffManager.stunTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Choáng",
                    TurnsRemaining = debuffManager.stunTurnsRemaining,
                    Detail = "Không hành động",
                    IsBuff = false,
                    Icon = debuffManager.stunIcon
                });
            }

            return effects;
        }
        #endregion    
    }

}


