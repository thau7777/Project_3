using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;


namespace Turnbase
{
    public class DamageAllCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float TARGET_DELAY = 0.05f;

        public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
        }

        public IEnumerator Execute()
        {
            if (!string.IsNullOrEmpty(skill.animationTriggerName))
            {
                user.animator.Play(skill.animationTriggerName);
            }

            yield return new WaitForSeconds(1.5f);

            List<Character> allTargets = GetTargets();

            yield return ApplyDamageToTargets(allTargets);

            float totalAnimationDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(totalAnimationDuration);

            if (battleManager != null)
            {
                battleManager.EndTurn(user);
            }
        }

        private List<Character> GetTargets()
        {
            if (user.isPlayer)
            {
                return battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
            }
            else
            {
                return battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
            }
        }

        private IEnumerator ApplyDamageToTargets(List<Character> targets)
        {
            ElementType element = skill.elementType;

            Debug.Log($"[DamageAllCommand] Bắt đầu áp dụng sát thương tuần tự cho {targets.Count} mục tiêu.");

            for (int i = 0; i < targets.Count; i++)
            {
                Character aoeTarget = targets[i];

                if (aoeTarget == null || !aoeTarget.isAlive) continue;

                try
                {
                    int finalDamage = DamageCalculator.GetFinalDamage(user, aoeTarget, skill, battleManager);

                    Debug.Log($"[DamageAllCommand] Đánh mục tiêu {i + 1}/{targets.Count}: {aoeTarget.gameObject.name} với {finalDamage} sát thương.");

                    aoeTarget.TakeDamage(finalDamage, element);

                    SpawnImpactEffect(aoeTarget.transform.position);

                    if (skill.debuffProperties.statToModify != DebuffType.None)
                    {
                        aoeTarget.debuffManager.ApplyDebuff(skill.debuffProperties);
                    }

                    if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                    {
                        user.buffManager.ProcessSkillStacks(skill, aoeTarget);
                    }

                    // LƯU Ý: Loại bỏ yield return khỏi đây!
                }
                catch (System.Exception ex)
                {
                    // Bắt lỗi, in ra log và tiếp tục (không dừng Coroutine)
                    Debug.LogError($"[DamageAllCommand] LỖI khi xử lý mục tiêu {aoeTarget.gameObject.name} (index {i}): {ex.Message}");

                    // LƯU Ý: Loại bỏ yield return khỏi đây!
                }

                // Đặt yield return sau khối try-catch để nó luôn chạy và không gây lỗi biên dịch.
                yield return new WaitForSeconds(TARGET_DELAY);
            }
            Debug.Log("[DamageAllCommand] Đã hoàn thành xử lý tất cả mục tiêu.");
        }
        private void SpawnImpactEffect(Vector3 position)
        {
            FlyweightSettings2 effectToSpawn = skill.impactVFXPrefab;

            if (effectToSpawn != null)
            {
                Flyweight2 effectInstance = FlyweightFactory2.Spawn(effectToSpawn);

                if (effectInstance != null)
                {
                    effectInstance.Initialize(position, Quaternion.identity);

                }
            }
        }
    }
}