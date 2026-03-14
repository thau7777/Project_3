using System;
using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class StationaryAttackCommand : SkillCommand
    {
        private int finalDamage;
        private bool projectileHit = false;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;
        private Quaternion targetLookRotation;

        public StationaryAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            targetLookRotation = GetTargetLookRotation();

            yield return RotateTowardsTarget();

            yield return PerformStationaryProjectileAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private Quaternion GetTargetLookRotation()
        {
            Vector3 targetPos = target.buffEffectSpawnPoint != null ? target.buffEffectSpawnPoint.position : target.transform.position + Vector3.up;
            Vector3 direction = (targetPos - user.transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
            return lookRotation;
        }

        private IEnumerator RotateTowardsTarget()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, targetLookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = targetLookRotation;
        }

        private IEnumerator PerformStationaryProjectileAttack()
        {
            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
            projectileHit = false;

            Action fireAction = () =>
            {
                SpawnProjectileByClass(finalDamage);
            };

            user.PrepareHitCallBack(fireAction);
            user.animator.Play("Attack");

            float startTime = Time.time;
            float timeout = 4.0f;
            while (!projectileHit && Time.time < startTime + timeout)
            {
                yield return null;
            }

            if (!projectileHit)
            {
                target.TakeDamage(user, finalDamage, skill != null ? skill.elementType : ElementType.Magical);
                projectileHit = true;
            }

            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            float timeLeft = stateInfo.length * (1f - (stateInfo.normalizedTime % 1f));
            if (timeLeft > 0) yield return new WaitForSeconds(timeLeft);

            user.animator.Play("Idle");
        }

        private void SpawnProjectileByClass(int damage)
        {
            string projectilePath = string.Empty;

            switch (user.characterClass)
            {
                case CharacterClass.Magical:
                case CharacterClass.Summon:
                case CharacterClass.Tank:
                    projectilePath = "Projectiles/MagicalBullet";
                    break;
                default:
                    projectilePath = "Projectiles/DefaultBullet";
                    break;
            }

            GameObject projectilePrefab = Resources.Load<GameObject>(projectilePath);

            if (projectilePrefab != null)
            {
                GameObject pObj = GameObject.Instantiate(projectilePrefab);

                Vector3 spawnPos = user.SkillSpawnPoint != null ? user.SkillSpawnPoint.position : user.transform.position + Vector3.up;

                Vector3 targetPos = target.buffEffectSpawnPoint != null
                    ? target.buffEffectSpawnPoint.position
                    : target.transform.position + Vector3.up;

                Vector3 shootDirection = (targetPos - spawnPos).normalized;
                Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

                pObj.transform.position = spawnPos;
                pObj.transform.rotation = shootRotation;

                ProjectileTurnBase pScript = pObj.GetComponent<ProjectileTurnBase>();
                if (pScript != null)
                {
                    Action hitCallback = () => { projectileHit = true; };
                    pScript.Setup(user, target, skill, damage, skill != null ? skill.elementType : ElementType.Normal, hitCallback);
                }
            }
            else
            {
                Debug.LogError($"Khong tim thay Projectile tai: {projectilePath}");
                projectileHit = true;
            }
        }

        private IEnumerator RotateBackToInitial()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, user.initialRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = user.initialRotation;
        }
    }
}