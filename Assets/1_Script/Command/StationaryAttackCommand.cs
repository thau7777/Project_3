using System.Collections;
using UnityEngine;

public class StationaryAttackCommand : SkillCommand
{
    private float rotationDuration = 0.25f;
    private BattleManager battleManager;

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
        user.transform.rotation = lookRotation;

        float attackDuration = 0f;

        user.animator.Play("Attack");

        yield return new WaitForSeconds(0.1f);

        attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(attackDuration);

        int effectiveAttack = user.stats.attack; 

        int finalDamage = effectiveAttack * skill.damage;

        target.TakeDamage(finalDamage);

        yield return new WaitForSeconds(0.2f);

        user.animator.Play("Idle");

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
}