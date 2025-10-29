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

        [Header("Poison Debuff State")]
        [HideInInspector] public int poisonTurnsRemaining = 0;
        [HideInInspector] public int poisonDamagePerTurn = 0;
        [HideInInspector] public Flyweight poisonVFXInstance;

        [Header("Stun Debuff State")]
        [HideInInspector] public int stunTurnsRemaining = 0;
        [HideInInspector] public Flyweight stunVFXInstance;




        private void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }

        }

        public void ApplyBurnDebuff(int baseDamage, int duration, Flyweight vfxInstance)
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

            Debug.Log($"{character.name} đã nhận Debuff Thiêu đốt: {burnDamagePerTurn} sát thương/lượt, {duration} lượt.");

        }

        public void ApplyPoisonDebuff(int baseDamage, int duration, Flyweight vfxInstance)
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

            Debug.Log($"{character.name} đã nhận Debuff Độc: {poisonDamagePerTurn} sát thương/lượt, {duration} lượt.");

        }

        public void ApplyStunDebuff(int duration, Flyweight newVfxInstance)
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
            }

            Debug.Log($"{character.name} đã bị Choáng trong {duration} lượt.");
        }


        public void ApplyDebuff(Skill.DebuffSettings debuffSettings)
        {
            if (debuffSettings.debuffType == DebuffType.None || debuffSettings.durationTurns <= 0)
                return;

            Flyweight debuffVFX = null;

            debuffVFX = FlyweightFactory.Spawn(debuffSettings.debuffEffect);

            if (debuffVFX != null)
            {
                debuffVFX.transform.SetParent(character.transform);
                debuffVFX.transform.localPosition = Vector3.zero;
                debuffVFX.gameObject.SetActive(true);
            }
            switch (debuffSettings.debuffType)
            {
                case DebuffType.Burn:
                    ApplyBurnDebuff(
                        debuffSettings.baseDamagePerTurn,
                        debuffSettings.durationTurns,
                        debuffVFX
                    );
                    break;

                case DebuffType.Poison:
                    ApplyPoisonDebuff(
                        debuffSettings.baseDamagePerTurn,
                        debuffSettings.durationTurns,
                        debuffVFX
                    );
                    break;

                case DebuffType.Stun:
                    ApplyStunDebuff(
                        debuffSettings.durationTurns,
                        debuffVFX
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
            if (burnTurnsRemaining > 0)
            {
                burnTurnsRemaining--;
                if (burnTurnsRemaining <= 0)
                {
                    RemoveExpiredBurnDebuff();
                }
            }

            if (poisonTurnsRemaining > 0)
            {
                poisonTurnsRemaining--;
                if (poisonTurnsRemaining <= 0)
                {
                    RemoveExpiredPoisonDebuff();
                }
            }

            if (stunTurnsRemaining > 0)
            {
                stunTurnsRemaining--;
                if (stunTurnsRemaining <= 0)
                {
                    RemoveExpiredStunDebuff();
                }
            }
        }


    }

}
