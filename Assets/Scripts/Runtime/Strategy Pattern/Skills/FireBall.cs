using UnityEngine;

[CreateAssetMenu(fileName = "FireBall", menuName = "ScriptableObjects/StrategyPattern/Skills/FireBall")]
public class FireBall : SkillStrategy
{
    public override void Execute(IStrategyContext context)
    {
        var skillContext = context as SkillStrategyContext;
        SpawnProjectile(skillContext);
        Debug.Log($"{skillContext.origin.name} casts {SkillName}!");
    }
}
