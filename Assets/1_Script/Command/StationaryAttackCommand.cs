using System.Collections;
using UnityEngine;

public class StationaryAttackCommand : SkillCommand
{
    private float rotationDuration = 0.25f; 

    public StationaryAttackCommand(Character user, Character target, Skill skill)
        : base(user, target, skill) { }

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
        AnimatorStateInfo stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);

        user.animator.Play("Attack");

        yield return new WaitForSeconds(0.1f);

        attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(attackDuration);

        target.TakeDamage(skill.damage);

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
    }
}