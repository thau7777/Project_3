using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Turnbase
{
    public class Enemy : Character
    {
        private Character currentCharacter;

        public float traildblaze = 100f;

        public bool isBroken = false;

        public EnemyStatsUI enemyUI;

        public Dictionary<SkillType, int> currentSkillTypePool;

        [Header("Break Status Settings")]
        public Skill.DebuffSettings BreakDebuffSettings;

        private EnemyAIController aiController;

        private void Awake() 
        {
            aiController = GetComponent<EnemyAIController>();

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

        public void Animation_CheckParryResult()
        {
            if (target != null && target.isAttackBlocked)
            {
                Debug.Log($"<color=orange>[PARRY HIT]</color> {gameObject.name} bị khựng lại do bị phản đòn!");

                if (stateMachine != null)
                {
                    stateMachine.SwitchState(new InterruptedState(stateMachine));
                }

                if (target.stateMachine != null)
                {
                    target.stateMachine.SwitchState(target.stateMachine.parryingState);
                    Debug.Log($"<color=green>[PARRY HIT]</color> Chuyển {target.gameObject.name} sang trạng thái ParryingState!");
                }


                StartCoroutine(DelayedCameraShake(0.3f));
            }
        }

        private IEnumerator DelayedCameraShake(float delay)
        {
            yield return new WaitForSeconds(delay);
            EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.15f, 0.3f));
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

        public void PrepareTurn()
        {
            RegenerateSkillTypePool();
            (Skill chosenSkill, Character chosenTarget) = aiController.DetermineBestAction(this, battleManager);

            if (chosenTarget != null && chosenSkill != null)
            {
                this.target = chosenTarget;
                this.selectedSkill = chosenSkill;
            }
        }

        public void ExecuteTurn()
        {
            if (target != null && selectedSkill != null)
            {
                if (selectedSkill.skillType != SkillType.Buff &&
                    selectedSkill.skillType != SkillType.Shield &&
                    selectedSkill.skillType != SkillType.Heal)
                {
                    RotateToTarget(target.transform.position);
                }

                CameraAction.instance.LookCameraAtTarget(this.target);
                this.stats.currentMP -= selectedSkill.manaCost;
                enemyUI.UpdateUI();
                ConsumeSkillTypePool(selectedSkill.skillType);

                stateMachine.SwitchState(new SkillAttackingState(stateMachine, selectedSkill));
                EventBus<HidePanelEvent>.Raise(new HidePanelEvent(panelName: "EnemyUI"));
            }
            else
            {
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

        public void RestoreFromBreak()
        {
            if (!isBroken) return;

            traildblaze = 100f;
            isBroken = false;

            if (enemyUI != null)
            {
                enemyUI.UpdateUI();
            }

        }

        public void ApplyBreakStatus(Skill.DebuffSettings breakDebuffSettings)
        {
            if(isBroken) return;

            isBroken = true;

            if (debuffManager != null)
            {
                debuffManager.ApplyDebuff(breakDebuffSettings);
            }
        }
    }
}