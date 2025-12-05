using UnityEngine;

[CreateAssetMenu(fileName = "New SummonerTarget Effect", menuName = "Scriptable Objects/Effect/Summoner Target Effect")]
public class AddSummonerTargetEffect : Effect
{
    public override void OnApply(GameObject target)
    {
        base.OnApply(target);
        GetVfxFlyweightOnTarget(target).transform.position = target.transform.position.Add(y: 1);
        MinionsManager.Instance.AddTargetedEnemies(target);
    }

    public override void OnRemove(GameObject target)
    {
        base.OnRemove(target);
        MinionsManager.Instance.RemoveTargetedEnemy(target);
    }
}
