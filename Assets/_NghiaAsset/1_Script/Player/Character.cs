using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ami.BroAudio;
using Cysharp.Threading.Tasks;
using MyRule;
using MyRule.Audio;
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

        public ScreenEffectUIController _screenEffectController;

        public SoundID hitSound;

        public bool isAlive
        {
            get { return stats.currentHP > 0; }
        }

        public bool isParrySuccessful = false;
        public bool isLastHit = false;
        public int parryMissCount = 0;
        public int totalHitsInSequence = 0;
        public int currentHitInSequence = 0;


        void Awake()
        {
            stateMachine = GetComponent<CharacterStateMachine>();
            buffManager = GetComponent<CharacterBuffManager>();
            debuffManager = GetComponent<CharacterDebuffManager>();
            animator = GetComponent<Animator>();
            healthSystem = GetComponent<HealthSystem>();
            if (healthSystem == null) healthSystem = gameObject.AddComponent<HealthSystem>();
            healthSystem.Init(this);

            CharacterData currentStats = CharacterManager.Instance.GetCharacterStats();
            stats = new CharacterStats(currentStats);

            if (stats == null)
            {

            }
            if (info == null)
            {
                info = GetComponent<CharacterInfo>();
            }


            InitializeCharacterFrom(characterClass);


        }

        public async void InitializeCharacterFrom(CharacterClass classTypeToLoad)
        {
            CharacterClassProfile targetProfile =
              allClassProfiles.FirstOrDefault(p => p.characterClass == classTypeToLoad);

            if (targetProfile == null)
            {
                Debug.LogWarning($"Không tìm thấy Class Profile cho lớp: {classTypeToLoad} trên {gameObject.name}!");
                return;
            }

            characterClass = targetProfile.characterClass;

            if (animator != null && targetProfile.animatorController != null)
            {
                animator.runtimeAnimatorController = targetProfile.animatorController;
            }

            var itemStorageMgr = ItemStorageManager.Instance;
            item.Clear();

            if (itemStorageMgr != null && itemStorageMgr.ItemStorage != null)
            {
                foreach (var storageItem in itemStorageMgr.ItemStorage.Items)
                {
                    if (storageItem == null) continue;
                    var template = targetProfile.initiaItem
                        .FirstOrDefault(t => t != null && t.type == storageItem.ItemType);

                    if (template != null)
                    {
                        Tb_Item clonedItem = Instantiate(template);
                        clonedItem.quantity = 1;

                        item.Add(clonedItem);
                    }
                }
                Debug.Log($"[Init] Nhân vật nhận {item.Count} vật phẩm từ kho đồ cá nhân.");
            }

            await UniTask.WaitUntil(() => SigilStorageManager.Instance.SigilStorageData != null);

            var storageManager = FindFirstObjectByType<SigilStorageManager>();
            if (storageManager == null || storageManager.SigilStorageData == null)
            {
                Debug.LogError("Không tìm thấy SigilStorageSO để lọc kỹ năng!");
                return;
            }

            Dictionary<string, SigilData> ownedSigils = new Dictionary<string, SigilData>();
            for (int i = 5; i < storageManager.SigilStorageData.ActiveSigils.Length; i++)
            {
                SigilData sigilData = storageManager.SigilStorageData.ActiveSigils[i];
                if (sigilData == null) continue;
                else if (sigilData != null) ownedSigils[sigilData.Name] = sigilData;
            }

            for (int i = 0; i < storageManager.SigilStorageData.PassiveSigils.Length; i++)
            {
                SigilData sigilData = storageManager.SigilStorageData.PassiveSigils[i];
                if (sigilData == null) continue;
                else if (sigilData != null) ownedSigils[sigilData.Name] = sigilData;
            }

            skills.Clear();
            if (targetProfile.initialSkills != null)
            {
                foreach (var skillTemplate in targetProfile.initialSkills)
                {
                    if (skillTemplate != null && ownedSigils.TryGetValue(skillTemplate.skillName, out SigilData data))
                    {
                        Skill clonedSkill = Instantiate(skillTemplate);

                        clonedSkill.damage = data.BaseDamage;
                        clonedSkill.manaCost = data.ManaCost;

                        skills.Add(clonedSkill);
                    }
                }
            }

            passiveSkills.Clear();
            if (targetProfile.initialPassiveSkills != null)
            {
                foreach (var pSkill in targetProfile.initialPassiveSkills)
                {
                    if (pSkill != null && ownedSigils.ContainsKey(pSkill.name))
                    {
                        passiveSkills.Add(pSkill);
                    }
                }
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

        private float lastHurtSoundTime;
        private const float HURT_SOUND_COOLDOWN = 2f;

        public void TakeDamage(Character attacker, int amount, ElementType element, bool ignoreBlock = false, bool isCrit = false)
        {
            if (buffManager != null && buffManager.CheckAndConsumeDivineShield())
            {
                return;
            }

            if (Time.time - lastHurtSoundTime > HURT_SOUND_COOLDOWN)
            {
                if (AudioManager.Instance != null)
                {
                    SFXType hurtSFX = isPlayer ? SFXType.Hurt : SFXType.EnemyHurt;
                    //AudioManager.Instance.PlaySFX(hurtSFX);
                }
                lastHurtSoundTime = Time.time;
            }

            if (healthSystem == null)
            {
                healthSystem = GetComponent<HealthSystem>();
                if (healthSystem == null)
                {
                    healthSystem = gameObject.AddComponent<HealthSystem>();
                }
                healthSystem.Init(this);
            }

            if (isPlayer && _screenEffectController != null)
            {
                _screenEffectController.OnDamaged();
                Debug.Log("show effect ondamgae");
            }

            healthSystem.TakeDamage(attacker, amount, element, ignoreBlock, isCrit);

            if (attacker != null && attacker.isAlive && buffManager != null)
            {
                Skill counterSkill = null;

                if (buffManager.magicalDefenseBuffTurnsRemaining > 0)
                {
                    counterSkill = skills.Find(s => s.stackApplicationTarget == StackApplicationTarget.Counter
                                  && s.activatedDebuff.statToModify == DebuffType.SpeedReduction);
                }

                if (counterSkill == null && buffManager.magicalAttackBuffTurnsRemaining > 0)
                {
                    counterSkill = skills.Find(s => s.stackApplicationTarget == StackApplicationTarget.Counter
                                  && s.activatedDebuff.statToModify == DebuffType.Burn);
                }

                if (counterSkill == null && buffManager.defenseBuffTurnsRemaining > 0)
                {
                    counterSkill = skills.Find(s => s.stackApplicationTarget == StackApplicationTarget.Counter
                                  && s.activatedDebuff.statToModify == DebuffType.Poison);
                }

                if (counterSkill != null)
                {
                    buffManager.ProcessSkillStacks(counterSkill, attacker);
                    Debug.Log($"[COUNTER] Phản đòn từ danh sách: {counterSkill.skillName}");
                }
            }

            Color elementColor = Color.white;

            CameraShaker.Instance.GenerateBasicShake();
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
            if (this.isAttackBlocked)
            {
                Debug.Log($"[LOG] {gameObject.name} bị chặn, nhưng vẫn gửi callback để giải phóng Command.");
            }

            damageCallback?.Invoke();
            damageCallback = null;

            BroAudio.Play(hitSound);
        }

        #region Heal Methods
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

        #endregion

    }

}
