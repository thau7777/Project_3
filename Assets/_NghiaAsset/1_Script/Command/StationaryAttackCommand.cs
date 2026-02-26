using System;
using System.Collections;
using UnityEngine;


namespace Turnbase
{
    public class StationaryAttackCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;

        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        public StationaryAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {

            yield return PerformStationaryAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        

        private IEnumerator PerformStationaryAttack()
        {
            CalculateFinalDamage();
            ElementType element = skill.elementType;
            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    target.TakeDamage(user, finalDamage, element);
                    damageApplied = true;
                    SpawnEffectAtTarget();
                }
            };

            user.PrepareHitCallBack(hitAction);

            user.animator.Play("Attack");

            while (!damageApplied)
            {
                yield return null;
            }

            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);

            float timeLeft = stateInfo.length * (1f - (stateInfo.normalizedTime % 1f));

            if (timeLeft > 0)
            {
                yield return new WaitForSeconds(timeLeft);
            }

            user.animator.Play("Idle");

        }

        private void CalculateFinalDamage()
        {
            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

        }

        private IEnumerator RotateBackToInitial()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            Quaternion endRotation = user.initialRotation;

            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = endRotation;
        }

        private void SpawnEffectAtTarget()
        {
            GameObject effectToSpawn = null;
            string effectPath = string.Empty;

            switch (user.characterClass)
            {
                case CharacterClass.Magical:
                    effectPath = "Effects/Effect1";
                    break;
                case CharacterClass.Summon:
                    effectPath = "Effects/Effect2";
                    break;
                case CharacterClass.Tank:
                    effectPath = "Effects/Effect3";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(effectPath))
            {
                effectToSpawn = Resources.Load<GameObject>(effectPath);
            }

            if (effectToSpawn != null)
            {
                GameObject effectInstance = GameObject.Instantiate(effectToSpawn, target.transform.position, Quaternion.identity);

                GameObject.Destroy(effectInstance, 3f);
            }
            else
            {
                Debug.LogError($"Không tìm thấy hiệu ứng tại đường dẫn: {effectPath} cho lớp: {user.characterClass}");
            }
        }
    }
}