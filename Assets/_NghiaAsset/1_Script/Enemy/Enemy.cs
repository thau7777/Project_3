using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Turnbase
{
    public class Enemy : Character
    {
        private Character currentCharacter;

        public EnemyStatsUI enemyUI;

        public Dictionary<SkillType, int> currentSkillTypePool;

        private void Awake() 
        {
            if (currentSkillTypePool == null)
            {
                currentSkillTypePool = new Dictionary<SkillType, int>();
            }

            foreach (var skillType in SkillResource.CostPerUse.Keys)
            {
                if (!currentSkillTypePool.ContainsKey(skillType))
                {
                    currentSkillTypePool.Add(skillType, SkillResource.MAX_POOL);
                }
            }
        }

        public void Animation_ReadyParry()
        {
            isAttackReadyForParry = true;
        }

        public void Animation_EndParry()
        {
            isParryWindowFinished = true;
        }


        public void ConsumeSkillTypePool(SkillType type)
        {
            if (currentSkillTypePool != null && currentSkillTypePool.ContainsKey(type))
            {
                int cost = SkillResource.CostPerUse.GetValueOrDefault(type, 0);
                currentSkillTypePool[type] -= cost;

                if (currentSkillTypePool[type] < 0)
                {
                    currentSkillTypePool[type] = 0;
                }
            }
        }

        public void RegenerateSkillTypePool()
        {
            if (currentSkillTypePool == null) return;

            foreach (var key in currentSkillTypePool.Keys.ToList())
            {
                currentSkillTypePool[key] = Math.Min(currentSkillTypePool[key] + 20, SkillResource.MAX_POOL);
            }
        }

        public void PerformTurn()
        {
            RegenerateSkillTypePool();

            EnemyAIController aiController = new EnemyAIController();
            (Skill chosenSkill, Character chosenTarget) = aiController.DetermineBestAction(this, battleManager);

            if (chosenTarget != null && chosenSkill != null)
            {
                this.target = chosenTarget;
                this.selectedSkill = chosenSkill;

                if (chosenSkill.skillType != SkillType.Buff &&
                    chosenSkill.skillType != SkillType.Shield &&
                    chosenSkill.skillType != SkillType.Heal)
                {
                    RotateToTarget(chosenTarget.transform.position);
                }

                CameraAction.instance.LookCameraAtTarget(this.target);

                Debug.Log($"[AI]{gameObject.name} dùng kỹ năng: {chosenSkill.skillName} lên mục tiêu: {chosenTarget.gameObject.name}");

                this.stats.currentMP -= chosenSkill.manaCost;
                enemyUI.UpdateUI();

                ConsumeSkillTypePool(chosenSkill.skillType);

                stateMachine.SwitchState(new SkillAttackingState(stateMachine, chosenSkill));

                EventBus<HidePanelEvent>.Raise(new HidePanelEvent(panelName: "EnemyUI"));

                
            }
            else
            {
                Debug.Log("[AI] Không tìm thấy mục tiêu khả dụng hoặc hành động phù hợp. Kết thúc lượt.");
                stateMachine.battleManager.EndTurn(this);
            }
        }

        private void RotateToTarget(Vector3 targetPosition)
        {
            Vector3 directionToTarget = targetPosition - transform.position;
            directionToTarget.y = 0;

            if (directionToTarget.sqrMagnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = targetRotation;
            }
        }
    }
}