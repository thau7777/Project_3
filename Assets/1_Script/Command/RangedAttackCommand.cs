using System;
using System.Collections;
using UnityEngine;

public class RangedAttackCommand : SkillCommand
{
    private int finalDamage;
    private bool damageApplied = false;

    private float rotationDuration = 0.25f;
    private BattleManager battleManager;

    public RangedAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
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
        user.transform.rotation = lookRotation;

        int effectiveAttack = user.stats.attack;
        finalDamage = effectiveAttack * skill.damage;

        Action hitAction = () =>
        {
            if (!damageApplied)
            {
                target.TakeDamage(finalDamage);
                damageApplied = true;
                SpawnImpactEffect(target.transform.position);
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


        elapsed = 0f;
        startRotation = user.transform.rotation;
        Quaternion endRotation = user.initialRotation;

        while (elapsed < rotationDuration)
        {
            user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        user.transform.rotation = endRotation;

        battleManager.EndTurn(user);
    }

    private void SpawnImpactEffect(Vector3 position)
    {
        GameObject effectToSpawn = skill.impactVFXPrefab;

        if (effectToSpawn != null)
        {
            GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);
            GameObject.Destroy(effectInstance, 3f);
        }
        else
        {
            Debug.LogWarning($"Thiếu Prefab Impact VFX cho kỹ năng: {skill.skillName}");
        }
    }
}