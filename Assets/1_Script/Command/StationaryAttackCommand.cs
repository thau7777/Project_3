using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


namespace Turnbase
{
    public class StationaryAttackCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;

        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        //private bool animationFinished = false;

        public StationaryAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Vector3 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);

            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;

            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            int effectiveAttack = user.stats.attack;
            finalDamage = effectiveAttack * skill.damage;

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



            elapsed = 0f;
            startRotation = user.transform.rotation;
            Quaternion endRotation = user.initialRotation;


            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            user.transform.rotation = endRotation;
            yield return new WaitForSeconds(attackDuration);



            battleManager.EndTurn(user);

        }

        private void SpawnEffectAtTarget()
        {
            GameObject effectToSpawn = null;
            string effectPath = string.Empty;

            switch (user.characterClass)
            {
                case CharacterClass.Magical:
                    effectPath = "Effects/Effect7";
                    break;
                case CharacterClass.Summon:
                    effectPath = "Effects/Effect27";
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
