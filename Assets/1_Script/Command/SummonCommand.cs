using System.Collections;
using UnityEngine;

public class SummonCommand : SkillCommand
{
    public SummonCommand(Character user, Character target, Skill skill) : base(user, target, skill) { }

    public override IEnumerator Execute()
    {
        Debug.Log($"{user.name} sử dụng kỹ năng triệu hồi {skill.skillName}");
        user.animator.Play("Cast");
        yield return new WaitForSeconds(1.5f);

        


        yield return null;
    }

}
