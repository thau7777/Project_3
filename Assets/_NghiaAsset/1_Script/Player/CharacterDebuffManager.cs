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
        [HideInInspector] public Flyweight burnVFXInstance;
        [HideInInspector] public Sprite burnIcon;

        [Header("Poison Debuff State")]
        [HideInInspector] public int poisonTurnsRemaining = 0;
        [HideInInspector] public int poisonDamagePerTurn = 0;
        [HideInInspector] public Flyweight poisonVFXInstance;
        [HideInInspector] public Sprite poisonIcon;

        [Header("Stun Debuff State")]
        [HideInInspector] public int stunTurnsRemaining = 0;
        [HideInInspector] public Flyweight stunVFXInstance;
        [HideInInspector] public Sprite stunIcon;


        private void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }

        }

        public void ApplyBurnDebuff(int baseDamage, int duration, Flyweight vfxInstance, Sprite icon)
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

            Debug.Log($"{character.name} đã nhận Debuff Thiêu đốt: **{burnDamagePerTurn} sát thương/lượt**, **{duration} lượt**.");
        }

        public void ApplyPoisonDebuff(int baseDamage, int duration, Flyweight vfxInstance, Sprite icon)
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

            Debug.Log($"{character.name} đã nhận Debuff Độc: **{poisonDamagePerTurn} sát thương/lượt**, **{duration} lượt**.");
        }

        public void ApplyStunDebuff(int duration, Flyweight newVfxInstance, Sprite icon)
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
                    FlyweightFactory.ReturnToPool(stunVFXInstance);
                }
                stunVFXInstance = newVfxInstance;

                stunIcon = icon;
            }

            Debug.Log($"{character.name} đã bị Choáng trong **{duration} lượt**.");
        }


        public void ApplyDebuff(Skill.DebuffSettings debuffSettings)
        {
            if (debuffSettings.debuffType == DebuffType.None || debuffSettings.durationTurns <= 0)
                return;

            Flyweight debuffVFX = null;



            if (debuffSettings.debuffEffect != null)
            {
                debuffVFX = FlyweightFactory.Spawn(debuffSettings.debuffEffect);

                if (debuffVFX != null)
                {
                    debuffVFX.transform.SetParent(character.transform);
                    debuffVFX.transform.localPosition = Vector3.zero;
                    debuffVFX.gameObject.SetActive(true);
                }
            }

            switch (debuffSettings.debuffType)
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

            }
        }

        public void ApplyDoTDamage()
        {
            if (!character.isAlive) return;


            if (burnTurnsRemaining > 0)
            {
                Debug.Log($"{character.name} nhận sát thương từ Thiêu đốt: {burnDamagePerTurn}");
                character.TakeDamage(burnDamagePerTurn);
            }

            if (poisonDamagePerTurn > 0)
            {
                Debug.Log($"{character.name} nhận sát thương từ Độc: {poisonDamagePerTurn}");
                character.TakeDamage(poisonDamagePerTurn);
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



            Debug.Log($"Debuff Thiêu đốt của {character.name} đã hết hạn.");

        }

        private void RemoveExpiredPoisonDebuff()
        {
            if (poisonVFXInstance != null)
            {
                poisonVFXInstance.ReturnToPool();
                poisonVFXInstance = null;
            }
            poisonDamagePerTurn = 0;

            Debug.Log($"Debuff Độc của {character.name} đã hết hạn.");
        }

        private void RemoveExpiredStunDebuff()
        {
            if (stunVFXInstance != null)
            {
                FlyweightFactory.ReturnToPool(stunVFXInstance);
                stunVFXInstance = null;
            }
            stunTurnsRemaining = 0;
            if (character.stateMachine != null && character.stateMachine.currentState == character.stateMachine.stunnedState)
            {
                character.stateMachine.SwitchState(character.stateMachine.waitingState);
            }
            Debug.Log($"Debuff Choáng của {character.name} đã hết hạn.");
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

            if (uiUpdateNeeded && character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }
    }
}