using System.Collections;
using Turnbase;
using UnityEngine;

namespace Turnbase
{
    public class CharacterDebuffManager : MonoBehaviour
    {
        private CharacterStats stats;
        private Character characterTarget;
        private Character character;


        [Header("Burn Debuff State")]
        [HideInInspector] public int burnTurnsRemaining = 0;
        [HideInInspector] public int burnDamagePerTurn = 0;
        [HideInInspector] public Flyweight_TB burnVFXInstance;
        [HideInInspector] public Sprite burnIcon;

        [Header("Poison Debuff State")]
        [HideInInspector] public int poisonTurnsRemaining = 0;
        [HideInInspector] public float poisonReductionPercentage = 0;
        [HideInInspector] public Flyweight_TB poisonVFXInstance;
        [HideInInspector] public Sprite poisonIcon;

        [Header("Stun Debuff State")]
        [HideInInspector] public int stunTurnsRemaining = 0;
        [HideInInspector] public Flyweight_TB stunVFXInstance;
        [HideInInspector] public Sprite stunIcon;

        [Header("Defense Reduction Debuff State")]
        [HideInInspector] public int defReductionTurnsRemaining = 0;
        [HideInInspector] public float defReductionPercentage = 0f;
        [HideInInspector] public Flyweight_TB defReductionVFXInstance;
        [HideInInspector] public Sprite defReductionIcon;

        [Header("Speed Reduction Debuff State")]
        [HideInInspector] public int speedReductionTurnsRemaining = 0;
        [HideInInspector] public float speedReductionPercentage = 0f;
        [HideInInspector] public Flyweight_TB speedReductionVFXInstance;
        [HideInInspector] public Sprite speedReductionIcon;

        [Header("Braek Debuff State")]
        [HideInInspector] public int breakTurnsRemaining = 0;
        [HideInInspector] public Flyweight_TB breakVFXInstance;
        [HideInInspector] public Sprite breakIcon;

        [Header("Paralysis Debuff State")]
        [HideInInspector] public int paralysisTurnsRemaining = 0;
        [HideInInspector] public float paralysisDamageReduction = 0f;
        [HideInInspector] public Flyweight_TB paralysisVFXInstance;
        [HideInInspector] public Sprite paralysisIcon;


        private void Awake()
        {
            characterTarget = GetComponent<Character>();
            if (characterTarget != null)
            {
                stats = characterTarget.stats;
            }

        }

        public void ApplyBurnDebuff(Character attacker, int baseDamage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            // 1. Kiểm tra điều kiện đầu vào cơ bản
            if (baseDamage <= 0 || duration <= 0) return;

            this.character = attacker; // Attacker thường được lưu để tính toán damage buff/debuff từ người đánh

            // 2. Tính toán Damage dựa trên các nội tại (Passive) của người đánh
            int finalDamage = baseDamage;
            if (attacker != null)
            {
                foreach (var passive in attacker.passiveSkills)
                {
                    if (passive is Passive_DoTBoost dotBoost)
                    {
                        finalDamage = dotBoost.GetBoostedDamage(finalDamage, DoTType.Burn);
                    }
                }
            }

            // 3. Logic cộng dồn hoặc ghi đè damage (Giữ giá trị damage cao nhất)
            if (burnTurnsRemaining <= 0)
            {
                burnDamagePerTurn = finalDamage;
            }
            else
            {
                burnDamagePerTurn = Mathf.Max(burnDamagePerTurn, finalDamage);
            }

            burnTurnsRemaining = duration;

            // 4. Xử lý VFX (Visual Effects)
            if (burnVFXInstance != null && burnVFXInstance != vfxInstance)
            {
                burnVFXInstance.ReturnToPool();
            }

            burnVFXInstance = vfxInstance;
            burnIcon = icon;

            // 5. Cập nhật vị trí và dán VFX vào Mesh (Phần quan trọng nhất)
            if (burnVFXInstance != null)
            {
                var effectController = burnVFXInstance.GetComponentInChildren<CharacterEffectController>();

                // Tìm Mesh ở các object con (như object Model hoặc Body) để tránh lỗi MissingComponent
                var skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

                if (effectController != null && skinnedMesh != null)
                {
                    // Trường hợp: VFX Graph / Shader (Cần dán vào Mesh)
                    burnVFXInstance.transform.SetParent(this.transform);
                    burnVFXInstance.transform.localPosition = Vector3.zero;
                    burnVFXInstance.transform.localRotation = Quaternion.identity;

                    // Truyền chính Transform của SkinnedMeshRenderer vào để Controller.GetComponent thấy luôn
                    effectController.SetupCharacterEffect(skinnedMesh.transform);

                    Debug.Log($"<color=cyan>[Debuff]</color> Đã dán Burn VFX vào Mesh của {gameObject.name}");
                }
                else
                {
                    // Trường hợp: Particle thường (Gán vào xương hoặc điểm Buff)
                    Transform vfxParent = skinnedMesh?.transform.Find("CharacterEffectTarget") ?? characterTarget.buffEffectSpawnPoint;
                    if (vfxParent == null) vfxParent = this.transform;

                    burnVFXInstance.transform.SetParent(vfxParent);
                    burnVFXInstance.transform.localPosition = Vector3.zero;
                    burnVFXInstance.transform.localRotation = Quaternion.identity;
                }
            }

            // 6. Cập nhật UI (Thanh máu, Icon debuff)
            if (characterTarget != null)
            {
                characterTarget.UpdateOwnUI();
            }
        }
        public void ApplyPoisonDebuff(float percentage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (percentage <= 0 || duration <= 0) return;

            if (percentage > poisonReductionPercentage)
            {
                poisonReductionPercentage = percentage;
            }
            poisonTurnsRemaining = duration;

            if (poisonVFXInstance != null && poisonVFXInstance != vfxInstance)
            {
                poisonVFXInstance.ReturnToPool();
            }
            poisonVFXInstance = vfxInstance;
            poisonIcon = icon;

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateDefenseStat();
            }

            characterTarget.UpdateOwnUI();
        }

        public void ApplyStunDebuff(int duration, FlyweightSettings_TB vfxSettings, Sprite icon)
        {
            if (duration <= 0) return;

            stunTurnsRemaining = duration;

            if (characterTarget.stateMachine != null)
            {
                characterTarget.stateMachine.SwitchState(characterTarget.stateMachine.stunnedState);
            }

            if (vfxSettings != null)
            {
                if (stunVFXInstance != null)
                {
                    FlyweightFactory_TB.ReturnToPool(stunVFXInstance);
                }
                SetupVFXTransform(stunVFXInstance);
            }
            stunIcon = icon;

            characterTarget.UpdateOwnUI();
        }

        public void ApplyDefReductionDebuff(float percentage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (percentage <= 0 || duration <= 0) return;

            if (percentage > defReductionPercentage)
            {
                defReductionPercentage = percentage;
            }

            defReductionTurnsRemaining = duration;

            if (defReductionVFXInstance != null && defReductionVFXInstance != vfxInstance)
            {
                defReductionVFXInstance.ReturnToPool();
            }
            defReductionVFXInstance = vfxInstance;

            defReductionIcon = icon;

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateDefenseStat();
            }

            characterTarget.UpdateOwnUI();
        }

        public void ApplyBreakDebuff(int duration, FlyweightSettings_TB vfxSettings, Sprite icon)
        {
            if (duration <= 0) return;

            breakTurnsRemaining = duration;

            if (characterTarget.stateMachine != null)
            {
                characterTarget.stateMachine.SwitchState(characterTarget.stateMachine.stunnedState);
            }

            if (breakVFXInstance != null)
            {
                FlyweightFactory_TB.ReturnToPool(breakVFXInstance);
                breakVFXInstance = null;
            }

            if (vfxSettings != null)
            {
                breakVFXInstance = FlyweightFactory_TB.Spawn(vfxSettings);

                if (breakVFXInstance != null)
                {
                    Transform targetParent = characterTarget.buffEffectSpawnPoint != null
                        ? characterTarget.buffEffectSpawnPoint
                        : characterTarget.transform;

                    breakVFXInstance.transform.SetParent(targetParent, false);
                    breakVFXInstance.transform.localPosition = Vector3.zero;
                    breakVFXInstance.transform.localRotation = Quaternion.identity;
                    breakVFXInstance.transform.localScale = Vector3.one;

                    breakVFXInstance.gameObject.SetActive(true);
                }
            }

            breakIcon = icon;
            characterTarget.UpdateOwnUI();
        }
        public void ApplySpeedReductionDebuff(float percentage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (percentage <= 0 || duration <= 0) return;

            if (percentage > speedReductionPercentage)
            {
                speedReductionPercentage = percentage;
            }

            speedReductionTurnsRemaining = duration;

            if (speedReductionVFXInstance != null && speedReductionVFXInstance != vfxInstance)
            {
                speedReductionVFXInstance.ReturnToPool();
            }

            speedReductionVFXInstance = vfxInstance;
            speedReductionIcon = icon;

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateSpeedStat();
            }

            characterTarget.UpdateOwnUI();

            Debug.Log($"[Debuff] {characterTarget.name} bị giảm {percentage * 100}% Speed trong {duration} lượt.");
        }

        public void ApplyParalysisDebuff(float percentage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if(percentage <= 0 || duration <= 0) return;

            this.paralysisDamageReduction = percentage;
            this.paralysisTurnsRemaining = duration;

            if (paralysisVFXInstance != null && paralysisVFXInstance != vfxInstance)
            {
                paralysisVFXInstance.ReturnToPool();
            }

            paralysisVFXInstance = vfxInstance;
            paralysisIcon = icon;

            characterTarget.UpdateOwnUI();
        }


        private void SetupVFXTransform(Flyweight_TB instance)
        {
            if (instance == null) return;
            Transform targetParent = characterTarget.buffEffectSpawnPoint != null ? characterTarget.buffEffectSpawnPoint : characterTarget.transform;
            instance.transform.SetParent(targetParent, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);
        }

        public void ApplyDebuff(Character attacker, Skill.DebuffSettings debuffSettings)
        {
            if (debuffSettings.statToModify == DebuffType.None || debuffSettings.durationTurns <= 0)
                return;

            Flyweight_TB debuffVFX = null;



            if (debuffSettings.debuffEffect != null)
            {
                debuffVFX = FlyweightFactory_TB.Spawn(debuffSettings.debuffEffect);

                if (debuffVFX != null)
                {
                    Transform targetParent = characterTarget.buffEffectSpawnPoint != null ? characterTarget.buffEffectSpawnPoint : characterTarget.transform;

                    debuffVFX.transform.SetParent(targetParent, false);

                    debuffVFX.transform.localPosition = Vector3.zero;
                    debuffVFX.transform.localRotation = Quaternion.identity;

                    debuffVFX.transform.localScale = Vector3.one;

                    debuffVFX.gameObject.SetActive(true);
                }
            }

            switch (debuffSettings.statToModify)
            {
                case DebuffType.Burn:
                    ApplyBurnDebuff(
                        attacker,
                        debuffSettings.baseDamagePerTurn,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.Poison:
                    ApplyPoisonDebuff(
                        debuffSettings.debuffValue,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon

                    );
                    break;

                case DebuffType.Stun:
                    ApplyStunDebuff(
                        debuffSettings.durationTurns,
                        debuffSettings.debuffEffect,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.DefReduction:
                    ApplyDefReductionDebuff(
                        debuffSettings.debuffValue,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.SpeedReduction:
                    ApplySpeedReductionDebuff(
                        debuffSettings.debuffValue,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.Break:
                    ApplyBreakDebuff(
                        debuffSettings.durationTurns,
                        debuffSettings.debuffEffect,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.Paralysis:
                    ApplyParalysisDebuff(
                        debuffSettings.debuffValue,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

            }
        }

        public int GetEstimatedPoisonDamage()
        {
            float statsDMG = character.stats.magicAttack * 0.1f;
            float hpDMG = characterTarget.stats.maxHP * 0.025f;
            return Mathf.RoundToInt(statsDMG + hpDMG);
        }

        public int GetEstimatedBurnDamage()
        {
            float statsDMG = character.stats.magicAttack * 0.1f;
            float hpDMG = characterTarget.stats.maxHP * 0.025f;
            return Mathf.RoundToInt(statsDMG + hpDMG);
        }

        public IEnumerator ApplyDoTDamage()
        {
            const float INTER_DOT_DELAY = 0.5f;
            const float TICK_DELAY = 0.15f;
            const int TICKS_COUNT = 3;

            if (!characterTarget.isAlive) yield break;

            const ElementType BURN_ELEMENT = ElementType.Fire;
            //const ElementType POISON_ELEMENT = ElementType.Poison;

            bool damageApplied = false;

            // --- BURN ---
            if (burnTurnsRemaining > 0)
            {
                int totalBurnDamage = GetEstimatedBurnDamage(); 

                int damagePerTick = totalBurnDamage / TICKS_COUNT;
                int remainder = totalBurnDamage % TICKS_COUNT;

                for (int i = 0; i < TICKS_COUNT; i++)
                {
                    if (!characterTarget.isAlive) yield break;

                    int currentTickDamage = (i == TICKS_COUNT - 1) ? (damagePerTick + remainder) : damagePerTick;

                    characterTarget.TakeDamage(null, currentTickDamage, BURN_ELEMENT, ignoreBlock: true);
                    damageApplied = true;

                    if (i < TICKS_COUNT - 1) yield return new WaitForSeconds(TICK_DELAY);
                }
            }

            if (damageApplied)
            {
                yield return new WaitForSeconds(INTER_DOT_DELAY);
                damageApplied = false;
            }

        }

        private void RemoveExpiredBurnDebuff()
        {
            if (burnVFXInstance != null)
            {
                burnVFXInstance.ReturnToPool();
                burnVFXInstance = null;
            }
            burnDamagePerTurn = 0;

            characterTarget.UpdateOwnUI();

        }

        private void RemoveExpiredPoisonDebuff()
        {
            poisonReductionPercentage = 0f;
            poisonTurnsRemaining = 0;

            if (poisonVFXInstance != null)
            {
                poisonVFXInstance.ReturnToPool();
                poisonVFXInstance = null;
            }

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateDefenseStat();
            }
        }

        private void RemoveExpiredStunDebuff()
        {
            if (stunVFXInstance != null)
            {
                FlyweightFactory_TB.ReturnToPool(stunVFXInstance);
                stunVFXInstance = null;
            }
            stunTurnsRemaining = 0;

            if (stunTurnsRemaining <= 0 && breakTurnsRemaining <= 0)
            {
                if (characterTarget.stateMachine != null && characterTarget.stateMachine.currentState == characterTarget.stateMachine.stunnedState)
                {
                    characterTarget.stateMachine.SwitchState(characterTarget.stateMachine.waitingState);
                }
            }
        }

        private void RemoveExpiredBreakDebuff()
        {
            if (breakVFXInstance != null)
            {
                FlyweightFactory_TB.ReturnToPool(breakVFXInstance);
                breakVFXInstance = null;
            }
            breakTurnsRemaining = 0;

            if (characterTarget is Enemy enemy)
            {
                enemy.RestoreFromBreak();
            }

            if (stunTurnsRemaining <= 0 && breakTurnsRemaining <= 0)
            {
                if (characterTarget.stateMachine != null && characterTarget.stateMachine.currentState == characterTarget.stateMachine.stunnedState)
                {
                    characterTarget.stateMachine.SwitchState(characterTarget.stateMachine.waitingState);
                }
            }
        }

        private void RemoveExpiredDefReductionDebuff()
        {
            if (defReductionVFXInstance != null)
            {
                FlyweightFactory_TB.ReturnToPool(defReductionVFXInstance);
                defReductionVFXInstance = null;
            }
            defReductionPercentage = 0f;
            defReductionTurnsRemaining = 0;

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateDefenseStat();
            }

            characterTarget.UpdateOwnUI();
        }

        public void RemoveExpiredSpeedReductionDebuff()
        {
            speedReductionPercentage = 0f;
            speedReductionTurnsRemaining = 0;

            if (speedReductionVFXInstance != null)
            {
                speedReductionVFXInstance.transform.SetParent(null);
                speedReductionVFXInstance.ReturnToPool();
                speedReductionVFXInstance = null;
            }

            speedReductionIcon = null;

            if (characterTarget.buffManager != null)
            {
                characterTarget.buffManager.RecalculateSpeedStat();
            }

            Debug.Log($"Debuff giảm Speed của {characterTarget.name} đã hết hạn.");
        }

        public void RemoveExpiredParalysisDebuf()
        {
            speedReductionTurnsRemaining = 0;

            if (paralysisVFXInstance != null)
            {
                paralysisVFXInstance.transform.SetParent(null);
                paralysisVFXInstance.ReturnToPool();
                paralysisVFXInstance = null;
            }

            paralysisIcon = null;

            Debug.Log($"Debuff giảm DMG của {characterTarget.name} đã hết hạn.");

        }


        public void PurifyAllDebuffs()
        {
            bool hadDebuffs = false;

            if (burnTurnsRemaining > 0 || poisonTurnsRemaining > 0 || stunTurnsRemaining > 0 ||
                defReductionTurnsRemaining > 0 || speedReductionTurnsRemaining > 0 || breakTurnsRemaining > 0)
            {
                hadDebuffs = true;
            }

            if (!hadDebuffs) return;

            if (burnVFXInstance != null) { burnVFXInstance.ReturnToPool(); burnVFXInstance = null; }
            burnTurnsRemaining = 0;
            burnDamagePerTurn = 0;
            burnIcon = null;

            if (poisonVFXInstance != null) { poisonVFXInstance.ReturnToPool(); poisonVFXInstance = null; }
            poisonTurnsRemaining = 0;
            poisonReductionPercentage = 0;
            poisonIcon = null;

            if (stunVFXInstance != null) { FlyweightFactory_TB.ReturnToPool(stunVFXInstance); stunVFXInstance = null; }
            stunTurnsRemaining = 0;
            stunIcon = null;

            if (breakVFXInstance != null) { FlyweightFactory_TB.ReturnToPool(breakVFXInstance); breakVFXInstance = null; }
            breakTurnsRemaining = 0;
            breakIcon = null;
            if (characterTarget is Enemy enemy) { enemy.RestoreFromBreak(); }

            if (defReductionVFXInstance != null) { FlyweightFactory_TB.ReturnToPool(defReductionVFXInstance); defReductionVFXInstance = null; }
            defReductionTurnsRemaining = 0;
            defReductionPercentage = 0f;
            defReductionIcon = null;
            if (characterTarget.buffManager != null) characterTarget.buffManager.RecalculateDefenseStat();

            if (speedReductionVFXInstance != null) { speedReductionVFXInstance.ReturnToPool(); speedReductionVFXInstance = null; }
            speedReductionTurnsRemaining = 0;
            speedReductionPercentage = 0f;
            speedReductionIcon = null;

            if(paralysisVFXInstance != null) { paralysisVFXInstance.ReturnToPool(); paralysisVFXInstance = null; }
            paralysisTurnsRemaining = 0;
            paralysisDamageReduction = 0f;
            paralysisIcon = null;

            if (characterTarget.buffManager != null) characterTarget.buffManager.RecalculateSpeedStat();

            if (characterTarget.stateMachine != null && characterTarget.stateMachine.currentState == characterTarget.stateMachine.stunnedState)
            {
                characterTarget.stateMachine.SwitchState(characterTarget.stateMachine.waitingState);
            }

            characterTarget.UpdateOwnUI();
            if (characterTarget.battleUIManager != null)
            {
                characterTarget.battleUIManager.UpdateCharacterUI(characterTarget);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(characterTarget));

            Debug.Log($"<color=green>[PURIFY]</color> {characterTarget.name} đã được thanh tẩy hoàn toàn debuff.");
        }

        public void ProcessTurnStartDecay()
        {
            bool uiUpdateNeeded = false;

            if (burnTurnsRemaining > 0)
            {
                burnTurnsRemaining--;
                if (burnTurnsRemaining <= 0)
                {
                    RemoveExpiredBurnDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if (poisonTurnsRemaining > 0)
            {
                poisonTurnsRemaining--;
                if (poisonTurnsRemaining <= 0)
                {
                    RemoveExpiredPoisonDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if (stunTurnsRemaining > 0)
            {
                stunTurnsRemaining--;
                if (stunTurnsRemaining <= 0)
                {
                    RemoveExpiredStunDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if (defReductionTurnsRemaining > 0)
            {
                defReductionTurnsRemaining--;
                if (defReductionTurnsRemaining <= 0)
                {
                    RemoveExpiredDefReductionDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if (speedReductionTurnsRemaining > 0)
            {
                speedReductionTurnsRemaining--;
                if (speedReductionTurnsRemaining <= 0)
                {
                    RemoveExpiredSpeedReductionDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if (breakTurnsRemaining > 0)
            {
                breakTurnsRemaining--;
                if (breakTurnsRemaining <= 0)
                {
                    RemoveExpiredBreakDebuff();
                    uiUpdateNeeded = true;
                }
            }

            if(paralysisTurnsRemaining > 0)
            {
                paralysisTurnsRemaining--;
                if(paralysisTurnsRemaining <= 0)
                {
                    RemoveExpiredParalysisDebuf();
                    uiUpdateNeeded = true;
                }
            }

            if (uiUpdateNeeded && characterTarget.battleUIManager != null)
            {
                characterTarget.battleUIManager.UpdateCharacterUI(characterTarget);
            }
        }



        public bool IsPoisoned()
        {
            return poisonTurnsRemaining > 0;
        }

        public bool IsBurning()
        {
            return burnTurnsRemaining > 0;
        }   

        public bool IsParalysis()
        {
            return paralysisTurnsRemaining > 0;
        }
    }
}