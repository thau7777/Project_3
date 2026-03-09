using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyRule.Audio;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace Turnbase
{
    public class Enemy : Character
    {
        private Character currentCharacter;

        public float traildblaze = 100f;

        public bool isBoss = false;

        public bool isBroken = false;

        public EnemyStatsUI enemyUI;

        public Dictionary<SkillType, int> currentSkillTypePool;

        [Header("Break Status Settings")]
        public Skill.DebuffSettings BreakDebuffSettings;

        [Header("Visual Effects")]
        public TelegraphEffect telegraphManager;

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
            telegraphManager = GetComponentInChildren<TelegraphEffect>();
        }

        private void Start()
        {
            OnSoundMonster();
        }

        public void Animation_StartAnticipation()
        {
            if (battleManager != null)
            {
                battleManager.evadeUI.StartAnticipation();
                battleManager.parryUI.StartAnticipation();
            }
        }

        public void Animation_TriggerEvent()
        {
            if (telegraphManager != null)
            {
                telegraphManager.Play();
            }

            if (battleManager != null && target != null && !target.isAttackBlocked)
            {
                battleManager.TriggerEvadeOnly(target, this);
                battleManager.TriggerParryOnly(target, this);
            }
        }

        public void Animation_ExecuteParryResult()
        {
            if (battleManager != null)
            {
                if (battleManager.evadeUI != null) battleManager.evadeUI.gameObject.SetActive(false);
                if (battleManager.parryUI != null) battleManager.parryUI.gameObject.SetActive(false);
            }

            if (telegraphManager != null) telegraphManager.Stop();

            if (target != null && target.isAttackBlocked && target.isParrySuccessful)
            {
                Debug.Log($"<color=cyan>[PARRY LOG]</color> {gameObject.name} bị chặn nhịp này.");
                StartCoroutine(DelayedCameraShake(0.1f));
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
            if (isBroken) return;
            isBroken = true;
            traildblaze = 0;

            if (debuffManager != null)
            {
                int turns = breakDebuffSettings.durationTurns;
                EffectSystem.Instance.TriggerBreak(this, turns);
            }

            if (enemyUI != null) enemyUI.UpdateUI();
        }

        private void OnSoundMonster()
        {
            AudioManager.Instance.PlaySFX(SFXType.EnemySound);
            Invoke(nameof(OnSoundMonster), UnityEngine.Random.Range(5f, 10f));
        }
    }
}