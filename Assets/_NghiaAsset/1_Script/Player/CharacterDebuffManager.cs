using System.Collections;
using Turnbase;
using UnityEngine;

namespace Turnbase
{
    public class CharacterDebuffManager : MonoBehaviour
    {
        private CharacterStats stats;
        private Character character;

        [Header("Burn Debuff State")]
        [HideInInspector] public int burnTurnsRemaining = 0;
        [HideInInspector] public int burnDamagePerTurn = 0;
        [HideInInspector] public Flyweight_TB burnVFXInstance;
        [HideInInspector] public Sprite burnIcon;

        [Header("Poison Debuff State")]
        [HideInInspector] public int poisonTurnsRemaining = 0;
        [HideInInspector] public int poisonDamagePerTurn = 0;
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

        [Header("Braek Debuff State")]
        [HideInInspector] public int breakTurnsRemaining = 0;
        [HideInInspector] public Flyweight_TB breakVFXInstance;
        [HideInInspector] public Sprite breakIcon;


        private void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }

        }

        public void ApplyBurnDebuff(int baseDamage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (baseDamage <= 0 || duration <= 0) return;

            if (burnTurnsRemaining <= 0)
            {
                burnDamagePerTurn = baseDamage;
            }
            else
            {
                burnDamagePerTurn = Mathf.Max(burnDamagePerTurn, baseDamage);
            }

            burnTurnsRemaining = duration;

            if (burnVFXInstance != null && burnVFXInstance != vfxInstance)
            {
                burnVFXInstance.ReturnToPool();
            }
            burnVFXInstance = vfxInstance;

            burnIcon = icon;

            character.UpdateOwnUI();
        }

        public void ApplyPoisonDebuff(int baseDamage, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (baseDamage <= 0 || duration <= 0) return;

            if (poisonTurnsRemaining <= 0)
            {
                poisonDamagePerTurn = baseDamage;
            }
            else
            {
                poisonDamagePerTurn = Mathf.Max(poisonDamagePerTurn, baseDamage);
            }

            poisonTurnsRemaining = duration;

            if (poisonVFXInstance != null && poisonVFXInstance != vfxInstance)
            {
                poisonVFXInstance.ReturnToPool();
            }
            poisonVFXInstance = vfxInstance;

            poisonIcon = icon;

            character.UpdateOwnUI();
        }

        public void ApplyStunDebuff(int duration, Flyweight_TB newVfxInstance, Sprite icon)
        {
            if (duration <= 0) return;

            stunTurnsRemaining = duration;

            if (character.stateMachine != null)
            {
                character.stateMachine.SwitchState(character.stateMachine.stunnedState);
            }

            if (newVfxInstance != null)
            {
                if (stunVFXInstance != null)
                {
                    FlyweightFactory_TB.ReturnToPool(stunVFXInstance);
                }
                stunVFXInstance = newVfxInstance;

            }
            stunIcon = icon;

            character.UpdateOwnUI();
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

            if (character.buffManager != null)
            {
                character.buffManager.RecalculateDefenseStat();
            }

            character.UpdateOwnUI();
        }

        public void ApplyBreakDebuff(int duration, Flyweight_TB newVfxInstance, Sprite icon)
        {
            if (duration <= 0) return;

            breakTurnsRemaining = duration;

            if (character.stateMachine != null)
            {
                character.stateMachine.SwitchState(character.stateMachine.stunnedState);

            }

            if (newVfxInstance != null)
            {
                if (breakVFXInstance != null)
                {
                    FlyweightFactory_TB.ReturnToPool(breakVFXInstance);
                }
                breakVFXInstance = newVfxInstance;

                breakIcon = icon;

                character.UpdateOwnUI();

            }
        }


        public void ApplyDebuff(Skill.DebuffSettings debuffSettings)
        {
            if (debuffSettings.statToModify == DebuffType.None || debuffSettings.durationTurns <= 0)
                return;

            Flyweight_TB debuffVFX = null;



            if (debuffSettings.debuffEffect != null)
            {
                debuffVFX = FlyweightFactory_TB.Spawn(debuffSettings.debuffEffect);

                if (debuffVFX != null)
                {
                    debuffVFX.transform.SetParent(character.transform);
                    debuffVFX.transform.localPosition = Vector3.zero;
                    debuffVFX.gameObject.SetActive(true);
                }
            }

            switch (debuffSettings.statToModify)
            {
                case DebuffType.Burn:
                    ApplyBurnDebuff(
                        debuffSettings.baseDamagePerTurn,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

                case DebuffType.Poison:
                    ApplyPoisonDebuff(
                        debuffSettings.baseDamagePerTurn,
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon

                    );
                    break;

                case DebuffType.Stun:
                    ApplyStunDebuff(
                        debuffSettings.durationTurns,
                        debuffVFX,
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

                case DebuffType.Break:
                    ApplyBreakDebuff(
                        debuffSettings.durationTurns,
                        debuffVFX,
                        debuffSettings.icon
                    );
                    break;

            }
        }

        public IEnumerator ApplyDoTDamage()
        {
            const float INTER_DOT_DELAY = 0.5f; 
            const float TICK_DELAY = 0.15f;  

            if (!character.isAlive) yield break;

            const ElementType BURN_ELEMENT = ElementType.Fire;
            const ElementType POISON_ELEMENT = ElementType.Poison;

            bool damageApplied = false;

            if (burnTurnsRemaining > 0)
            {
                const int BURN_TICKS = 3;
                int totalBurnDamage = burnDamagePerTurn;
                int damagePerTick = totalBurnDamage / BURN_TICKS;
                int remainder = totalBurnDamage % BURN_TICKS;

                for (int i = 0; i < BURN_TICKS; i++)
                {
                    if (!character.isAlive) yield break;

                    int currentTickDamage = damagePerTick;
                    if (i == BURN_TICKS - 1)
                    {
                        currentTickDamage += remainder;
                    }

                    character.TakeDamage(currentTickDamage, BURN_ELEMENT);

                    damageApplied = true;

                    if (i < BURN_TICKS - 1)
                    {
                        yield return new WaitForSeconds(TICK_DELAY);
                    }
                }
            }

            if (damageApplied)
            {
                yield return new WaitForSeconds(INTER_DOT_DELAY);
                damageApplied = false; 
            }

            if (poisonTurnsRemaining > 0)
            {
                const int POISON_TICKS = 2;
                int totalPoisonDamage = poisonDamagePerTurn;
                int damagePerTick = totalPoisonDamage / POISON_TICKS;
                int remainder = totalPoisonDamage % POISON_TICKS;


                for (int i = 0; i < POISON_TICKS; i++)
                {
                    if (!character.isAlive) yield break;

                    int currentTickDamage = damagePerTick;
                    if (i == POISON_TICKS - 1)
                    {
                        currentTickDamage += remainder;
                    }

                    character.TakeDamage(currentTickDamage, POISON_ELEMENT);

                    damageApplied = true;

                    if (i < POISON_TICKS - 1)
                    {
                        yield return new WaitForSeconds(TICK_DELAY);
                    }
                }
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

            character.UpdateOwnUI();

        }

        private void RemoveExpiredPoisonDebuff()
        {
            if (poisonVFXInstance != null)
            {
                poisonVFXInstance.ReturnToPool();
                poisonVFXInstance = null;
            }
            poisonDamagePerTurn = 0;
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
                if (character.stateMachine != null && character.stateMachine.currentState == character.stateMachine.stunnedState)
                {
                    character.stateMachine.SwitchState(character.stateMachine.waitingState);
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

            if (character is Enemy enemy)
            {
                enemy.RestoreFromBreak();
            }

            if (stunTurnsRemaining <= 0 && breakTurnsRemaining <= 0)
            {
                if (character.stateMachine != null && character.stateMachine.currentState == character.stateMachine.stunnedState)
                {
                    character.stateMachine.SwitchState(character.stateMachine.waitingState);
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

            if (character.buffManager != null)
            {
                character.buffManager.RecalculateDefenseStat();
            }

            character.UpdateOwnUI();
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
                else
                {
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

            if (uiUpdateNeeded && character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }
    }
}