using UnityEngine;
using System;

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

        public void TakeDamage(int damageAmount, ElementType damageElement, bool ignoreBlock = false)
        {
            if (!ignoreBlock && owner.isAttackBlocked)
            {
                Debug.Log($"<color=cyan>[BLOCK SUCCESS]</color> {owner.name} chặn damage thành công!");
                return;
            }

            int remainingDamage = damageAmount;

            if (owner.stats.currentShield > 0)
            {
                int shieldAbsorb = Mathf.Min(owner.stats.currentShield, remainingDamage);
                owner.stats.currentShield -= shieldAbsorb;
                remainingDamage -= shieldAbsorb;
                Debug.Log($"{owner.name} hấp thụ {shieldAbsorb} bằng lá chắn. Còn lại: {owner.stats.currentShield}");
            }

            if (remainingDamage > 0)
            {
                ApplyCoreDamage(remainingDamage, damageElement);
            }
            else if (damageAmount > 0)
            {
                Debug.Log(owner.name + " không mất máu do lá chắn còn đủ.");
            }

            owner.UpdateOwnUI();
            if (owner.battleManager != null)
            {
                owner.battleUIManager.UpdateCharacterUI(owner);
            }

            CheckLifeStatus(remainingDamage);
        }

        private void ApplyCoreDamage(int damage, ElementType element)
        {
            owner.stats.currentHP -= damage;

            float intensity = Mathf.Clamp(damage * 0.01f, 0.1f, 0.5f);
            EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.15f, intensity));

            Color popupColor = VFXManager.Instance.elementColorMap.GetColor(element);
            DamagePopup.Create(
                owner.transform.position,
                damage,
                owner.damagePopupCanvasParent,
                popupColor
            );

            if (owner is Enemy enemyTarget)
            {
                HandleTraildblaze(enemyTarget, element);
            }

            Debug.Log($"{owner.name} nhận {damage} sát thương. Máu còn lại: {owner.stats.currentHP}");
        }

        private void HandleTraildblaze(Enemy enemy, ElementType element)
        {
            float multiplier = 1.0f;
            if (owner.battleManager?.elementChart != null)
            {
                multiplier = owner.battleManager.elementChart.GetMultiplier(element, enemy.characterElement);
            }

            if (multiplier > 1.0f) 
            {
                enemy.traildblaze -= TRAILDBLAZE_REDUCTION;
                enemy.traildblaze = Mathf.Max(0f, enemy.traildblaze);

                if (enemy.enemyUI != null) enemy.enemyUI.UpdateUI();

                if (enemy.traildblaze <= 0)
                {
                    enemy.ApplyBreakStatus(enemy.BreakDebuffSettings);
                }
            }
        }

        private void CheckLifeStatus(int lastDamage)
        {
            if (owner.stats.currentHP <= 0)
            {
                owner.stats.currentHP = 0;
                owner.ProcessOnDeathPassives();
                owner.stateMachine.SwitchState(owner.stateMachine.deadState);
            }
            else if (lastDamage > 0)
            {
                owner.stateMachine.SwitchState(owner.stateMachine.takingDamageState);
            }
        }
    }
}