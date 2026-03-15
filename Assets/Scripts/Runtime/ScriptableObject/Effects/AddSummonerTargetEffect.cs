using UnityEngine;

[CreateAssetMenu(fileName = "New SummonerTarget Effect", menuName = "Scriptable Objects/Effect/Summoner Target Effect")]
public class AddSummonerTargetEffect : Effect
{
    public override Flyweight OnApply(GameObject sender, GameObject target, ActiveEffect activeEffect)
    {
        var vfx = base.OnApply(sender, target, activeEffect);
        if(GetVfxFlyweightOnTarget(target) != null)
            GetVfxFlyweightOnTarget(target).transform.position = target.transform.position.Add(y: 1);
        MinionsManager.Instance.AddTargetedEnemies(target);
        return vfx;
    }

    public override void OnRemove(ActiveEffect activeEffect, GameObject target = null)
    {
        MinionsManager.Instance.RemoveTargetedEnemy(activeEffect.activeVFX.transform.root.gameObject);
        base.OnRemove(activeEffect);
    }
}
