using System;
using UnityEngine;
using MyRule;

namespace Turnbase
{
    public class HealthSystem : MonoBehaviour
    {
        private Character owner;
        private const int TRAILDBLAZE_REDUCTION = 30;

        public void Init(Character character)
        {
            owner = character;
        }

        public void TakeDamage(Character attacker, int damageAmount, ElementType damageElement, bool ignoreBlock = false, bool isCrit = false)
        {
            if (!ignoreBlock && owner.isAttackBlocked)
            {
                Debug.Log($"<color=cyan>[BLOCK SUCCESS]</color> {owner.name} chặn damage thành công!");
                return;
            }

            int remainingDamage = damageAmount;

            if (remainingDamage > 0)
            {
                ApplyCoreDamage(remainingDamage, damageElement, isCrit);
            }

            owner.UpdateOwnUI();
            if (owner.battleManager != null)
            {
                owner.battleUIManager.UpdateCharacterUI(owner);
            }

            CheckLifeStatus(attacker, remainingDamage);
        }

        private void ApplyCoreDamage(int damage, ElementType element, bool isCrit)
        {
            bool finalCrit = isCrit || DamageCalculator.IsLastHitCrit;
            owner.stats.currentHP -= damage;

            if (finalCrit) 
            {
                Debug.Log($"<color=yellow>[CRITICAL HIT!]</color> {owner.name} nhận sát thương chí mạng...");
            }


            float intensity = Mathf.Clamp(damage * 0.01f, 0.1f, 0.5f);

            Color popupColor = VFXManager.Instance.elementColorMap.GetColor(element);

            float randomRadius = 1f;
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * randomRadius;
            Vector3 spawnPosition = owner.damagePopupCanvasParent.position + randomOffset;

            DamagePopup.Create(spawnPosition, damage, owner.damagePopupCanvasParent, popupColor, finalCrit);

            if (owner is Enemy enemyTarget)
            {
                HandleTraildblaze(enemyTarget, element);
            }
        }

        private void HandleTraildblaze(Enemy enemy, ElementType element)
        {
            float multiplier = 1.0f;
            if (owner.battleManager?.elementChart != null)
                multiplier = owner.battleManager.elementChart.GetMultiplier(element, enemy.characterElement);

            if(multiplier > 1.0f)
            {
                float reduction = TRAILDBLAZE_REDUCTION;

                if (enemy.debuffManager != null && enemy.debuffManager.IsPoisoned())
                {
                    reduction *= 1.5f;
                }

                enemy.traildblaze -= reduction;
                enemy.traildblaze = Mathf.Max(0f, enemy.traildblaze);

                if (enemy.enemyUI != null) enemy.enemyUI.UpdateUI();
                if (enemy.traildblaze <= 0) enemy.ApplyBreakStatus(enemy.BreakDebuffSettings);

            }
        }

        private void CheckLifeStatus(Character attacker, int lastDamage)
        {
            if (owner.stats.currentHP <= 0)
            {
                owner.stats.currentHP = 0;
                owner.ProcessOnDeathPassives();

                if (attacker != null && attacker.passiveSkills != null)
                {
                    foreach (var passive in attacker.passiveSkills)
                    {
                        passive.OnKill(attacker, owner);
                    }
                }

                if (!owner.isPlayer)
                {
                    AchievementManager.Instance.Trigger<int>(AchievementType.KillEnemy, 1);
                }
                else if (!owner.isPet && owner.battleManager != null)
                {
                    owner.battleManager.CheckWaveCondition();
                }

                owner.stateMachine.SwitchState(owner.stateMachine.deadState);
            }
            else if (lastDamage > 0)
            {
                owner.stateMachine.SwitchState(owner.stateMachine.takingDamageState);
            }
        }
    }
}