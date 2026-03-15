using System.Collections.Generic;
using UnityEngine;

public class TopdownSkillDataBase : Singleton<TopdownSkillDataBase>
{
    [SerializeField]
    private List<SkillStrategy> _skillStrategies;

    public SkillStrategy GetSkillStrategyByName(string name)
    {
        foreach (var strategy in _skillStrategies)
        {
            if(name == strategy.name)
                return strategy;
        }
        return null;
    }
}
