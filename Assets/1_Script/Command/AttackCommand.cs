using System.Collections;
using UnityEngine;

public class AttackCommand : SkillCommand
{
    private float moveSpeed = 5f;

    float attackDuration = 0f;
    private float rotationDuration = 0.25f; 

    public AttackCommand(Character user, Character target, Skill skill)
    : base(user, target, skill) { }

    public override IEnumerator Execute()
    {
        Vector3 initialPosition = user.initialPosition;
        float attackDistance = 1.5f;
        float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
        Vector3 destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

        user.animator.Play("Run");
        user.animator.SetBool("IsRunning", true);
        while (Vector3.Distance(user.transform.position, destination) > 0.1f)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;

        user.animator.SetBool("IsRunning", false);
        user.animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDuration);

        target.TakeDamage(skill.damage);
        yield return new WaitForSeconds(attackDuration);

        user.animator.SetBool("IsRunning", true);
        while (Vector3.Distance(user.transform.position, initialPosition) > 0.1f)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, initialPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        user.animator.SetBool("IsRunning", false);

        Quaternion startRotation = user.transform.rotation;

        Quaternion endRotation = user.initialRotation;

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        user.transform.rotation = endRotation;
    }
}