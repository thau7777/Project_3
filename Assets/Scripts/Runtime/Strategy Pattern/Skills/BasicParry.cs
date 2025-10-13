using UnityEngine;

[CreateAssetMenu(fileName = "BasicParry", menuName = "ScriptableObjects/StrategyPattern/Skills/BasicParry")]
public class BasicParry : SkillStrategy
{
    public override void Execute(IStrategyContext context)
    {
        var skillContext = context as SkillStrategyContext;
        SpawnProjectile(skillContext);
        Debug.Log($"{skillContext.origin.name} casts {SkillName}!");
    }
}
