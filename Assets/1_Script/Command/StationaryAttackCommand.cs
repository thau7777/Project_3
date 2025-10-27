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
            int offensiveStat = user.stats.attack;
            int defensiveStat = target.stats.defense; 

            finalDamage = offensiveStat * skill.damage;

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    target.TakeDamage(finalDamage);
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

            float attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(attackDuration);
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
                    effectPath = "Effects/Attack1";
                    break;
                case CharacterClass.Summon:
                    effectPath = "Effects/Effect27";
                    break;
                case CharacterClass.Tank:
                    effectPath = "Effects/Effect7";
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